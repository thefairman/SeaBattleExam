using SeaBattleGame;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace SeaBattleServer.GameLogic
{
    class LastShootedPlayer
    {
        public Player Player { get; set; }
        public DateTime Time { get; set; } = DateTime.Now;
    }

    class SeaBattleField
    {
        public CellStatus[,] Field { get; private set; } = null;
        bool ValidateFiled(int[,] field)
        {
            int[,] _shipsCount = { { 1, 2, 3, 4 }, { 4, 3, 2, 1 } };
            Dictionary<int, int> ships = new Dictionary<int, int>();

            for (int i = 0; i < field.GetLength(0); i++)
            {
                for (int j = 0; j < field.GetLength(1); j++)
                {
                    if (field[i, j] == 1)
                    {
                        // check horizontal orientation of ship
                        if (i < field.GetLength(0) - 1)
                        {
                            if (field[i + 1, j] == 1)
                        }
                    }
                }
            }

        }

        int MarkFindedShip(int[,] field, int i, int j)
        {
            field[i, j] = 2;
            int shipRank = 1;
            // check vertical orientation of ship
            if (i < field.GetLength(0) - 1)
            {
                if (field[i + 1, j] == 1)
                {
                    for (int k = i + 1; k < field.GetLength(0) && field[k, j] == 1; k++)
                    {
                        field[k, j] = 2;
                        ++shipRank;
                    }
                    return shipRank;
                }
            }
            // check horizontal orientation of ship
            if (j < field.GetLength(1) - 1)
            {
                if (field[i, j + 1] == 1)
                {
                    for (int k = j + 1; k < field.GetLength(1) && field[i, j] == 1; k++)
                    {
                        field[i, k] = 2;
                        ++shipRank;
                    }
                    return shipRank;
                }
            }
            return shipRank;
        }

        bool CheckCollisions(int[,] field, int i, int j)
        {
            if (i < 0 || j < 0)
                return false;
            if (i >= field.GetLength(0) || j >= field.GetLength(1))
                return false;
            int curCell = field[i, j];
            field[i, j] = 3;
            switch (curCell)
            {
                case 0:
                case 3:
                    return false;
                case 1:
                    return true;
                case 2:
                    if (CheckCollisions(field, i - 1, j - 1)) return true;
                    if (CheckCollisions(field, i - 1, j)) return true;
                    if (CheckCollisions(field, i - 1, j + 1)) return true;
                    if (CheckCollisions(field, i, j - 1)) return true;
                    if (CheckCollisions(field, i, j + 1)) return true;
                    if (CheckCollisions(field, i + 1, j - 1)) return true;
                    if (CheckCollisions(field, i + 1, j)) return true;
                    if (CheckCollisions(field, i + 1, j + 1)) return true;
                    break;
            }
            return false;
        }
    }

    public class SeaBattleGame : GameBase
    {
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
