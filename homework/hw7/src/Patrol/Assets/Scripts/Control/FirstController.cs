using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstController : MonoBehaviour, IUserAction, ISceneController
{
    public PatrolFactory patrolFactory;                               //巡逻者工厂
    public ScoreRecorder scoreRecorder;                               //记分员
    public PatrolActionManager actionManager;                         //动作管理器
    public UserGUI userGUI;                                           //用户图形界面
    public GameObject player;                                         //玩家
    public List<GameObject> patrols;                                  //巡逻兵队列

    private GameState gameState;                                      //游戏状态
    private readonly float playerSpeed = 5;                           //玩家移动速度

    void Start()
    {
        SSDirector director = SSDirector.GetInstance();
        director.CurrentScenceController = this;
        patrolFactory = Singleton<PatrolFactory>.Instance;
        actionManager = gameObject.AddComponent<PatrolActionManager>() as PatrolActionManager;
        userGUI = gameObject.AddComponent<UserGUI>() as UserGUI;
        LoadResources();
        scoreRecorder = Singleton<ScoreRecorder>.Instance;
    }

    void Update()
    {
        int wallSign = 0;
        if (player.transform.position.x < -1 && player.transform.position.z > 1)
            wallSign = 3;
        if (player.transform.position.x > 1 && player.transform.position.z > 1)
            wallSign = 1;
        if (player.transform.position.x < -1 && player.transform.position.z < -1)
            wallSign = 4;
        if (player.transform.position.x > 1 && player.transform.position.z < -1)
            wallSign = 2;

        for (int i = 0; i < patrols.Count; i++)
        {
            patrols[i].gameObject.GetComponent<PatrolData>().wallSign = wallSign;
        }
        if (wallSign != 0)
            patrols[wallSign - 1].gameObject.GetComponent<PatrolData>().player = player;
    }

    public void LoadResources()
    {
        Instantiate(Resources.Load<GameObject>("Prefabs/Plane"));
        player = Instantiate(Resources.Load("Prefabs/player"), new Vector3(0, 0, -4), Quaternion.identity) as GameObject;
        player.GetComponent<Animator>().enabled = false;
        patrols = patrolFactory.GetPatrols();

        
        for (int i = 0; i < patrols.Count; i++)
        {
            patrols[i].GetComponent<Animator>().enabled = false;
        }        
    }

    //用户接口
    public void MovePlayer(float translationX, float translationZ)
    {
        //实际上移动玩家这个任务应该交给动作管理器完成，这里由于玩家动作简单，所以为了简化代码，就交给场景管理器代劳
        if (translationX != 0 || translationZ != 0)
        {
            player.GetComponent<Animator>().enabled = true;

            if (translationZ > 0)
            {
                player.transform.localEulerAngles = new Vector3(0, 0, 0);
                player.transform.Translate(0, 0, translationZ * playerSpeed * Time.deltaTime);
            }
            if (translationZ < 0)
            {
                player.transform.localEulerAngles = new Vector3(0, 180, 0);
                player.transform.Translate(0, 0, -translationZ * playerSpeed * Time.deltaTime);
            }
            if (translationX > 0.45)
            {
                player.transform.localEulerAngles = new Vector3(0, 90, 0);
                player.transform.Translate(0, 0, translationX * playerSpeed * Time.deltaTime);
            }
            if (translationX < -0.45)
            {
                player.transform.localEulerAngles = new Vector3(0, -90, 0);
                player.transform.Translate(0, 0, -translationX * playerSpeed * Time.deltaTime);
            }         
        }
        else
        {
            player.GetComponent<Animator>().enabled = false;
            player.transform.localEulerAngles = new Vector3(0, player.transform.localEulerAngles.y, 0);
        }

        //防止碰撞带来的移动
        if (player.transform.localEulerAngles.x != 0 || player.transform.localEulerAngles.z != 0)
        {
            player.transform.localEulerAngles = new Vector3(0, player.transform.localEulerAngles.y, 0);
        }
        if (player.transform.position.y != 0)
        {
            player.transform.position = new Vector3(player.transform.position.x, 0, player.transform.position.z);
        }
    }

    public void GameStart()
    {
        for (int i = 0; i < patrols.Count; i++)
        {
            actionManager.Patrol(patrols[i]);
            patrols[i].GetComponent<Animator>().enabled = true;
        }
    }

    public void Restart()
    {
        gameState = GameState.RUNNING;
        scoreRecorder.Reset();
        player.transform.position = new Vector3(0, 0, -4);
        player.transform.localEulerAngles = new Vector3(0, 0, 0);

        int[] pos_x = { 1, -4 };
        int[] pos_z = { 4, -1 };
        Vector3[] position = new Vector3[9];
        int index = 0;
        //生成不同的巡逻兵初始位置
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                position[index] = new Vector3(pos_x[i], 0, pos_z[j]);
                index++;
            }
        }
        for (int i = 0; i < patrols.Count; i++)
        {
            patrols[i].transform.position = position[i]; 
        }
        

        GameStart();
    }

    //订阅者模式
    void OnEnable()
    {
        GameEventManager.ScoreChange += AddScore;
        GameEventManager.GameoverChange += Gameover;
    }



    void OnDisable()
    {
        GameEventManager.ScoreChange -= AddScore;
        GameEventManager.GameoverChange -= Gameover;
    }

    private void AddScore()
    {
        scoreRecorder.Add();
    }

    private void Gameover()
    {
        this.gameState = GameState.OVER;
        player.GetComponent<Animator>().enabled = false;
        for (int i = 0; i < patrols.Count; i++)
        {
            patrols[i].GetComponent<Animator>().enabled = false;
        }
        actionManager.DestroyAll();
    }

    public GameState GetGameState()
    {
        return gameState;
    }

    public int GetScore()
    {
        return scoreRecorder.score;
    }

    public void SetGameState(GameState gameState)
    {
        this.gameState = gameState;
    }

}
