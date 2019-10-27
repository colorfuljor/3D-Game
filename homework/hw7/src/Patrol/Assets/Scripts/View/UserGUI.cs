using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserGUI : MonoBehaviour
{
    private IUserAction action;

    GUIStyle labelStyle;
    GUIStyle buttonStyle;
    GUIStyle pauseStyle;
    GUIStyle resultStyle;

    void Start()
    {
        action = SSDirector.GetInstance().CurrentScenceController as IUserAction;

        labelStyle = new GUIStyle();
        labelStyle.fontSize = 20;

        buttonStyle = new GUIStyle("button");
        buttonStyle.fontSize = 30;

        resultStyle = new GUIStyle();
        resultStyle.fontSize = 50;
        resultStyle.alignment = TextAnchor.MiddleCenter;
        resultStyle.normal.textColor = Color.white;
    }

    void Update()
    {
        //获取方向键的偏移量
        float translationX = Input.GetAxis("Horizontal");
        float translationZ = Input.GetAxis("Vertical");
        //移动玩家
        if (action.GetGameState() == GameState.RUNNING)
            action.MovePlayer(translationX, translationZ);
    }

    private void OnGUI()
    {
        if (action.GetGameState() != GameState.START)
        {
            GUI.Label(new Rect(20, 40, 80, 20), "Score: " + action.GetScore().ToString(), labelStyle);
        }

        if (action.GetGameState() == GameState.START && GUI.Button(new Rect(320, 280, 130, 55), "Start", buttonStyle))
        {
            action.SetGameState(GameState.RUNNING);
            action.GameStart();
        }

        if (action.GetGameState() == GameState.OVER)
        {
            if (GUI.Button(new Rect(320, 280, 130, 55), "Restart", buttonStyle))
                action.Restart();

            GUI.Label(new Rect(285, 130, 200, 50), "Your score is " + action.GetScore().ToString() + "!", resultStyle);
        }
    }
}
