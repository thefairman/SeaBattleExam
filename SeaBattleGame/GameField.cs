using System;
using System.Diagnostics;

namespace SeaBattleGame
{
    public enum CellStatus { Empty, Ship, Strelano, Popal, Killed };

    public class GameField
    {
        Random rand = new Random();

        private CellStatus[,] _field = new CellStatus[10, 10];

        public CellStatus this[int x, int y]
        {
            get => _field[x, y];
        }

        public GameField()
        {
            Rasstanovka();
        }

        void Rasstanovka()
        {
            int x, y, r, flag;

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    _field[i, j] = CellStatus.Empty;
                }
            }
            for (int i = 0; i < 4; i++)
            {
                do
                {
                    x = rand.Next(10);
                    y = rand.Next(10);
                    flag = 0;
                    for (int a = -1; a <= 1; a++)
                    {
                        for (int b = -1; b <= 1; b++)
                        {
                            if (x + a >= 0 && x + a < 10 &&
                                y + b >= 0 && y + b < 10 &&
                                _field[x + a, y + b] == CellStatus.Ship)
                            {
                                flag = 1;
                            }
                        }
                    }
                } while (flag == 1);
                _field[x, y] = CellStatus.Ship;
            }
            for (int i = 0; i < 3; i++)
            {
                do
                {
                    r = rand.Next(2);
                    x = rand.Next(9 + r);
                    y = rand.Next(9 + 1 - r);
                    flag = 0;
                    for (int a = -1; a <= (2 - r); a++)
                    {
                        for (int b = -1; b <= (1 + r); b++)
                        {
                            if (x + a >= 0 && x + a < 10 &&
                                y + b >= 0 && y + b < 10 &&
                                _field[x + a, y + b] == CellStatus.Ship)
                            {
                                flag = 1;
                            }
                        }
                    }
                } while (flag == 1);
                _field[x, y] = CellStatus.Ship;
                _field[x + 1 - r, y + r] = CellStatus.Ship;

            }
            for (int i = 0; i < 2; i++)
            {
                do
                {
                    r = rand.Next(2);
                    x = rand.Next(8 + 2 * r);
                    y = rand.Next(8 + 2 * (1 - r));
                    flag = 0;
                    for (int a = -1; a <= (1 + 2 * (1 - r)); a++)
                    {
                        for (int b = -1; b <= (1 + 2 * r); b++)
                        {
                            if (x + a >= 0 && x + a < 10 &&
                                y + b >= 0 && y + b < 10 &&
                                _field[x + a, y + b] == CellStatus.Ship)
                            {
                                flag = 1;
                            }
                        }
                    }
                } while (flag == 1);
                _field[x, y] = CellStatus.Ship;
                _field[x + 1 - r, y + r] = CellStatus.Ship;
                _field[x + 2 * (1 - r), y + 2 * r] = CellStatus.Ship;
            }

