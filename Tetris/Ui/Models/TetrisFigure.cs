using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Tetris.Core;
using Tetris.Exceptions.Ceils;
using Tetris.Extensions;
using Tetris.Ui.UiElements;
using Tetris.Ui.UserControllers;

namespace Tetris.Ui.Models
{
    public class TetrisFigure : Model
    {
        public const int MiddleColumn = PlayGrid.ColumnCount / 2;

        public IReadOnlyList<Ceil> Ceils => ceils;

        private IReadOnlyDictionary<TetrisFigureMove, Action> movesTo;

        private static List<Func<TetrisFigure>> creators = new List<Func<TetrisFigure>>
        {
            CreateL,
            CreateT,
            CreateStraight,
            CreateSquare,
            CreateSkew,
        };

        private List<Ceil> ceils = new List<Ceil>();

        private List<int> plot = new List<int>
        {
            0, 0, 0, 0,
            0, 0, 0, 0,
            0, 0, 0, 0,
            0, 0, 0, 0,
        };

        public TetrisFigure()
        {
            movesTo = new Dictionary<TetrisFigureMove, Action>
            {
                { TetrisFigureMove.Down, MoveDown },
                { TetrisFigureMove.Left, MoveLeft },
                { TetrisFigureMove.Right, MoveRight },
            };
        }

        public TetrisFigure(IEnumerable<Ceil> enumerable): this()
        {
            ceils.AddRange(enumerable);
        }

        public TetrisFigure(params Ceil[] ceils): this()
        {
            this.ceils.AddRange(ceils);
        }

        #region Create Figure Methods

        public static TetrisFigure CreateRandom()
        {
            var index = new Random().Next(creators.Count - 1);

            return creators[index].Invoke();
        }

        public static TetrisFigure CreateStraight()
        {
            var color = Colors.DarkBlue;

            return new TetrisFigure(
                Ceil.Create(0, 4, color),
                Ceil.Create(1, 4, color),
                Ceil.Create(2, 4, color),
                Ceil.Create(3, 4, color)
                );
        }

        public static TetrisFigure CreateL()
        {
            var color = Colors.Orange;

            return new TetrisFigure(
                Ceil.Create(0, MiddleColumn - 1, color),
                Ceil.Create(1, MiddleColumn - 1, color),
                Ceil.Create(2, MiddleColumn - 1, color),
                Ceil.Create(2, MiddleColumn, color)
                );
        }

        public static TetrisFigure CreateT()
        {
            var color = Colors.Purple;

            return new TetrisFigure(
                Ceil.Create(0, MiddleColumn + 1, color),
                Ceil.Create(0, MiddleColumn - 1, color),
                Ceil.Create(0, MiddleColumn, color),
                Ceil.Create(1, MiddleColumn, color)
                );
        }

        public static TetrisFigure CreateSkew()
        {
            var color = Colors.Green;

            return new TetrisFigure(
                Ceil.Create(1, MiddleColumn - 2, color),
                Ceil.Create(1, MiddleColumn - 1, color),
                Ceil.Create(0, MiddleColumn - 1, color),
                Ceil.Create(0, MiddleColumn, color)
                );
        }

        public static TetrisFigure CreateSquare()
        {
            var color = Colors.Yellow;

            return new TetrisFigure(
                Ceil.Create(1, MiddleColumn, color),
                Ceil.Create(1, MiddleColumn - 1, color),
                Ceil.Create(0, MiddleColumn - 1, color),
                Ceil.Create(0, MiddleColumn, color)
                );
        }

        #endregion

        public TetrisFigure Clone()
        {
            var newCeils = new List<Ceil>();
            foreach (var ceil in ceils)
                newCeils.Add(ceil.Clone());

            return new TetrisFigure(newCeils);
        }

        public int GetTopRow()
        {
            var topCeil = ceils.First();
            foreach (var ceil in ceils)
            {
                if (ceil.Row < topCeil.Row)
                    topCeil = ceil;
            }

            return topCeil.Row;
        }

        public int GetLeftColumn()
        {
            var leftCeil = ceils.First();
            foreach (var ceil in ceils)
            {
                if (ceil.Column < leftCeil.Column)
                    leftCeil = ceil;
            }

            return leftCeil.Column;
        }

        public void MoveRight()
        {
            ceils.ForEach(ceil => ceil.Column++);
        }

        public void MoveDown()
        {
            ceils.ForEach(ceil => ceil.Row++);
        }

        public void MoveLeft()
        {
            ceils.ForEach(ceil => ceil.Column--);
        }

        public void RemoveCeil(Ceil ceil)
        {
            if (!ceils.Remove(ceil))
                throw new CeilNotFoundException(ceil);
        }

        public void MoveTo(TetrisFigureMove move)
        {
            movesTo[move].Invoke();
        }

        public bool HasCeilInColumn(Ceil ceil)
        {
            return HasCeilInColumn(ceil.Column);
        }

        public bool HasCeilInColumn(int column)
        {
            return ceils.Where(c => c.Column == column).Any();
        }

        public bool HasCeilOnRow(Ceil ceil)
        {
            return HasCeilOnRow(ceil.Row);
        }

        public bool HasCeilOnRow(int row)
        {
            return ceils.Where(c => c.Row == row).Any();
        }
    }
}
