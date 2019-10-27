using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { START, RUNNING, OVER, PAUSE }

public interface IUserAction
{
    void MovePlayer(float translationX, float translationZ);
    void Restart();
    GameState GetGameState();
    int GetScore();
    void SetGameState(GameState gameState);
    void GameStart();
}