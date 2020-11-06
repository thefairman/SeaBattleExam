using SeaBattleGame;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Timers;

[assembly: InternalsVisibleTo("GameLogic.Test")]

namespace SeaBattleServer.GameLogic
 {
    enum SeaBattleGameStatus { WaitToFillFields, WaitToMove }
    public class SeaBattleGame : GameBase
    {
        class LastShootedPlayer
        {
            public Player Player { get; set; }
            public DateTime Time { get; set; } = DateTime.Now;
        }
        int _moveWaitMsTime = 120000;
        Dictionary<Player, SeaBattleField> _fields = new Dictionary<Player, SeaBattleField>();
        LastShootedPlayer _lastShootedPlayer;
        Timer IDLEChecker;
        Random _random = new Random();
        SeaBattleGameStatus _seaBattleGameStatus = SeaBattleGameStatus.WaitToFillFields;
        SeaBattleGameData lastMessage;
        public SeaBattleGame(int gameId, Player player)
            : base(gameId, player, GameListing.SeaBattle, 2, 2)
        {
            _fields.Add(player, new SeaBattleField());
            IDLEChecker = new Timer(_moveWaitMsTime);
            IDLEChecker.Elapsed += IDLEChecker_Elapsed;
            //IDLEChecker.
        }

        object _moveLock = new object();
        private void IDLEChecker_Elapsed(object sender, ElapsedEventArgs e)
        {
            ((Timer)sender).Stop();
            lock (_moveLock)
            {
                if (_lastShootedPlayer?.Time >= DateTime.Now.AddMilliseconds(_moveWaitMsTime))
                    ;//todo: end game
            }
        }

        protected override void OnJoin(Player player)
        {
            _fields.Add(players[1], new SeaBattleField());
            StartGame(RoomOwner);
        }

        protected override void OnStartGame()
        {
            _lastShootedPlayer = new LastShootedPlayer { Time = DateTime.Now };
            IDLEChecker.Start();
            foreach (var player in players)
            {
                SendEvent(player, GetGameData(player, SeaBattleActionType.PlayerJoins));
            }
        }

        public GameAnswer FillField(Player player, int[,] field)
        {
            lock (_moveLock)
            {
                if (!players.Contains(player))
                    return new GameAnswer("Вас нет в этой игре!");
                if (_fields[player]?.Field != null)
                    return new GameAnswer("Вы уже сделали расстановку кораблей!");
                var setFiledResult = _fields[player].SetField(field);
                if (setFiledResult.Error)
                    return setFiledResult;
                return GetGameData(player);
            }
        }

        Player GetOtherPlayer(Player player)
        {
            if (player == null || players.Count <= 1)
                return null;
            return players.FindLast(x => x.Id != player.Id);
        }

        GameAnswer GetGameData(Player player, SeaBattleActionType? actionType = null, object actionResult = null)
        {
            if (!players.Contains(player))
                return new GameAnswer("Вас нет в этой игре!");
            var myFiled = _fields[player].Field;
            CellStatus[,] enemyField = null;
            var otherPlayer = GetOtherPlayer(player);
            if (otherPlayer != null)
                enemyField = _fields[otherPlayer].GetFieldForEnemy();
            lastMessage = new SeaBattleGameData()
            {
                GameId = GameId,
                ActionResult = actionResult,
                EnemyField = enemyField,
                MyField = myFiled,
                GameResult = GameResult,
                GameStatus = GameStatus,
                CurrentWalker = GetOtherPlayer(_lastShootedPlayer?.Player),
                LastMoveTime = _lastShootedPlayer?.Time,
                MillisecondsForNextMove = _moveWaitMsTime,
                LastWalker = _lastShootedPlayer?.Player,
                ActionType = actionType
            };
            return new GameAnswer(lastMessage);
        }
    }
}
