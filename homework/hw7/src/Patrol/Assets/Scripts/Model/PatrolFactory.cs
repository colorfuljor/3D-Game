﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolFactory : MonoBehaviour
{
    private GameObject patrol = null;                              //巡逻兵
    private List<GameObject> used = new List<GameObject>();        //正在被使用的巡逻兵，该游戏巡逻兵不需要回收，所以不需要free表
    private Vector3[] position = new Vector3[9];                   //保存每个巡逻兵的初始位置

    public FirstController sceneControler;                         //场景控制器

    public List<GameObject> GetPatrols()
    {
        int[] pos_x = { 1, -4};
        int[] pos_z = { 4, -1};
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
        for (int i = 0; i < 4; i++)
        {
            patrol = Instantiate(Resources.Load<GameObject>("Prefabs/zombie"));
            patrol.transform.position = position[i];
            patrol.AddComponent<PatrolData>();
            patrol.GetComponent<PatrolData>().sign = i + 1;
            patrol.GetComponent<PatrolData>().startPosition = position[i];
            used.Add(patrol);
        }
        return used;
    }

    public void StopPatrol()
    {
        //切换所有侦查兵的动画
        for (int i = 0; i < used.Count; i++)
        {
            used[i].gameObject.GetComponent<Animator>().SetBool("run", false);
        }
    }
}
