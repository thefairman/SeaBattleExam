using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SeaBattleServer.GameLogic
{
    public class Player
    {
        public int Id { get; }
        public string Nick { get; set; }

        public override int GetHashCode()
        {
            return Id;
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as Player);
        }
        public bool Equals(Player obj)
        {
            return obj != null && obj.Id == Id;
        }
    }

    public enum GameStatus { WaitForPlayers, InGame, Done }
    public enum GameResult { Win, Lose, Forward, None }
    public class GameEventArgs : EventArgs
    {
        public GameListing Game { get; set; }
        public Player Player { get; set; }
        public GameAnswer Message { get; set; }
    }
    public abstract class GameBase : IGame
    {
        public int GameId { get; }
        protected List<Player> players = new List<Player>();
        protected int _minPlayers, _maxPlayers;
        public GameStatus GameStatus { get; protected set; } = GameStatus.WaitForPlayers;
        public GameListing CurrentGame { get; }
        public Player RoomOwner => players?[0];

        public event EventHandler<GameEventArgs> GameEvent;
        public GameBase(int gameId, Player player, GameListing currentGame, int minPlayers, int maxPlayers)
        {
            if (player == null)
                throw new ArgumentException();
            this.GameId = gameId;
            players.Add(player);
            _minPlayers = minPlayers;
            _maxPlayers = maxPlayers;
            CurrentGame = currentGame;
        }
        object _joinLock = new object();
        public void JoinToGame(Player player)
        {
            lock (_joinLock)
            {
                if (players.Contains(player))
                {
                    SendEventAsync(player, new GameAnswer("Вы уже в этой игре!"));
                    return;
                }
                if (GameStatus != GameStatus.WaitForPlayers)
                {
                    SendEventAsync(player, new GameAnswer("Данная комната не ждет подключения!"));
                    return;
                }
                if (players.Count >= _maxPlayers)
                {
                    SendEventAsync(player, new GameAnswer("Достигнуто максимальное количество игроков!"));
                    return;
                }
                players.Add(player);
            }
            OnJoin(player);
        }

        protected Task SendEventAsync(Player player, GameAnswer gameAnswer)
        {
            return Task.Run(()=>GameEvent?.Invoke(this, new GameEventArgs { Game = CurrentGame, Message = gameAnswer, Player = player }));
        }

        protected abstract void OnJoin(Player player);

        virtual public void StartGame(Player player)
        {
            if (GameStatus != GameStatus.WaitForPlayers)
            {
                SendEventAsync(player, new GameAnswer("Игра должна быть в состоянии ожидания игроков, чтобы запуститься!"));
                return;
            }
            if (player != RoomOwner)
            {
                SendEventAsync(player, new GameAnswer("Запустить игру может только владелец комнаты!"));
                return;
            }
            if (players.Count < _minPlayers)
            {
                SendEventAsync(player, new GameAnswer("Недостаточное колличество игроков!"));
                return;
            }
            GameStatus = GameStatus.InGame;
            OnStartGame();
        }

        protected bool CheckPlayerInThisGame(Player player)
        {
            if (!players.Contains(player))
            {
                SendEventAsync(player, new GameAnswer("Вас нет в этой игре!"));
                return false;
            }
            return true;
        }

        protected abstract void OnStartGame();

        public void GetGameInfo(Player player)
        {
            if (!CheckPlayerInThisGame(player))
            OnGameInfo(player);
        }

        protected abstract void OnGameInfo(Player player); 
    }
}
