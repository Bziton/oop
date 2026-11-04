using System;
using System.Collections.Generic;

public static class CollectionHelper<T>
{
    public static IEnumerable<TResult> Map<TResult>(IEnumerable<T> collection, Func<T, TResult> mapper)
    {
        if (collection == null)
        {
            throw new ArgumentNullException(nameof(collection));
        }

        if (mapper == null)
        {
            throw new ArgumentNullException(nameof(mapper));
        }

        return MapIterator(collection, mapper);
    }

    private static IEnumerable<TResult> MapIterator<TResult>(IEnumerable<T> collection, Func<T, TResult> mapper)
    {
        foreach (T item in collection)
        {
            yield return mapper(item);
        }
    }
}