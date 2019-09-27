using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstController : MonoBehaviour, ISceneController, IUserAction
{
    readonly Vector3 waterPos = new Vector3(0, 0, 0);
    public LandModel fromLand;            
    public LandModel toLand;              
    public BoatModel boat;
    public Judge judge;
    private CharacterModel[] characters;
    private UserGUI userGUI;

    public MySceneActionManager actionManager;   //动作管理

    // Start is called before the first frame update
    void Start()
    {
        SSDirector director = SSDirector.GetInstance();      //得到导演实例
        director.CurrentScenceController = this;             //设置当前场景控制器
        userGUI = gameObject.AddComponent<UserGUI>() as UserGUI;  //添加UserGUI脚本作为组件
        actionManager = gameObject.AddComponent<MySceneActionManager>() as MySceneActionManager; //添加MySceneActionManager脚本作为组件
        characters = new CharacterModel[6];
        LoadResources();                                     //加载资源
        boat.DisableClick();
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].DisableClick();
        }
        judge = new Judge(fromLand, toLand, boat);
    }

    public void LoadResources()              //游戏开始前的布景：创建水，陆地，角色，船
    {
        GameObject water = Instantiate(Resources.Load("Prefabs/Water", typeof(GameObject))) as GameObject;
        water.transform.position = waterPos;
        water.name = "water";

        fromLand = new LandModel("from");
        toLand = new LandModel("to");
        boat = new BoatModel();

        for (int i = 0; i < 3; i++)
        {
            CharacterModel newOne = new CharacterModel("priest");
            newOne.SetName("priest" + i);
            newOne.SetPosition(fromLand.GetEmptyPosition());
            newOne.GetOnLand(fromLand);
            fromLand.GetOnLand(newOne);

            characters[i] = newOne;
        }

        for (int i = 0; i < 3; i++)
        {
            CharacterModel newOne = new CharacterModel("devil");
            newOne.SetName("devil" + i);
            newOne.SetPosition(fromLand.GetEmptyPosition());
            newOne.GetOnLand(fromLand);
            fromLand.GetOnLand(newOne);

            characters[i + 3] = newOne;
        }
    }

    public void MoveBoat()                  //移动船
    {
        if (boat.IsEmpty())
            return;
        //boat.Move();
        actionManager.moveBoat(boat.getGameObject(), boat.BoatMoveToPosition(), boat.speed);
        userGUI.status = isOver();
        if (isOver() != 0)
        {
            boat.DisableClick();
            for (int i = 0; i < characters.Length; i++)
            {
                characters[i].DisableClick();
            }
        }
    }

    private int isOver()
    {
        return judge.isOver();
    }

    public void MoveCharacter(CharacterModel character)    //移动角色
    {
        if (character.IsOnBoat())
        {
            LandModel whichLand;
            if (boat.GetToOrFrom() == -1)
            { // to->-1; from->1
                whichLand = toLand;
            }
            else
            {
                whichLand = fromLand;
            }

            boat.GetOffBoat(character.GetName());
            Vector3 dest = whichLand.GetEmptyPosition();                                         //动作分离版本改变
            Vector3 middle = new Vector3(character.getGameObject().transform.position.x, dest.y, dest.z);  //动作分离版本改变
            actionManager.moveRole(character.getGameObject(), middle, dest, boat.speed);  //动作分离版本改变
            character.GetOnLand(whichLand);
            whichLand.GetOnLand(character);

            if (boat.IsEmpty() && boat.GetToOrFrom() == -1)
                actionManager.moveBoat(boat.getGameObject(), boat.BoatMoveToPosition(), boat.speed);

        }
        else
        {                                   
            LandModel whichLand = character.GetLandModel(); // character在land上

            if (boat.GetEmptyIndex() == -1)
            {       // 船满了
                return;
            }

            if (whichLand.GetToOrFrom() != boat.GetToOrFrom())   // 船在另一边
                return;

            whichLand.GetOffLand(character.GetName());
            Vector3 dest = boat.GetEmptyPosition();                                        //动作分离版本改变
            Vector3 middle = new Vector3(dest.x, character.getGameObject().transform.position.y, dest.z);  //动作分离版本改变
            actionManager.moveRole(character.getGameObject(), middle, dest, boat.speed);  //动作分离版本改变
            character.GetOnBoat(boat);
            boat.GetOnBoat(character);
        }
        userGUI.status = isOver();
        if (isOver() != 0)
        {
            boat.DisableClick();
            for (int i = 0; i < characters.Length; i++)
            {
                characters[i].DisableClick();
            }
        }
    }

    public void Restart()
    {
        boat.Reset(); 
        if (boat.GetToOrFrom() == -1)
        {
            actionManager.moveBoat(boat.getGameObject(), boat.BoatMoveToPosition(), boat.speed);
        }
        fromLand.Reset();
        toLand.Reset();
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].Reset();
        }
    }
}
