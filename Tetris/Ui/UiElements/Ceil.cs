using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Tetris.Ui.UserControllers;

namespace Tetris.Ui.UiElements
{
    public class Ceil : Border
    {
        public int Row { get; set; }

        public int Column { get; set; }

        public Ceil() { }

        public Ceil(Ceil ceil)
        {
        }

        public static Ceil Create(int row, int column, Color background)
        {
            var ceil = Create(background);
            ceil.Row = row;
            ceil.Column = column;

            return ceil;
        }

        public static Ceil Create(int row, int column)
        {
            var ceil = Create(row, column, Colors.Gray);
            
            return ceil;
        }

        public static Ceil Create(Color background)
        {
            var ceil = new Ceil();
            ceil.BorderBrush = new SolidColorBrush(Colors.White);
            ceil.BorderThickness = new Thickness(PlayGrid.ThicknessBetweenWidth);
            ceil.Child = new StackPanel
            {
                Width = PlayGrid.CeilSize,
                Height = PlayGrid.CeilSize,
                Background = new SolidColorBrush(background)
            };

            return ceil;
        }

        public static List<Ceil> CreateMany(int rowRange, int columnRange)
        {
            var ceils = new List<Ceil>();
            for (int column = 0; column < columnRange; column++)
            {
                for (int row = 0; row < rowRange; row++)
                {
                    ceils.Add(Create(row, column));
                }
            }

            return ceils;
        }

        public Ceil Clone()
        {
            var ceil = new Ceil();
            var stackPanel = (StackPanel)Child;

            ceil.Column = Column;
            ceil.Row = Row;
            ceil.BorderBrush = BorderBrush.Clone();
            ceil.BorderThickness = BorderThickness;
            ceil.Child = new StackPanel
            {
                Width = stackPanel.Width,
                Height = stackPanel.Height,
                Background = stackPanel.Background.Clone()
            };

            return ceil;
        }
    }
}
