using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolActionManager : SSActionManager
{
    //外界调用的动作
    private PatrolAction patrolAction;                            //巡逻兵巡逻

    //外界调用动作的接口
    public void Patrol(GameObject patrol)
    {
        patrolAction = PatrolAction.GetSSAction(patrol.transform.position);
        this.RunAction(patrol, patrolAction, this);
    }


    //停止所有动作
    public void DestroyAllAction()
    {
        DestroyAll();
    }
}
