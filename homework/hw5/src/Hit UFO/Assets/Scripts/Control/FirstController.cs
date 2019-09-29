using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstController : MonoBehaviour, ISceneController, IUserAction
{
    readonly float roundTime = 40f;
    private float speed = 1.5f;                                                //发射一个飞碟的时间间隔
    readonly int[] passScore = { 20, 50 };

    private List<GameObject> disks = new List<GameObject>();          //飞碟队列
    private int currentRound = 0;                                                   //回合
    private float time = 0f;                                                 //记录时间间隔
    private float currrentTime = 0f;
    private GameState gameState = GameState.START;

    public UserGUI userGUI;
    public ScoreRecorder scoreRecorder;      //计分   
    public DiskFactory diskFactory;          //生成和回收飞碟
    public FlyActionManager actionManager;   //动作管理


    // Start is called before the first frame update
    void Start()
    {
        SSDirector director = SSDirector.GetInstance();
        director.CurrentScenceController = this;
        diskFactory = Singleton<DiskFactory>.Instance;
        scoreRecorder = Singleton<ScoreRecorder>.Instance;
        actionManager = gameObject.AddComponent<FlyActionManager>() as FlyActionManager;
        userGUI = gameObject.AddComponent<UserGUI>() as UserGUI;

        gameState = GameState.START;
        time = 0f;
        currentRound = 0;
        currrentTime = 0;

        LoadResources();
    }

    void Update()
    {
        if (gameState == GameState.RUNNING ) 
        {
            for (int i = 0; i < disks.Count; i++)
            {
                //飞碟飞出摄像机视野也没被打中
                if ((disks[i].transform.position.y <= -4.5) && disks[i].gameObject.activeSelf == true)
                {
                    diskFactory.FreeDisk(disks[i]);
                    disks.Remove(disks[i]);
                    scoreRecorder.Miss();
                }
            }
            if (time > speed)
            {
                time = 0;
                SendDisk();
            }
            else
            {
                time += Time.deltaTime;
            }

            if (currrentTime > roundTime)
            {
                currrentTime = 0;
                if (currentRound < 2 && GetScore() >= passScore[currentRound])
                {
                    currentRound++;
                    time = 0f;
                }
                else
                {
                    GameOver();
                } 
            }
            else
            {
                currrentTime += Time.deltaTime;
            }
        }
    }

    private void GameOver()
    {
        gameState = GameState.OVER;
        currrentTime = 40;
    }

    public void LoadResources()              
    {
        //不需要加载，飞碟由diskFactory生产了
    }

    public void Hit(Vector3 pos)
    {
        Ray ray = Camera.main.ScreenPointToRay(pos);
        RaycastHit[] hits;
        hits = Physics.RaycastAll(ray);
        bool isHit = false;
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];

            
            if (hit.collider.gameObject.GetComponent<DiskData>() != null)       //射线打中某物体
            {
                for (int j = 0; j < disks.Count; j++)        //射中的物体要在飞碟列表中
                {
                    if (hit.collider.gameObject.GetInstanceID() == disks[j].gameObject.GetInstanceID())
                    {
                        isHit = true;
                    }
                }
                if (!isHit)        //如果没有打中，返回
                {
                    return;
                }

                disks.Remove(hit.collider.gameObject);          //从队列中移除

                scoreRecorder.Record(hit.collider.gameObject);      //记分员记录分数
                
                StartCoroutine(WaitingParticle(0.08f, hit, diskFactory, hit.collider.gameObject));      //等0.08秒后执行回收飞碟，这一步很关键
                break;
            }
        }
    }
    //暂停几秒后回收飞碟
    IEnumerator WaitingParticle(float waitTime, RaycastHit hit, DiskFactory diskFactory, GameObject obj)
    {
        yield return new WaitForSeconds(waitTime);
        //等待之后执行的动作  
        hit.collider.gameObject.transform.position = new Vector3(0, -9, 0);
        diskFactory.FreeDisk(obj);
    }

    //发送飞碟
    private void SendDisk()
    {
        
        GameObject disk = diskFactory.GetDisk(currentRound);
        disks.Add(disk);
        disk.SetActive(true);
        //设置被隐藏了或是新建的飞碟的位置
        float positionX = 16;
        float ranY = Random.Range(1f, 4f);
        float ranX = Random.Range(-1f, 1f) < 0 ? -1 : 1;
        disk.GetComponent<DiskData>().direction = new Vector3(ranX, ranY, 0);
        Vector3 position = new Vector3(-disk.GetComponent<DiskData>().direction.x * positionX, ranY, 0);
        disk.transform.position = position;
        //设置飞碟初始角度
        float angle = Random.Range(15f, 20f);
        actionManager.UFOFly(disk, angle);
        if (disk.GetComponent<DiskData>().color == Color.blue)
        {
            GameObject disk1 = Instantiate(disk);
            GameObject disk2 = Instantiate(disk);
            disks.Add(disk1);
            disk1.SetActive(true);
            disk1.GetComponent<DiskData>().direction = new Vector3(ranX, ranY, 0);
            disk1.transform.position = position;
            actionManager.UFOFly(disk1, Random.Range(15f, 28f));

            disks.Add(disk2);
            disk2.SetActive(true);
            disk2.GetComponent<DiskData>().direction = new Vector3(ranX, ranY, 0);
            disk2.transform.position = position;
            actionManager.UFOFly(disk2, Random.Range(15f, 28f));

        }
    }

    public void Restart()
    {
        time = 0f;
        currentRound = 0;
        currrentTime = 0;
        scoreRecorder.Reset();
        gameState = GameState.RUNNING;
    }

    public int GetScore()
    {
        return scoreRecorder.score;
    }

    public void SetGameState(GameState state)
    {
        gameState = state;
    }

    public GameState GetGameState()
    {
        return gameState;
    }

    public float GetTime()
    {
        return currrentTime;
    }

    public int GetRound()
    {
        return currentRound;
    }

    public void Pause()
    {
        actionManager.Pause();
    }

    public void Run()
    {
        actionManager.Run();
    }
}   
