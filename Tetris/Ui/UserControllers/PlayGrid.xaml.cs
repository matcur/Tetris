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
    /// Interaction logic for PlayGrid.xaml
    /// </summary>
    public partial class PlayGrid : UserControl
    {
        public const int RowCount = 20;

        public const int ColumnCount = 10;

        public const int CeilSize = 25;

        public const int ThicknessBetweenWidth = 1;

        public event Action<TetrisFigure> NextFigureChanged = delegate { };

        public TetrisFigure NextFigure
        {
            get => nextFigure;
            set
            {
                nextFigure = value;
                NextFigureChanged.Invoke(value);
            }
        }

        public TetrisGrid TetrisGrid { get; } = new TetrisGrid(RowCount, ColumnCount, CeilSize);

        private TetrisFigure nextFigure;

        public PlayGrid()
        {
            InitializeComponent();
            AddChild(TetrisGrid);

            TetrisGrid.FigureFallen += delegate
            {
                Dispatcher.Invoke(() => TetrisGrid.AddFigure(NextFigure));
            };
            TetrisGrid.FigureAdded += delegate
            {
                NextFigure = TetrisFigure.CreateRandom();
            };
        }

        public void Start()
        {
            var timer = new Timer(150);
            timer.Elapsed += delegate
            {
                Dispatcher.Invoke(() => TetrisGrid.ToNextTick());
            };
            timer.Enabled = true;

            Dispatcher.Invoke(() => TetrisGrid.AddFigure(NextFigure));
        }
    }
}
