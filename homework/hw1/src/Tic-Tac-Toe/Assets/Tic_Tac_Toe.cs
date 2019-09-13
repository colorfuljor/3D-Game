using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tic_Tac_Toe : MonoBehaviour
{
    // 0 presents nothing, 1 presents "O", 2 presents "X"
    private int[,] chess = new int[3, 3];
    private int turn = 1;
    private bool start = false;

    //Reset the game
    private void Reset()
    {
        turn = 1;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                chess[i, j] = 0;
            }
        }
    }

    private int isWin()
    {
        //Vertical same
        for (int i = 0; i < 3; i++)
        {
            if (chess[i, 0] != 0 && chess[i, 0] == chess[i, 1] && chess[i, 0] == chess[i, 2])
            {
                return chess[i, 0];
            }
        }

        //Parallel same
        for (int j = 0; j < 3; j++)
        {
            if (chess[0, j] != 0 && chess[0, j] == chess[1, j] && chess[0, j] == chess[2, j])
            {
                return chess[0, j];
            }
        }

        //diagonal same
        if (chess[0, 0] != 0 && chess[0, 0] == chess[1, 1] && chess[0, 0] == chess[2, 2]) return chess[1, 1];
        if (chess[0, 2] != 0 && chess[0, 2] == chess[1, 1] && chess[0, 2] == chess[2, 0]) return chess[1, 1];

        //Judge if it's locked
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (chess[i, j] == 0)
                {
                    return 0;
                }
            }
        }
        return 3;
    }

    // Start is called before the first frame update
    void Start()
    {
        start = false;
        Reset();  
    }

    //Generate GUI
    void OnGUI()
    {
        GUI.skin.button.fontSize = 20;
        GUI.skin.label.fontSize = 30;
        if (!start)
        {
            if (GUI.Button(new Rect(300, 300, 150, 50), "Start"))
            {
                start = true;
            }
            GUI.skin.label.fontSize = 60;
            GUI.Label(new Rect(218, 110, 314, 100), "Tic-Tac-Toe");
        }
        else
        {
            if (GUI.Button(new Rect(300, 340, 150, 50), "Reset"))
            {
                Reset();
            }

            int state = isWin();
            if (state == 1)
            {
                GUI.Label(new Rect(322, 20, 106, 50), "O Wins!");
            }
            else if (state == 2)
            {
                GUI.Label(new Rect(323, 20, 104, 50), "X Wins!");
            }
            else if (state == 3)
            {
                GUI.Label(new Rect(284, 20, 182, 50), "No one Wins.");
            }
            else
            {
                if (turn == 1)
                    GUI.Label(new Rect(364, 6, 22, 50), "O");
                else if (turn == 2)
                    GUI.Label(new Rect(364, 20, 22, 50), "X");
            }
            

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (chess[i, j] == 1)
                    {
                        GUI.Button(new Rect(i * 80 + 255, j * 80 + 80, 80, 80), "O");
                    }
                    else if (chess[i, j] == 2)
                    {
                        GUI.Button(new Rect(i * 80 + 255, j * 80 + 80, 80, 80), "X");
                    }

                    if (GUI.Button(new Rect(i * 80 + 255, j * 80 + 80, 80, 80), "") && state == 0)
                    {
                        chess[i, j] = turn;
                        turn = 3 - turn;
                    }
                }
            }
        }
    }
 
}
