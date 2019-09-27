## 第三次作业
### 1、基本操作演练【建议做】

#### 下载 Fantasy Skybox FREE， 构建自己的游戏场景

![1](img/1.png)

#### 写一个简单的总结，总结游戏对象的使用

1. 首先需要添加一个GameObject——Terrian

![2](img/2.png)

2. 需要在Asset Store中下载一些资源，我下载了Fantasy Skybox FREE

3. 然后在这些地方对这块地进行修容

![3](img/3.png)

4. 还可以添加一些树，也可以添加人进去

![4](img/4.png)

5. 最后是设置Skybox，需要创建一个Material，并在Inspector的shader设置其为 skybox的6 sided，然后把照片拖进去

![5](img/5.png)

6. 最后的最后是要调整一些辅助工具，有如摄像头、阳光等辅助对象。

### 2、编程实践

#### 牧师与魔鬼 动作分离版
* 【2019新要求】：设计一个裁判类，当游戏达到结束条件时，通知场景控制器游戏结束

**什么是动作分离版呢？还记得我们上个版本的牧师与魔鬼为了实现character与boat的移动，实现了一个Moveable的脚本，并把此脚本作为组件加到character与boat上了。然而这样的实现方法是不完全符合MVC的分离的，动作的控制应该交由Control模块管理，而不是在Model模块中。所以这次为了实现动作分离版，我们实现一个Action模块，专门管理Model的动作，实际使用上与Moveable差别不大。**

Action模块：  
1. ISSActionCallback（动作事件接口）   
    这是一个接口，用以实现callback。也就是当某个动作完成之后，使用callback回调到调用此函数的类中实现的此接口的函数，如SSActionEvent中。

```
public enum SSActionEventType : int { Started, Competeted }

public interface ISSActionCallback
{
    void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Competeted,
        int intParam = 0, string strParam = null, Object objectParam = null);
}
```

2. SSAction （动作基类）   
    这是所有动作的基类，所有动作继承该类以实现各种动作以及组合动作。ScriptableObject 是不需要绑定 GameObject 对象的可编程基类，这些对象受 Unity 引擎场景管理。这个类更关键的是利用接口（ISSACtionCallback）实现消息通知，避免了与动作管理者直接依赖。

```
public class SSAction : ScriptableObject            //动作
{

    public bool enable = true;                      //是否正在进行此动作
    public bool destroy = false;                    //是否需要被销毁

    public GameObject gameobject { get; set; }                  //动作对象
    public Transform transform { get; set; }                     //动作对象的transform
    public ISSActionCallback callback { get; set; }              //回调函数

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
```

3. SequenceAction（组合动作实现）  
    该类继承了SSAction类，实现多个单动作的组合成一个动作，组合动作同时也是一个动作。该类利用一个List管理多个动作，并在接口方法中实现了动作的执行顺序。

```
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
```

4. SSMoveToAction（移动动作实现）  
    这是一个具体的动作实现——移动动作，当然可以实现各种各样的动作，该游戏场景只需要移动，即将某个游戏对象移动到指定的position上，且动作执行完会回调到调用者上。

```
public class SSMoveToAction : SSAction                        //移动
{
    public Vector3 target;        //移动到的目的地
    public float speed;           //移动的速度

    private SSMoveToAction() { }
    public static SSMoveToAction GetSSAction(Vector3 target, float speed)
    {
        SSMoveToAction action = ScriptableObject.CreateInstance<SSMoveToAction>();//让unity自己创建一个MoveToAction实例，并自己回收
        action.target = target;
        action.speed = speed;
        return action;
    }

    public override void Update()
    {
        this.transform.position = Vector3.MoveTowards(this.transform.position, target, speed * Time.deltaTime);
        if (this.transform.position == target)
        {
            this.destroy = true;
            this.callback.SSActionEvent(this);      //告诉动作管理或动作组合这个动作已完成
        }
    }

    public override void Start()
    {
        //移动动作建立时候不做任何事情
    }
}
```

