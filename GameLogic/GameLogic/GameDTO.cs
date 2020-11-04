using SeaBattleGame;
using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattleServer.GameLogic
{
    public class GameAnswer
    {
        public object Object { get; }
        public bool Error { get; }
        public string Message { get; }
        public GameAnswer(object @object, string message = "")
        {
            Object = @object;
            Error = false;
            Message = message;
        }
        public GameAnswer(string message, bool error = true)
        {
            Message = message;
            Error = error;
        }
    }

    public class ShootAnswer
    {
        public ShootStatus ShootStatus { get; }
        public CellStatus[,] EnemyField { get; }
        public ShootAnswer(ShootStatus shootStatus, CellStatus[,] enemyField)
        {
            ShootStatus = shootStatus;
            EnemyField = enemyField;
        }
    }
}
