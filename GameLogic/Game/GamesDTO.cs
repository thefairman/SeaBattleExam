using SeaBattleGame;
using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattleServer.GameLogic
{
    public enum GameListing { SeaBattle }
    public class GameAnswer
    {
        public object Object { get; set; }
        public bool Error { get; set; }
        public string Message { get; set; }
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
        public GameAnswer()
        {

        }
    }

    public enum SeaBattleActionType { PlayerJoins, FillField, Move, Info }
    public class SeaBattleGameData
    {
        public int GameId { get; set; }
        public GameStatus GameStatus { get; set; }
        public GameResult GameResult { get; set; }
        public CellStatus[,] MyField { get; set; }
        public CellStatus[,] EnemyField { get; set; }
        public SeaBattleActionType? ActionType { get; set; }
        public object ActionResult { get; set; }
        public Player CurrentWalker { get; set; }
        public Player LastWalker { get; set; }
        public DateTime? LastMoveTime { get; set; }
        public int MillisecondsForNextMove { get; set; }
    }
}
