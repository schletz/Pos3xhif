using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ListDemo.Extensions
{
    public static class ObservableCollectionExtensions
    {
        public static void AddRange<TSource>(this ObservableCollection<TSource> source, IEnumerable<TSource> items)
        {
            foreach (var item in items)
            {
                source.Add(item);
            }
        }

        public static void ReplaceAll<TSource>(this ObservableCollection<TSource> source, IEnumerable<TSource> items)
        {
            source.Clear();
            AddRange(source, items);
        }
    }
}
