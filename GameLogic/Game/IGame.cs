using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattleServer.GameLogic
{
    interface IGame
    {
        int GameId { get; }
        Player RoomOwner { get; }
        event EventHandler<GameEventArgs> GameEvent;
    }
}
