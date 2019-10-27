using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowAction : SSAction
{
    private float speed = 2f;            //跟随玩家的速度
    private GameObject player;           //玩家
    private PatrolData data;             //侦查兵数据

    private FollowAction() { }
    public static FollowAction GetSSAction(GameObject player)
    {
        FollowAction action = CreateInstance<FollowAction>();
        action.player = player;
        return action;
    }

    public override void Update()
    {
        //防止碰撞发生后的旋转
        if (transform.localEulerAngles.x != 0 || transform.localEulerAngles.z != 0)
        {
            transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
        }
        if (transform.position.y != 0)
        {
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        }

        Follow();

        //如果侦察兵没有跟随对象，或者需要跟随的玩家不在侦查兵的区域内
        if (data.wallSign != data.sign)
        {
            this.destroy = true;
            this.enable = false;
            this.callback.SSActionEvent(this, 1, this.gameobject);
        }
    }
    public override void Start()
    {
        data = this.gameobject.GetComponent<PatrolData>();
    }
    void Follow()
    {
        this.transform.position = Vector3.MoveTowards(this.transform.position, player.transform.position, speed * Time.deltaTime);
        this.transform.LookAt(player.transform.position);
    }
}
