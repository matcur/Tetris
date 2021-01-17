using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Tetris.Core
{
    public class Matrix : IEnumerable<int[]>
    {
        public int this[int row, int column]
        {
            get => grid[row, column];
            set => grid[row, column] = value;
        }

        private readonly int[,] grid;

        private readonly int size;

        public Matrix(int size, int defaultValue = 0)
        {
            this.size = size;
            
            grid = new int[size, size];
            for (var row = 0; row < size; row++)
            {
                for (var column = 0; column < size; column++)
                {
                    grid[row, column] = defaultValue;
                }
            }
        }

        public void RotateClockwise()
        {
            for (int i = 0; i < size / 2; i++)
            {
                for (int j = i; j < size - i - 1; j++)
                {
                    int temp = grid[i, j];
                    grid[i,j] = grid[size - 1 - j, i];
                    grid[size - 1 - j, i] = grid[size - 1 - i, size - 1 - j];
                    grid[size - 1 - i, size - 1 - j] = grid[j, size - 1 - i];
                    grid[j, size - 1 - i] = temp;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return grid.GetEnumerator();
        }

        public IEnumerator<int[]> GetEnumerator()
        {
            while (grid.GetEnumerator().MoveNext())
                yield return (int[])grid.GetEnumerator().Current;
        }
    }
}