            do
            {
                r = rand.Next(2);
                x = rand.Next(7 + 3 * r);
                y = rand.Next(7 + 3 * (1 - r));
                flag = 0;
                for (int a = -1; a <= (1 + 3 * (1 - r)); a++)
                {
                    for (int b = -1; b <= (1 + 3 * r); b++)
                    {
                        if (x + a >= 0 && x + a < 10 &&
                            y + b >= 0 && y + b < 10 &&
                            _field[x + a, y + b] == CellStatus.Ship)
                        {
                            flag = 1;
                        }
                    }
                }
            } while (flag == 1);
            _field[x, y] = CellStatus.Ship;
            _field[x + 1 - r, y + r] = CellStatus.Ship;
            _field[x + 2 * (1 - r), y + 2 * r] = CellStatus.Ship;
            _field[x + 3 * (1 - r), y + 3 * r] = CellStatus.Ship;
        }

        public bool Babax(int i, int j)
        {

            switch (_field[i, j])
            {
                case CellStatus.Empty: //Pusto
                    _field[i, j] = CellStatus.Strelano;
                    Debug.WriteLine("Промазал!!!");
                    return true;

                case CellStatus.Ship: //Ship

                    _field[i, j] = CellStatus.Popal;

                    if (IfKill(i, j))
                    {
                        Debug.WriteLine("Убил!!!");
                        PaintKilled(i, j);

                    }
                    else
                    {
                        Debug.WriteLine("Ранен!!!");
                    }

                    return true;

                case CellStatus.Strelano: //Strelano
                    Debug.WriteLine("Записывать надо!!!");
                    return false;
                case CellStatus.Popal: //Popal
                    Debug.WriteLine("Записывать надо!!!\n");
                    return false;

                case CellStatus.Killed: //Killed

                    Debug.WriteLine("Записывать надо!!!\n");
                    return false;
            }
            return false;
        }

        bool IfKill(int i, int j)
        {

            int flag = 0;

            for (int n = i - 1; n <= i + 1; n++)
                for (int k = j - 1; k <= j + 1; k++)
                    if (n < 10 && k < 10 && n >= 0 && k >= 0 && _field[n, k] == CellStatus.Ship)
                    {
                        return false;//ОДНОЗНАЧНО РАНИЛ
                    }

            //Проверка не убит ли однопалубный, в принципе не нужна, но оставлена				
            for (int n = i - 1; n <= i + 1; n++)
                for (int k = j - 1; k <= j + 1; k++)
                    if (n < 10 && k < 10 && n >= 0 && k >= 0 && _field[n, k] == CellStatus.Popal)
                    {
                        flag++;
                    }
            if (flag == 1)
            {
                Debug.WriteLine("УБИТ ОДНОПАЛУБНЫЙ");
                return true;
            }
            //корабль имеет более одной палубы

            //Проверяем две палубы влево
            if (i > 1)
                if (_field[i - 1, j] == CellStatus.Ship || _field[i - 1, j] == CellStatus.Popal)
                    if (_field[i - 2, j] == CellStatus.Ship)
                        return false;

            //Проверяем третью палубу
            if (i > 2)
                if (_field[i - 2, j] == CellStatus.Popal)
                    if (_field[i - 3, j] == CellStatus.Ship) return false;


            //Проверяем две палубы вправо

            if (i < 8)
                if (_field[i + 1, j] == CellStatus.Ship || _field[i + 1, j] == CellStatus.Popal)
                    if (_field[i + 2, j] == CellStatus.Ship)
                        return false;
            //Проверяем третью палубу
            if (i < 7)
                if (_field[i + 2, j] == CellStatus.Popal)
                    if (_field[i + 3, j] == CellStatus.Ship)
                        return false;


            //Проверяем две палубы вверх
            if (j > 1)
                if (_field[i, j - 1] == CellStatus.Ship || _field[i, j - 1] == CellStatus.Popal)
                    if (_field[i, j - 2] == CellStatus.Ship)
                        return false;
            //Проверяем третью палубу
            if (j > 2)
                if (_field[i, j - 2] == CellStatus.Popal)
                    if (_field[i, j - 3] == CellStatus.Ship)
                        return false;

            //Проверяем две палубы вниз
            if (j < 8)
                if (_field[i, j + 1] == CellStatus.Ship || _field[i, j + 1] == CellStatus.Popal)
                    if (_field[i, j + 2] == CellStatus.Ship)
                        return false;
            //Проверяем третью палубу
            if (j < 7)
                if (_field[i, j + 2] == CellStatus.Popal)
                    if (_field[i, j + 3] == CellStatus.Ship)
                        return false;

            return true;
        }

        void OnPaintKilled(int i, int j)
        {
            if (_field[i, j] == CellStatus.Popal)
            {
                _field[i, j] = CellStatus.Killed;
            }

            for (int n = i - 1; n <= i + 1; n++)
                for (int k = j - 1; k <= j + 1; k++)
                {
                    if (n < 10 && k < 10 && n >= 0 && k >= 0 && _field[n, k] == CellStatus.Empty)
                        _field[n, k] = CellStatus.Strelano;

                    if (n < 10 && k < 10 && n >= 0 && k >= 0 && _field[n, k] == CellStatus.Popal)
                        _field[n, k] = CellStatus.Killed;

                }
        }

        void PaintKilled(int i, int j)
        {
            OnPaintKilled(i, j);

            if (i > 1 && (_field[i - 1, j] == CellStatus.Popal || _field[i - 1, j] == CellStatus.Killed))
            {
                OnPaintKilled(i - 1, j);

                if (i > 2 && (_field[i - 2, j] == CellStatus.Popal || _field[i - 2, j] == CellStatus.Killed))
                {
                    OnPaintKilled(i - 2, j);
                    if (i > 3 && (_field[i - 3, j] == CellStatus.Popal || _field[i - 3, j] == CellStatus.Killed))
                        OnPaintKilled(i - 3, j);
                }
            }

            if (i < 8 && (_field[i + 1, j] == CellStatus.Popal || _field[i + 1, j] == CellStatus.Killed))
            {
                OnPaintKilled(i + 1, j);
                if (i < 7 && (_field[i + 2, j] == CellStatus.Popal || _field[i + 2, j] == CellStatus.Killed))
                {
                    OnPaintKilled(i + 2, j);
                    if (i < 6 && (_field[i + 3, j] == CellStatus.Popal || _field[i + 3, j] == CellStatus.Killed))
                        OnPaintKilled(i + 3, j);
                }
            }

            if (j > 1 && (_field[i, j - 1] == CellStatus.Popal || _field[i, j - 1] == CellStatus.Killed))
            {
                OnPaintKilled(i, j - 1);
                if (j > 2 && (_field[i, j - 2] == CellStatus.Popal || _field[i, j - 2] == CellStatus.Killed))
                {
                    OnPaintKilled(i, j - 2);
                    if (j > 3 && (_field[i, j - 3] == CellStatus.Popal || _field[i, j - 3] == CellStatus.Killed))
                        OnPaintKilled(i, j - 3);
                }
            }

            if (j < 8 && (_field[i, j + 1] == CellStatus.Popal || _field[i, j + 1] == CellStatus.Killed))
            {
                OnPaintKilled(i, j + 1);
                if (j < 7 && (_field[i, j + 2] == CellStatus.Popal || _field[i, j + 2] == CellStatus.Killed))
                {
                    OnPaintKilled(i, j + 2);
                    if (j < 6 && (_field[i, j + 3] == CellStatus.Popal || _field[i, j + 3] == CellStatus.Killed))
                        OnPaintKilled(i, j + 3);
                }
            }
        }
    }
}
