using System;

namespace EventSourcing.Demo
{
    public static class NullableExtensions
    {
        public static TResult? Select<T, TResult>(this T? source, Func<T, TResult> selector)
            where T : struct
            where TResult : struct
        {
            return source.HasValue ? (TResult?) selector(source.Value) : null;
        }
    }
}