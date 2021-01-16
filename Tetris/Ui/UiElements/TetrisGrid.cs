using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Tetris.Core;
using Tetris.Ui.Models;

namespace Tetris.Ui.UiElements
{
    public class TetrisGrid : Grid
    {
        public event Action FigureFallen = delegate { };

        public event Action FigureAdded = delegate { };

        public TetrisFigure FallingFigure => fallingFigure;

        public IReadOnlyDictionary<TetrisFigureMove, Predicate<TetrisFigure>> PossibleFigureMoves
        {
            get => new Dictionary<TetrisFigureMove, Predicate<TetrisFigure>>
            {
                { TetrisFigureMove.Down, CanFigureMoveDown },
                { TetrisFigureMove.Left, CanFigureMoveLeft },
                { TetrisFigureMove.Right, CanFigureMoveRight },
            };
        }

        public int RowCount { get; }

        public int ColumnCount { get; }
        
        public int CeilSize { get; }

        private ObservableCollection<TetrisFigure> fallenFigures = new ObservableCollection<TetrisFigure>();

        private TetrisFigure fallingFigure;

        public TetrisGrid(int rowCount, int columnCount, int ceilSize)
        {
            RowCount = rowCount;
            ColumnCount = columnCount;
            CeilSize = ceilSize;
            MaxWidth = columnCount * ceilSize;
            MaxHeight = rowCount * ceilSize;

            InitializeColumns();
            InitializeRows();
            AddCeils(Ceil.CreateMany(rowCount, columnCount));
        }

        public void AddFigure(TetrisFigure figure)
        {
            fallingFigure = figure;
            AddCeils(figure.Ceils, 10);
            FigureAdded.Invoke();
        }

        public void ToNextTick()
        {
            if (fallingFigure == null)
                throw new Exception("Add figure.");

            MoveFallingFigure();
            RedrawFigures();
        }

        public void RedrawFigures()
        {
            var figures = new List<TetrisFigure>(fallenFigures);
            if (fallingFigure != null)
                figures.Add(fallingFigure);

            foreach (var figure in figures)
            {
                foreach (var ceil in figure.Ceils)
                {
                    SetColumn(ceil, ceil.Column);
                    SetRow(ceil, ceil.Row);
                }
            }
        }

        public void ClearCeils()
        {
            var ceils = fallenFigures.SelectMany(f => f.Ceils).ToList();
            if (fallingFigure != null)
                ceils.AddRange(fallingFigure.Ceils);

            ceils.ForEach(c => Children.Remove(c));
        }

        public bool CanFigureMoveRight(TetrisFigure figure)
        {
            if (figure.Ceils.Any(ceil => ceil.Column == ColumnCount - 1))
                return false;

            var ceils = fallenFigures.SelectMany(f => f.Ceils).ToList();
            foreach (var ceil in figure.Ceils)
            {
                if (ceils.Any(c => c.IsOnSameRow(ceil) &&
                                   c.IsInPrevColumn(ceil)))
                    return false;
            }

            return true;
        }

        public bool CanFigureMoveLeft(TetrisFigure figure)
        {
            if (figure.Ceils.Any(ceil => ceil.Column == 0))
                return false;

            var ceils = fallenFigures.SelectMany(f => f.Ceils).ToList();
            foreach (var ceil in figure.Ceils)
            {
                if (ceils.Any(c => c.IsOnSameRow(ceil) &&
                                   c.IsInNextColumn(ceil)))
                    return false;
            }

            return true;
        }

        public bool CanFigureMoveDown(TetrisFigure figure)
        {
            if (figure.Ceils.Where(ceil => ceil.Row == RowCount - 1).Any())
                return false;

            var ceils = fallenFigures.SelectMany(f => f.Ceils).ToList();
            foreach (var ceil in figure.Ceils)
            {
                if (ceils.Any(c => c.IsInSameColumn(ceil) &&
                                   c.IsOnPrevRow(ceil)))
                    return false;
            }

            return true;
        }

        public bool CanFigureMoveTo(TetrisFigure figure, TetrisFigureMove move)
        {
            return PossibleFigureMoves[move].Invoke(figure);
        }

        public void TryMoveFigureTo(TetrisFigure figure, TetrisFigureMove move)
        {
            if (CanFigureMoveTo(figure, move))
                figure.MoveTo(move);
        }

        private void InitializeColumns()
        {
            for (int i = 0; i < ColumnCount; i++)
            {
                var column = new ColumnDefinition { Width = new GridLength(CeilSize) };
                ColumnDefinitions.Add(column);
            }
        }

        private void InitializeRows()
        {
            for (int i = 0; i < RowCount; i++)
            {
                var definition = new RowDefinition { Height = new GridLength(CeilSize) };
                RowDefinitions.Add(definition);
            }
        }

        private void AddCeils(IEnumerable<Ceil> ceils, int zIndex = 1)
        {
            foreach (var ceil in ceils)
            {
                SetColumn(ceil, ceil.Column);
                SetRow(ceil, ceil.Row);
                SetZIndex(ceil, zIndex);
                Children.Add(ceil);
            }
        }

        private void MoveFallingFigure()
        {
            if (CanFigureMoveDown(fallingFigure))
            {
                fallingFigure.MoveDown();
            }
            else
            {
                fallenFigures.Add(fallingFigure);
                FigureFallen.Invoke();
            }
        }
    }
}
