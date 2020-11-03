using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattleServer.GameLogic
{
    interface IGame
    {
        public int GameId { get; }
        public Player RoomOwner { get; }

        bool JoinToGame(Player player);
    }
}