5. SSActionManager（动作管理基类）  
    这是个动作管理者，在此游戏唯一的场景动作管理者就继承于此类，实现该场景的动作管理。动作管理管理着所有动作，控制着每个动作的执行顺序以及执行的对象等。

```
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
        //牧师与魔鬼的游戏对象移动完成后就没有下一个要做的动作了，所以回调函数为空
    }
}
```

6. MySceneActionManager（移动动作管理实现）  
    最后这就是这个场景的动作管理者，可以知道这个场景中主要有moveBoat和moveCharacter两个动作，也就是之前版本Moveable脚本实现的两个动作。通过那么复杂终于实现了动作的管理，由于这个游戏只有两个小动作，所以使用这样的管理难免会觉得繁琐。然而，如果游戏的复杂性上升，动作多了起来，使用这样一个动作管理器可以将动作集中起来共同管理，利用管理器效率就会更高。

```
public class MySceneActionManager : SSActionManager  //本游戏管理器
{

    private SSMoveToAction moveBoatToEndOrStart;     //移动船到结束岸，移动船到开始岸
    private SequenceAction moveRoleToLandorBoat;     //移动角色到陆地，移动角色到船上

    public FirstController sceneController;

    protected new void Start()
    {
        sceneController = (FirstController)SSDirector.GetInstance().CurrentScenceController;
        sceneController.actionManager = this;
    }
    public void moveBoat(GameObject boat, Vector3 target, float speed)
    {
        moveBoatToEndOrStart = SSMoveToAction.GetSSAction(target, speed);
        this.RunAction(boat, moveBoatToEndOrStart, this);
    }

    public void moveRole(GameObject role, Vector3 middle_pos, Vector3 end_pos, float speed)
    {
        SSAction action1 = SSMoveToAction.GetSSAction(middle_pos, speed);
        SSAction action2 = SSMoveToAction.GetSSAction(end_pos, speed);
        moveRoleToLandorBoat = SequenceAction.GetSSAcition(1, 0, new List<SSAction> { action1, action2 });
        this.RunAction(role, moveRoleToLandorBoat, this);
    }
}
```  
&emsp;

**除此之外，在该版本中还新增一个裁判类（Judge）以判断游戏是否结束。该功能通过场景控制者（FirstController）调用该类的判断函数，如果游戏结束裁判类会传递结束的信息回来。**

Judge实现：
1. Judge（裁判类） 

```
//Judge.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Judge
{
    public LandModel fromLand;
    public LandModel toLand;
    public BoatModel boat;

    public Judge(LandModel _fromLand, LandModel _toLand, BoatModel _boat)
    {
        fromLand = _fromLand;
        toLand = _toLand;
        boat = _boat;
    }

    public  int isOver()
    {

        int fromPriest = 0;
        int fromDevil = 0;
        int toPriest = 0;
        int toDevil = 0;

        int[] fromCount = fromLand.GetCharacterNum();
        fromPriest += fromCount[0];
        fromDevil += fromCount[1];

        int[] toCount = toLand.GetCharacterNum();
        toPriest += toCount[0];
        toDevil += toCount[1];

        if (toPriest + toDevil == 6)      // 赢了
            return 2;

        int[] boatCount = boat.GetCharacterNum();
        if (boat.GetToOrFrom() == -1)
        {
            toPriest += boatCount[0];
            toDevil += boatCount[1];
        }
        else
        {
            fromPriest += boatCount[0];
            fromDevil += boatCount[1];
        }
        if (fromPriest < fromDevil && fromPriest > 0) //输了
        {
            return 1;
        }
        if (toPriest < toDevil && toPriest > 0)
        {
            return 1;
        }
        return 0;			// 游戏未结束
    }
}

```

2. 在FirstController中
```
    private int isOver()
    {
        return judge.isOver();
    }
```
