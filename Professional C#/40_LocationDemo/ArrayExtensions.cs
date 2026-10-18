using System;
using System.Collections.Generic;

namespace LocationDemo
{
    public static class ArrayExtensions
    {
        /// <summary>
        /// Sucht alle Werte von Values in LookupArray und gibt ein Array in der Größe des Lookup
        /// Arrays zurück. Nicht gefundene werde haben den default Wert, bei mehreren Werten wird
        /// der erste Wert verwendet.
        /// </summary>
        public static Tsource[] Spread<Tsource>(this IEnumerable<Tsource> values, Tsource[] lookupArray) =>
            Spread(values, lookupArray, x => x);

        public static Tresult[] Spread<Tsource, Tresult>(this IEnumerable<Tresult> values, Tsource[] lookupArray, Func<Tresult, Tsource> selector)
        {
            Tresult[] result = new Tresult[lookupArray.Length];
            foreach (var value in values)
            {
                int idx = Array.IndexOf(lookupArray, selector(value));
                if (idx != -1) result[idx] = value;
            }
            return result;
        }
    }
}
