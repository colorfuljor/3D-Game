using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IMGUI : MonoBehaviour
{
    public float health = 0.0f;
    private float resultHealth;
    void Start()
    {
        resultHealth = health;
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(200, 100, 50, 20), "Add"))
        {
            resultHealth = resultHealth + 0.1f > 1.0f ? 1.0f : resultHealth + 0.1f;
        }
        if (GUI.Button(new Rect(250, 100, 50, 20), "Ded"))
        {
            resultHealth = resultHealth - 0.1f < 0.0f ? 0.0f : resultHealth - 0.1f;
        }

        //以0.05f的速度平滑增加health，可以尝试1.0f, 0.1f以及0.01f，比较区别
        health = Mathf.Lerp(health, resultHealth, 0.05f);

        //使用水平滚动条模拟血条，起始位置为0，长度为health
        GUI.HorizontalScrollbar(new Rect(150, 60, 200, 20), 0.0f, health, 0.0f, 1.0f);
    }
}

