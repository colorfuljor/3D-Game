using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSAction : ScriptableObject            //动作
{

    public bool enable = true;                      //是否正在进行此动作
    public bool destroy = false;                    //是否需要被销毁

    public GameObject gameobject;                   //动作对象
    public Transform transform;                     //动作对象的transform
    public ISSActionCallback callback;              //回调函数

    protected SSAction() { }                        //保证SSAction不会被new

    public virtual void Start()                    //子类可以使用这两个函数
    {
        throw new System.NotImplementedException();
    }

    public virtual void Update()
    {
        throw new System.NotImplementedException();
    }
}

public class UFOFlyAction : SSAction                       
{

    public float gravity = -5;                                 //向下的加速度
    private Vector3 startVector;                              //初速度向量
    private Vector3 gravityVector = Vector3.zero;             //加速度的向量，初始时为0
    private float time;                                        //已经过去的时间
    private Vector3 currentAngle = Vector3.zero;               //当前时间的欧拉角
    public bool run = true;

    private UFOFlyAction() { }
    public static UFOFlyAction GetSSAction(Vector3 direction, float angle, float power)
    {
        //初始化物体将要运动的初速度向量
        UFOFlyAction action = CreateInstance<UFOFlyAction>();
        if (direction.x == -1)
        {
            action.startVector = Quaternion.Euler(new Vector3(0, 0, -angle)) * Vector3.left * power;
        }
        else
        {
            action.startVector = Quaternion.Euler(new Vector3(0, 0, angle)) * Vector3.right * power;
        }
        return action;
    }

    public override void Update()
    {
        if (run)
        {
            //斜抛运动
            //计算物体的向下的速度,v=at
            time += Time.fixedDeltaTime;
            gravityVector.y = gravity * time;

            //位移模拟
            transform.position += (startVector + gravityVector) * Time.fixedDeltaTime;
            currentAngle.z = Mathf.Atan((startVector.y + gravityVector.y) / startVector.x) * Mathf.Rad2Deg;
            transform.eulerAngles = currentAngle;

            //如果物体y坐标小于-10，动作就做完了
            if (this.transform.position.y < -10)
            {
                this.destroy = true;
                this.callback.SSActionEvent(this);
            }
        } 
    }
    public override void Start() { }
}

public class SequenceAction : SSAction, ISSActionCallback
{
    public List<SSAction> sequence;    //动作的列表
    public int repeat = -1;            //-1就是无限循环做组合中的动作
    public int start = 0;              //当前做的动作的索引

    public static SequenceAction GetSSAcition(int repeat, int start, List<SSAction> sequence)
    {
        SequenceAction action = ScriptableObject.CreateInstance<SequenceAction>();//让unity自己创建一个SequenceAction实例
        action.repeat = repeat;
        action.sequence = sequence;
        action.start = start;
        return action;
    }

    public override void Update()
    {
        if (sequence.Count == 0) return;
        if (start < sequence.Count)
        {
            sequence[start].Update();     //一个组合中的一个动作执行完后会调用接口,所以这里看似没有start++实则是在回调接口函数中实现
        }
    }

    public void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Competeted,
        int intParam = 0, string strParam = null, Object objectParam = null)
    {
        source.destroy = false;          //先保留这个动作，如果是无限循环动作组合之后还需要使用
        this.start++;
        if (this.start >= sequence.Count)
        {
            this.start = 0;
            if (repeat > 0) repeat--;
            if (repeat == 0)
            {
                this.destroy = true;               //整个组合动作就删除
                this.callback.SSActionEvent(this); //告诉组合动作的管理对象组合做完了
            }
        }
    }

    public override void Start()
    {
        foreach (SSAction action in sequence)
        {
            action.gameobject = this.gameobject;
            action.transform = this.transform;
            action.callback = this;                //组合动作的每个小的动作的回调是这个组合动作
            action.Start();
        }
    }

    void OnDestroy()
    {
        //如果组合动作做完第一个动作突然不要它继续做了，那么后面的具体的动作需要被释放
    }
}

public enum SSActionEventType : int { Started, Competeted }

public interface ISSActionCallback
{
    void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Competeted,
        int intParam = 0, string strParam = null, Object objectParam = null);
}

public class SSActionManager : MonoBehaviour, ISSActionCallback                      //action管理器
{

    private Dictionary<int, SSAction> actions = new Dictionary<int, SSAction>();    //将执行的动作的字典集合,int为key，SSAction为value
    private List<SSAction> waitingAdd = new List<SSAction>();                       //等待去执行的动作列表
    private List<int> waitingDelete = new List<int>();                              //等待删除的动作的key                

    protected void Update()
    {
        foreach (SSAction ac in waitingAdd)
        {
            actions[ac.GetInstanceID()] = ac;                                      //获取动作实例的ID作为key
        }
        waitingAdd.Clear();

        foreach (KeyValuePair<int, SSAction> kv in actions)
        {
            SSAction ac = kv.Value;
            if (ac.destroy)
            {
                waitingDelete.Add(ac.GetInstanceID());
            }
            else if (ac.enable)
            {
                ac.Update();
            }
        }

        foreach (int key in waitingDelete)
        {
            SSAction ac = actions[key];
            actions.Remove(key);
            DestroyObject(ac);
        }
        waitingDelete.Clear();
    }

    public void RunAction(GameObject gameobject, SSAction action, ISSActionCallback manager)
    {
        action.gameobject = gameobject;
        action.transform = gameobject.transform;
        action.callback = manager;
        waitingAdd.Add(action);
        action.Start();
    }

    public void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Competeted,
        int intParam = 0, string strParam = null, Object objectParam = null)
    {

    }
}

public class FlyActionManager : SSActionManager  //本游戏管理器
{

    private UFOFlyAction fly;     //飞行动作，这次只有单独动作，没有组合动作

    public FirstController sceneController;

    protected void Start()
    {
        sceneController = (FirstController)SSDirector.GetInstance().CurrentScenceController;
        sceneController.actionManager = this;
    }

    public void UFOFly(GameObject disk, float angle)
    {
        fly = UFOFlyAction.GetSSAction(disk.GetComponent<DiskData>().direction, angle, disk.GetComponent<DiskData>().speed);
        this.RunAction(disk, fly, this);
    }

    public void Pause()
    {
        fly.run = false;
    }

    public void Run()
    {
        fly.run = true;
    }

}