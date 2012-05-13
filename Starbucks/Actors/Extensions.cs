using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace Starbucks.Actors
{
    public static class Extensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T item in source) action(item);
        }

        public static IObservable<T> On<T>(this IObservable<T> source, int numberOftimes = 1)
        {
            return source.Take(numberOftimes);
        }

        public static IDisposable Dispatch<T, TResult>(this IObservable<T> source, 
            Func<T, TResult> project, Action<TResult> dispatcher)
        {
            return source.Select(project).Subscribe(dispatcher);
        }
    }
}