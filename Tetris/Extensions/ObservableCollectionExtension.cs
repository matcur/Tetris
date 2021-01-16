using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Tetris.Extensions
{
    public static class ObservableCollectionExtension
    {
        public static void AddRange<T>(this ObservableCollection<T> source, IEnumerable<T> enumerable)
        {
            foreach (var value in enumerable)
                source.Add(value);
        }

        public static void ForEach<T>(this ObservableCollection<T> source, Action<T> action)
        {
            foreach (var value in source)
                action.Invoke(value);
        }
    }
}
