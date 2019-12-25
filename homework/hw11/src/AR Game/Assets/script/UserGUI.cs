using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserGUI : MonoBehaviour
{
    public GameObject dragon;// 龙
    private bool isMove = false;
    Vector3 dest;
    public float speed = 0.05f;

    void OnGUI()
    {
        if (GUI.Button(new Rect(50, Screen.height - 100, 90, 45), "左转"))
        {
            dragon.transform.Rotate(new Vector3(0, -90, 0));
        }
        if (GUI.Button(new Rect(Screen.width - 140, Screen.height - 100, 90, 45), "右转"))
        {
            dragon.transform.Rotate(new Vector3(0, 90, 0));
        }
    }
}
