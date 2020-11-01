using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using SeaBattleGame;

namespace GameFieldLibrary
{
    public partial class GameField : UserControl
    {
        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(true)]
        public bool IsMyShips { get; set; } = true;

        SeaBattleGame.GameField _gameField = new SeaBattleGame.GameField();


        public GameField()
        {
            InitializeComponent();
        }

        private void GameField_Load(object sender, EventArgs e)
        {
            
        }

        private void SeaBattleField_Paint(object sender, PaintEventArgs e)
        {
            float cellWidth = ClientSize.Width / 10.0F - 2.0F;
            float cellHeight = ClientSize.Height / 10.0F - 2.0F;

            for (int i = 0; i < 10; ++i)
            {
                for (int j = 0; j < 10; ++j)
                {
                    PaintCell(e.Graphics, _gameField[j, i],
                        new RectangleF(
                            1 + (cellWidth + 2) * j,
                            1 + (cellHeight + 2) * i,
                            cellWidth,
                            cellHeight));
                }
            }
        }

        private void PaintCell(Graphics gr, CellStatus cs, RectangleF rect)
        {
            switch (cs)
            {
                case CellStatus.Empty:
                    gr.FillRectangle(Brushes.Green, rect);
                    return;
                case CellStatus.Ship:
                    if (IsMyShips)
                        gr.FillRectangle(Brushes.Red, rect);
                    else
                        gr.FillRectangle(Brushes.Green, rect);
                    return;
                case CellStatus.Killed:
                    gr.FillRectangle(Brushes.Black, rect);
                    return;
                case CellStatus.Popal:
                    gr.FillRectangle(Brushes.Red, rect);
                    //gr.DrawRectangle(new Pen(Color.Black, 2), rect.Left+1, rect.Top+1, rect.Width-2, rect.Height-2);
                    gr.DrawLine(new Pen(Color.Black, 2), rect.Left, rect.Top, rect.Right, rect.Bottom);
                    gr.DrawLine(new Pen(Color.Black, 2), rect.Left, rect.Bottom, rect.Right, rect.Top);
                    return;
                case CellStatus.Strelano:
                    gr.FillRectangle(Brushes.Green, rect);
                    gr.FillEllipse(Brushes.Black, rect.Left + rect.Width / 2 - 3, rect.Top + rect.Height / 2 - 3, 6, 6);
                    return;
            }

        }

        private void SeaBattleField_MouseDown(object sender, MouseEventArgs e)
        {
            if (IsMyShips)
            {
                MessageBox.Show("Открытие огня по своим запрещено");
                return;
            }

            if (_gameField.Babax(e.X * 10 / ClientSize.Width, e.Y * 10 / ClientSize.Height))
            {
                Invalidate();
            }
        }
    }
}
