using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace SeaBattleServer.RoomsLogic
{
    abstract class RoomBase: IRoom
    {
        protected int _minPlayers, _maxPlayers;
        private int _roomOwner;
        public int RoomOwner { get => _roomOwner; }

        public event Action<int> GameDone;
    }
}
