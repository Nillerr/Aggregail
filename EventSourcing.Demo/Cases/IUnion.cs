using System;
using JetBrains.Annotations;

namespace EventSourcing.Demo.Cases
{
    public interface IUnion<out T1, out T2>
    {
        TResult Apply<TResult>(
            [InstantHandle] Func<T1, TResult> case1,
            [InstantHandle] Func<T2, TResult> case2
        );
    }
}