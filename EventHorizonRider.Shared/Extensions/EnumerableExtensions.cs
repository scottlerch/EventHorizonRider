using System.Collections.Generic;

namespace EventHorizonRider.Core.Extensions;

internal static class EnumerableExtensions
{
    public static T Next<T>(this IEnumerator<T> enumerator) where T : class => enumerator.MoveNext() ? enumerator.Current : null;

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
