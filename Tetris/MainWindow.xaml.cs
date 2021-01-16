using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using Tetris.Core;
using Tetris.Ui.Models;
using Tetris.Ui.UiElements;

namespace Tetris
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<Key, TetrisFigureMove> figureMoves = new Dictionary<Key, TetrisFigureMove>
        {
            { Key.Down, TetrisFigureMove.Down },
            { Key.Left, TetrisFigureMove.Left },
            { Key.Right, TetrisFigureMove.Right },
        };

        private TetrisFigure firstFigure = TetrisFigure.CreateStraight();

        public MainWindow()
        {
            InitializeComponent();
            playGrid.NextFigure = firstFigure;
            playInfo.NextFigure = firstFigure;

            playGrid.NextFigureChanged += nextFigure => playInfo.NextFigure = nextFigure;
        }

        private void MoveFallingFigure(object sender, KeyEventArgs e)
        {
            var key = e.Key;
            if (figureMoves.ContainsKey(key))
            {
                var tetrisGrid = playGrid.TetrisGrid;
                var fallinFigure = tetrisGrid.FallingFigure;

                tetrisGrid.TryMoveFigureTo(fallinFigure, figureMoves[key]);
            }
        }

        private void StartGame(object sender, RoutedEventArgs e)
        {
            playGrid.Start();
        }

        private void ClearPlayGrid(object sender, RoutedEventArgs e)
        {
            playGrid.Refresh();
            playGrid.Start();
        }
    }
}
