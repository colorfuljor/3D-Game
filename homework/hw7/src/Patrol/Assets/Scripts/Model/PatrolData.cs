using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolData : MonoBehaviour
{
    public int sign;                      //标志巡逻兵在哪一块区域
    public int wallSign = -1;            //当前玩家所在区域标志
    public GameObject player;             //玩家游戏对象,用于跟随
    public Vector3 startPosition;        //当前巡逻兵初始位置    
}
