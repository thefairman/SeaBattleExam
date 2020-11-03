using SeaBattleGame;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Timers;

[assembly: InternalsVisibleTo("GameLogic.Test")]

namespace SeaBattleServer.GameLogic
{
    class SeaBattleField
    {
        public CellStatus[,] Field { get; private set; } = null;
        GameAnswer ValidateFiled(int[,] field)
        {
            Dictionary<int, int> ships = new Dictionary<int, int>();
            for (int i = 0; i < field.GetLength(0); i++)
            {
                for (int j = 0; j < field.GetLength(1); j++)
                {
                    if (field[i, j] == 1)
                    {
                        var shipRank = MarkFindedShip(field, i, j);
                        if (CheckCollisions(field, i, j))
                            return new GameAnswer("Есть коллизия кораблей!", true);
                        if (ships.ContainsKey(shipRank))
                            ships[shipRank]++;
                        else
                            ships.Add(shipRank, 1);
                    }
                }
            }

            bool error = false;
            foreach (var item in ships)
            {
                switch (item.Key)
                {
                    case 1:
                        if (item.Value != 4)
                            error = true;
                        break;
                    case 2:
                        if (item.Value != 3)
                            error = true;
                        break;
                    case 3:
                        if (item.Value != 2)
                            error = true;
                        break;
                    case 4:
                        if (item.Value != 1)
                            error = true;
                        break;
                    default:
                        return new GameAnswer("Есть корабль с некоректным количеством палуб!", true);
                }
                if (error)
                    return new GameAnswer("Не корректное количество кораблей!", true);
            }
            return new GameAnswer("ok", false);
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
                    for (int k = j + 1; k < field.GetLength(1) && field[i, k] == 1; k++)
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

        public GameAnswer SetField(int[,] field)
        {
            if (Field != null)
                return new GameAnswer("Поле уже установленно!", true);
            if (field.GetLongLength(0) != 10 || field.GetLongLength(1) != 10)
                return new GameAnswer("Не корректный размер поля, должен быть 10х10!", true);
            int[,] _field = new int[10, 10];
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    _field[i, j] = field[i, j];
                }
            }
            var validateResult = ValidateFiled(_field);
            if (validateResult.Error)
                return validateResult;
            Field = new CellStatus[10, 10];
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (field[i, j] == 1)
                        Field[i, j] = CellStatus.Ship;
                    else
                        Field[i, j] = CellStatus.Empty;
                }
            }
            return validateResult;
        }
    }

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
