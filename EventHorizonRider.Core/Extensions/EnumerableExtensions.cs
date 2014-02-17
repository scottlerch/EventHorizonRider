using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHorizonRider.Core.Extensions
{
    internal static class EnumerableExtensions
    {
        public static T Next<T>(this IEnumerator<T> enumerator) where T : class
        {
            if (enumerator.MoveNext())
            {
                return enumerator.Current;
            }

            return null;
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
