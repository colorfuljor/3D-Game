using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolAction : SSAction
{
    private enum Dirction { EAST, NORTH, WEST, SOUTH };
    private float posX, posZ;                 //移动前的初始x和z方向坐标
    private float moveLength;                  //移动的长度
    private float moveSpeed = 1.2f;            //移动速度
    private bool moveSign = true;              //是否到达目的地
    private Dirction dirction = Dirction.WEST;  //移动的方向
    private PatrolData data;                    //侦察兵的数据


    private PatrolAction() { }                      

    public static PatrolAction GetSSAction(Vector3 location)
    {
        PatrolAction action = CreateInstance<PatrolAction>();
        action.posX = location.x;
        action.posZ = location.z;

        //设定移动矩形的边长
        action.moveLength = Random.Range(3, 3);
        return action;
    }

    public override void Start()
    {
        this.gameobject.GetComponent<Animator>().enabled = true;
        data = this.gameobject.GetComponent<PatrolData>();
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

        //如果侦察兵需要跟随玩家并且玩家就在侦察兵所在的区域，侦查动作结束
        if (data.wallSign == data.sign)    //由事件触发
        {
            this.destroy = true;
            this.enable = false;
            this.callback.SSActionEvent(this, 0, this.gameobject);
        }
        else
        {
            Patrol();
        }
    }
    

    private void Patrol()
    {
        if (moveSign)
        {
            //不需要转向则设定一个目的地，按照矩形移动
            switch (dirction)
            {
                case Dirction.EAST:
                    posX -= moveLength;
                    break;
                case Dirction.NORTH:
                    posZ += moveLength;
                    break;
                case Dirction.WEST:
                    posX += moveLength;
                    break;
                case Dirction.SOUTH:
                    posZ -= moveLength;
                    break;
            }
            moveSign = false;
        }
        this.transform.LookAt(new Vector3(posX, 0, posZ));
        float distance = Vector3.Distance(transform.position, new Vector3(posX, 0, posZ));

        //当前位置与目的地距离浮点数的比较决定是否转向
        if (distance > 0.9)
        {
            transform.position = Vector3.MoveTowards(this.transform.position, new Vector3(posX, 0, posZ), moveSpeed * Time.deltaTime);
        }
        else
        {
            dirction = dirction + 1;
            if (dirction > Dirction.SOUTH)
            {
                dirction = Dirction.EAST;
            }
            moveSign = true;
        }
    }
}
