using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHorizonRider.Core
{
    internal static class EnumerableExtensions
    {
        public static T Next<T>(this IEnumerator<T> enumerator)
        {
            enumerator.MoveNext();
            return enumerator.Current;
        }

        public static IEnumerable<T> Concat<T>(this IEnumerable<T> enumerable, params IEnumerable<T>[] enumerables)
        {
            foreach (var item in enumerable)
            {
                yield return item;
            }

            foreach (var nextEnumerable in enumerables)
            {
                foreach (var item in nextEnumerable)
                {
                    yield return item;
                }
            }
        }
    }
}
