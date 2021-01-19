using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Tetris.Core;
using Tetris.Exceptions.Ceils;
using Tetris.Extensions;
using Tetris.Ui.Models;

namespace Tetris.Ui.UiElements
{
    public class TetrisGrid : Grid
    {
        public event Action<TetrisFigure> FigureFallen = delegate { };

        public event Action FigureAdded = delegate { };

        public event Action GameLost = delegate { };

        public IReadOnlyDictionary<TetrisFigureMove, Predicate<TetrisFigure>> PossibleFigureMoves
        {
            get => new Dictionary<TetrisFigureMove, Predicate<TetrisFigure>>
            {
                { TetrisFigureMove.Down, CanFigureMoveDown },
                { TetrisFigureMove.Left, CanFigureMoveLeft },
                { TetrisFigureMove.Right, CanFigureMoveRight },
            };
        }

        public IReadOnlyList<Ceil> AllFigureCeils => fallenFigures.SelectMany(f => f.Ceils).ToList();

        public TetrisFigure FallingFigure => fallingFigure;

        public int RowCount { get; }

        public int ColumnCount { get; }
        
        public int CeilSize { get; }

        private List<TetrisFigure> fallenFigures = new List<TetrisFigure>();

        private List<int> rows;

        private TetrisFigure fallingFigure;

        public TetrisGrid(int rowCount, int columnCount, int ceilSize)
        {
            RowCount = rowCount;
            ColumnCount = columnCount;
            CeilSize = ceilSize;
            MaxWidth = columnCount * ceilSize;
            MaxHeight = rowCount * ceilSize;
            rows = Enumerable.Range(0, rowCount).ToList();

            InitializeColumns();
            InitializeRows();
            InitializeBackground();

            FigureFallen += OnFigureFallen;
        }

        public void AddFigure(TetrisFigure figure)
        {
            fallingFigure = figure;
            AddCeils(figure.Ceils, 10);
            FigureAdded.Invoke();
        }

        public void Update()
        {
            if (fallingFigure == null)
                throw new Exception("Add figure.");

            if (CanFigureMoveDown(fallingFigure))
            {
                fallingFigure.MoveDown();
            }
            else
            {
                fallenFigures.Add(fallingFigure);
                FigureFallen.Invoke(fallingFigure);
            }
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

        public void Clear()
        {
            var ceils = fallenFigures.SelectMany(f => f.Ceils).ToList();
            if (fallingFigure != null)
                ceils.AddRange(fallingFigure.Ceils);

            ceils.ForEach(c => Children.Remove(c));
            fallenFigures.Clear();
        }

        public bool CanFigureMoveRight(TetrisFigure figure)
        {
            if (figure.Ceils.Any(ceil => ceil.Column == ColumnCount - 1))
                return false;

            foreach (var ceil in figure.Ceils)
            {
                if (AllFigureCeils.Any(c => c.IsOnSameRow(ceil) &&
                                            c.IsInPrevColumn(ceil)))
                    return false;
            }

            return true;
        }

        public bool CanFigureMoveLeft(TetrisFigure figure)
        {
            if (figure.Ceils.Any(ceil => ceil.Column == 0))
                return false;

            foreach (var ceil in figure.Ceils)
            {
                if (AllFigureCeils.Any(c => c.IsOnSameRow(ceil) &&
                                            c.IsInNextColumn(ceil)))
                    return false;
            }

            return true;
        }

        public bool CanFigureMoveDown(TetrisFigure figure)
        {
            if (figure.Ceils.Any(ceil => ceil.Row == RowCount - 1))
                return false;

            foreach (var ceil in figure.Ceils)
            {
                if (AllFigureCeils.Any(c => c.IsInSameColumn(ceil) &&
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

        public void RotateFigureRight(TetrisFigure figure)
        {
            var matrix = new Matrix(4);

            var topRow = figure.GetTopRow();
            var leftColumn = figure.GetLeftColumn();
            foreach (var ceil in figure.Ceils)
                matrix[ceil.Row - topRow, ceil.Column - leftColumn] = 1;

            matrix.RotateClockwise();

            var ceils = new List<Ceil>(figure.Ceils);
            for (int row = 0; row < 4; row++)
            {
                for (int column = 0; column < 4; column++)
                {
                    if (matrix[row, column] == 1)
                    {
                        var ceil = ceils.First();
                        ceil.Row = row + topRow;
                        ceil.Column = column + leftColumn - 2;
                        ceils.Remove(ceil);
                    }
                }
            }
        }

        private List<int> GetFilledRows()
        {
            var ceilsPerRows = new Dictionary<int, int>();
            foreach (var ceil in AllFigureCeils)
            {
                var row = ceil.Row;
                if (ceilsPerRows.ContainsKey(row))
                    ceilsPerRows[row]++;
                else
                    ceilsPerRows[row] = 1;
            }

            var filledRows = new List<int>();
            foreach (var row in ceilsPerRows.Keys)
            {
                if (ceilsPerRows[row] == ColumnCount)
                    filledRows.Add(row);
            }

            return filledRows;
        }

        private TetrisFigure FindFigureByCeil(Ceil ceil)
        {
            foreach (var figure in fallenFigures)
            {
                if (figure.Ceils.Contains(ceil))
                    return figure;
            }

            throw new CeilNotFoundException(ceil);
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

        private void ClearRows(List<int> rows)
        {
            foreach (var ceil in AllFigureCeils)
            {
                if (rows.Contains(ceil.Row))
                {
                    var figure = FindFigureByCeil(ceil);
                    figure.RemoveCeil(ceil);
                    Children.Remove(ceil);
                }
            }
        }

        private void RemoveEmptyFigures()
        {
            var emptyFigures = new List<TetrisFigure>();
            foreach (var figure in fallenFigures)
            {
                if (figure.Ceils.Count() == 0)
                    emptyFigures.Add(figure);
            }

            fallenFigures.RemoveRange(emptyFigures);
        }

        private void OnFigureFallen(TetrisFigure figure)
        {
            var filledRows = GetFilledRows();

            ClearRows(filledRows);
            RemoveEmptyFigures();

            if (filledRows.Count() > 0)
            {
                var needLowerRows = rows.Where(r => r < filledRows.Max()).ToList();
                LowerRows(needLowerRows, filledRows.Count());
            }

            if (figure.Ceils.Any(c => c.Row == 0))
            {
                GameLost.Invoke();
            }
        }

        private void LowerRows(List<int> rows, int amount)
        {
            foreach (var ceil in AllFigureCeils)
            {
                if (rows.Contains(ceil.Row))
                    ceil.Row += amount;
            }
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

        private void InitializeBackground()
        {
            AddCeils(Ceil.CreateMany(RowCount, ColumnCount));
        }
    }
}
