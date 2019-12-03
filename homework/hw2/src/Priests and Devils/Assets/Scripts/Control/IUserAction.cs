public interface IUserAction
{
    void MoveBoat();                                   //移动船
    void Restart();                                    //重新开始
    void MoveCharacter(CharacterModel character);      //移动人物
    bool Tips();                                       //智能游戏
}