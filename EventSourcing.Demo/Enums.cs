using System;
using System.Collections.Generic;

namespace EventSourcing.Demo
{
    public static class Enums
    {
        private static readonly Random Random = new Random();

        public static T[] GetValues<T>() where T : struct, Enum => Enum<T>.GetValues();

        public static T GetRandomValue<T>() where T : struct, Enum => Enum<T>.GetRandomValue();

        private static class Enum<T> where T : struct, Enum
        {
            private static readonly List<T> ValuesByIndex = new List<T>();

            static Enum()
            {
                var values = Enum.GetValues(typeof(T));
                foreach (var value in values)
                {
                    ValuesByIndex.Add((T) value!);
                }
            }

            public static T[] GetValues() => ValuesByIndex.ToArray();

            public static T GetRandomValue() => ValuesByIndex[Random.Next(0, ValuesByIndex.Count)];
        }
    }
}