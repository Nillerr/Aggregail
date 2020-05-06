using System;

namespace EventSourcing.Demo.Cases
{
    public static class ActionExtensions
    {
        public static Func<T, int> ToFunc<T>(this Action<T> action) => action.Return(0);
        
        public static Func<T, TResult> Return<T, TResult>(this Action<T> action, TResult result) => (arg) =>
        {
            action(arg);
            return result;
        };
    }
}