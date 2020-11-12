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
        class CurrentWalkerPlayer
        {
            public Player Player { get; set; }
            public DateTime Time { get; set; } = DateTime.Now;
        }
        int _moveWaitMsTime = 120000;
        Dictionary<Player, SeaBattleField> _fields = new Dictionary<Player, SeaBattleField>();
        CurrentWalkerPlayer _currentWalkerPlayer;
        Player _lastShootedPlayer;
        Timer IDLEChecker;
        Random _random = new Random();
        SeaBattleGameStatus _seaBattleGameStatus = SeaBattleGameStatus.WaitToFillFields;
        public SeaBattleGame(int gameId, Player player)
            : base(gameId, player, GameListing.SeaBattle, 2, 2)
        {
            _fields.Add(player, new SeaBattleField());
            IDLEChecker = new Timer(_moveWaitMsTime);
            IDLEChecker.Elapsed += IDLEChecker_Elapsed;
            //IDLEChecker.
        }

        object _moveLock = new object();

        Player LastShootedPlayer
        {
            get => _lastShootedPlayer;
            set
            {
                IDLEChecker.Stop();
                IDLEChecker.Start();
                _lastShootedPlayer = value;
            }
        }

        private void IDLEChecker_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (_moveLock)
            {
                if (_currentWalkerPlayer?.Time.AddMilliseconds(_moveWaitMsTime) < DateTime.Now)
                {
                    GameStatus = GameStatus.Done;
                    SendToEveryone(SeaBattleActionType.Info);
                }
            }
        }

        protected override void OnJoin(Player player)
        {
            _fields.Add(players[1], new SeaBattleField());
            StartGame(RoomOwner);
        }

        protected override void OnStartGame()
        {
            _currentWalkerPlayer = new CurrentWalkerPlayer { Time = DateTime.Now };
            IDLEChecker.Start();
            SendToEveryone(SeaBattleActionType.PlayerJoins);
        }

        void SendToEveryone(SeaBattleActionType? type = null, object actionResult = null)
        {
            foreach (var player in players)
            {
                SendEventAsync(player, GetGameData(player, type, actionResult));
            }
        }

        protected override void OnGameInfo(Player player)
        {
            SendEventAsync(player, GetGameData(player, SeaBattleActionType.Info));
        }

        void TryToSetStateInMove()
        {
            if (players.Count < _minPlayers)
                return;
            if (_seaBattleGameStatus == SeaBattleGameStatus.WaitToMove)
                return;
            foreach (var item in _fields)
            {
                if (item.Value.Field == null)
                    return;
            }

            _seaBattleGameStatus = SeaBattleGameStatus.WaitToMove;
            _currentWalkerPlayer = new CurrentWalkerPlayer
            {
                Player = players[_random.Next(players.Count)],
                Time = DateTime.Now
            };
        }

        public void FillField(Player player, int[,] field)
        {
            lock (_moveLock)
            {
                if (!CheckPlayerInThisGame(player))
                    return;
                if (_fields[player]?.Field != null)
                {
                    SendEventAsync(player, new GameAnswer("Вы уже сделали расстановку кораблей!"));
                    return;
                }
                var setFiledResult = _fields[player].SetField(field);
                if (setFiledResult.Error)
                {
                    SendEventAsync(player, setFiledResult);
                    return;
                }
                TryToSetStateInMove();
                LastShootedPlayer = player;
                SendToEveryone(SeaBattleActionType.FillField);
            }
        }

        public void Shoot(Player player, int row, int col)
        {
            lock (_moveLock)
            {
                if (!CheckPlayerInThisGame(player))
                    return;
                if (_seaBattleGameStatus == SeaBattleGameStatus.WaitToFillFields)
                {
                    SendEventAsync(player, new GameAnswer("Ожидание пока все игроки сделают расстановку кораблей!"));
                    return;
                }
                if (GameStatus != GameStatus.InGame)
                {
                    SendEventAsync(player, new GameAnswer("Игра не в процессе!"));
                    return;
                }
                if (_currentWalkerPlayer?.Player != player)
                {
                    SendEventAsync(player, new GameAnswer("Сейчас не ваш ход!"));
                    return;
                }

                var shootResult = _fields[player].Shoot(row, col);
                if (shootResult.Error)
                {
                    SendEventAsync(player, shootResult);
                    return;
                }

                LastShootedPlayer = player;
                switch ((ShootStatus)shootResult.Object)
                {
                    case ShootStatus.Miss:
                        _currentWalkerPlayer = new CurrentWalkerPlayer { Player = GetOtherPlayer(player), Time = DateTime.Now };
                        break;
                    case ShootStatus.Hit:
                    case ShootStatus.Kill:
                        _currentWalkerPlayer = new CurrentWalkerPlayer { Player = player, Time = DateTime.Now };
                        break;
                    case ShootStatus.Win:
                        GameStatus = GameStatus.Done;
                        break;
                }
                SendToEveryone(SeaBattleActionType.Move, shootResult.Object);
            }
        }

        Player GetOtherPlayer(Player player)
        {
            if (player == null || players.Count <= 1)
                return null;
            return players.FindLast(x => x.Id != player.Id);
        }

        GameResult GetGameResult(Player player)
        {
            if (GameStatus != GameStatus.Done)
                return GameResult.None;
            if (LastShootedPlayer == player)
                return GameResult.Win;
            if (LastShootedPlayer == null)
                return GameResult.Forward;
            return GameResult.Lose;
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
            return new GameAnswer(new SeaBattleGameData()
            {
                GameId = GameId,
                ActionResult = actionResult,
                EnemyField = enemyField,
                MyField = myFiled,
                GameResult = GetGameResult(player),
                GameStatus = GameStatus,
                CurrentWalker = _currentWalkerPlayer?.Player,
                LastMoveTime = _currentWalkerPlayer?.Time,
                MillisecondsForNextMove = _moveWaitMsTime,
                LastWalker = LastShootedPlayer,
                ActionType = actionType
            });
        }
    }
}
