using System;
using JetBrains.Annotations;

namespace EventSourcing.Demo.Cases
{
    public static class UnionExtensions
    {

        public static void Apply<T1, T2>(
            this IUnion<T1, T2> source,
            [InstantHandle] Action<T1> case1,
            [InstantHandle] Action<T2> case2
        )
        {
            source.Apply(case1.ToFunc(), case2.ToFunc());
        }
    }
}