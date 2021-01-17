using System;
using System.Collections.Generic;
using System.Text;

namespace Tetris.Extensions
{
    public static class ListExtension
    {
        public static void RemoveRange<T>(this IList<T> list, IEnumerable<T> enumerable)
        {
            foreach (var value in enumerable)
                list.Remove(value);
        }
    }
}
