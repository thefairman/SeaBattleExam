using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Timers;

[assembly: InternalsVisibleTo("GameLogic.Test")]

namespace SeaBattleServer.GameLogic
{
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

        public SeaBattleGame(int gameId, Player player)
            : base(gameId, player, 2, 2)
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
    }
}
