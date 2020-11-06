using SeaBattleGame;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("GameLogic.Test")]

namespace SeaBattleServer.GameLogic
{
    public enum ShootStatus { Miss, Hit, Kill, Win }
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
                            return new GameAnswer("Есть коллизия кораблей!");
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
                        return new GameAnswer("Есть корабль с некоректным количеством палуб!");
                }
                if (error)
                    return new GameAnswer("Не корректное количество кораблей!");
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

        /// <summary>
        /// Set a field
        /// </summary>
        /// <param name="field">Array of int[10,10] where 1 is ship and 0 is empty</param>
        /// <returns>Return ok in Message or message of error if Error is true</returns>
        public GameAnswer SetField(int[,] field)
        {
            if (Field != null)
                return new GameAnswer("Поле уже установленно!");
            if (field.GetLongLength(0) != 10 || field.GetLongLength(1) != 10)
                return new GameAnswer("Не корректный размер поля, должен быть 10х10!");
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

        /// <summary>
        /// Make a shoot to field
        /// </summary>
        /// <param name="row">Num of row filed</param>
        /// <param name="col">Num of col field</param>
        /// <returns>Return <see cref="ShootStatus"/> in Object or Message of error if Error is true
        /// </returns>
        public GameAnswer Shoot(int row, int col)
        {
            if (Field == null)
                return new GameAnswer("Игровое поле не установленно!");
            if (row < 0 || row >= 10)
                return new GameAnswer("Не корректная строка!");
            if (col < 0 || col >= 10)
                return new GameAnswer("Не корректная колонка!");
            if (Field[row, col] != CellStatus.Empty && Field[row, col] != CellStatus.Ship)
                return new GameAnswer("Данная ячейка уже отсрелянна!");
            if (Field[row, col] == CellStatus.Empty)
            {
                Field[row, col] = CellStatus.Strelano;
                return new GameAnswer(ShootStatus.Miss);
            }

            Field[row, col] = CellStatus.Popal;
            if (CheckForKill(row, col))
            {
                if (ChekForWin())
                    return new GameAnswer(ShootStatus.Win);
                else
                    return new GameAnswer(ShootStatus.Kill);
            }
            else
                return new GameAnswer(ShootStatus.Hit);
        }

        bool ChekForWin()
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (Field[i, j] == CellStatus.Ship)
                        return false;
                }
            }
            return true;
        }

        public CellStatus[,] GetFieldForEnemy()
        {
            CellStatus[,] field = new CellStatus[10, 10];
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (Field[i, j] != CellStatus.Ship)
                        field[i, j] = Field[i, j];
                    else
                        field[i, j] = CellStatus.Empty;
                }
            }
            return field;
        }

        void FillKilledShip(int i, int j)
        {
            if (i < 0 || j < 0)
                return;
            if (i >= Field.GetLength(0) || j >= Field.GetLength(1))
                return;
            if (Field[i, j] == CellStatus.Empty)
            {
                Field[i, j] = CellStatus.Strelano;
                return;
            }
            if (Field[i, j] == CellStatus.Popal)
            {
                Field[i, j] = CellStatus.Killed;
                FillKilledShip(i - 1, j - 1);
                FillKilledShip(i - 1, j);
                FillKilledShip(i - 1, j + 1);
                FillKilledShip(i, j - 1);
                FillKilledShip(i, j + 1);
                FillKilledShip(i + 1, j - 1);
                FillKilledShip(i + 1, j);
                FillKilledShip(i + 1, j + 1);
            }
        }

        bool CheckForKill(int i, int j)
        {
            for (int k = i + 1; k < 10 && Field[k, j] != CellStatus.Empty; k++) // check down
                if (Field[k, j] == CellStatus.Ship)
                    return false;
            for (int k = i - 1; k >= 0 && Field[k, j] != CellStatus.Empty; k--) // check up
                if (Field[k, j] == CellStatus.Ship)
                    return false;
            for (int k = j + 1; k < 10 && Field[i, k] != CellStatus.Empty; k++) // check right
                if (Field[i, k] == CellStatus.Ship)
                    return false;
            for (int k = j - 1; k >= 0 && Field[i, k] != CellStatus.Empty; k--) // check left
                if (Field[i, k] == CellStatus.Ship)
                    return false;
            FillKilledShip(i, j);
            return true;
        }

#if DEBUG
        public void SetUnsafeMyField(int[,] field)
        {
            Field = new CellStatus[10, 10];
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    Field[i, j] = (CellStatus)field[i, j];
                }
            }
        }
#endif
    }
}
