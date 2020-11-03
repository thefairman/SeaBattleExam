using System;
using System.Collections.Generic;
using System.Text;

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
    public class GameEventArgs : EventArgs
    {
        public string Winner { get; set; }
    }

    public abstract class GameBase: IGame
    {
        public int GameId { get; }
        List<Player> players = new List<Player>();
        protected int _minPlayers, _maxPlayers;
        public GameStatus GameStatus { get; protected set; } = GameStatus.WaitForPlayers;

        public Player RoomOwner => players[0];

        public event EventHandler<GameEventArgs> GameDone;
        public GameBase(int gameId, Player player, int minPlayers, int maxPlayers)
        {
            if (player == null)
                throw new ArgumentException();
            this.GameId = gameId;
            players.Add(player);
            _minPlayers = minPlayers;
            _maxPlayers = maxPlayers;
        }
        object joinLocker = new object();
        virtual public bool JoinToGame(Player player)
        {
            lock (joinLocker)
            {
                if (players.Contains(player))
                    return false;
                if (GameStatus != GameStatus.WaitForPlayers)
                    return false;
                if (players.Count >= _maxPlayers)
                    return false;
                players.Add(player);
                return true;
            }
        }
    }
}
