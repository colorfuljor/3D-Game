using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollide : MonoBehaviour
{
    void OnCollisionEnter(Collision other)
    {
        //当玩家与巡逻兵相撞
        if (other.gameObject.tag == "Player")
        {
            //游戏结束，发布消息
            Singleton<GameEventManager>.Instance.Gameover();
        }
    }
}
