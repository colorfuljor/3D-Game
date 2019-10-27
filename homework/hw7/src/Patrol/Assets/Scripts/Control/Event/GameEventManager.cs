using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventManager : MonoBehaviour
{
    //分数变化
    public delegate void ScoreEvent();
    public static event ScoreEvent ScoreChange;
    //游戏结束变化
    public delegate void GameoverEvent();
    public static event GameoverEvent GameoverChange;

    //玩家逃脱
    public void Escape()
    {
        if (ScoreChange != null)
        {
            ScoreChange();  //玩家逃脱则加一分
        }
    }
    //玩家被捕
    public void Gameover()
    {
        if (GameoverChange != null)
        {
            GameoverChange();   //玩家被捕即游戏结束
        }
    }
}
