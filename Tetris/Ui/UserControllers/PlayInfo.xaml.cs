using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tetris.Ui.Models;
using Tetris.Ui.UiElements;

namespace Tetris.Ui.UserControllers
{
    /// <summary>
    /// Interaction logic for PlayInfo.xaml
    /// </summary>
    public partial class PlayInfo : UserControl
    {
        public TetrisFigure NextFigure
        {
            set
            {
                var nextFigure = value.Clone();
                nextFigure.MoveLeft();
                nextFigure.MoveLeft();
                nextFigure.MoveLeft();
                tetrisGrid.Clear();
                tetrisGrid.AddFigure(nextFigure);
                tetrisGrid.RedrawFigures();
            }
        }

        public readonly TetrisGrid tetrisGrid = new TetrisGrid(4, 4, PlayGrid.CeilSize);

        public PlayInfo()
        {
            InitializeComponent();
            AddChild(tetrisGrid);
        }
    }
}
