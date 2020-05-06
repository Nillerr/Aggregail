using System;
using JetBrains.Annotations;
using MongoDB.Bson.Serialization.Attributes;

namespace EventSourcing.Demo.Cases
{
    [JsonKnownTypes(typeof(User), typeof(Import))]
    public abstract class RobotRegistrar : IUnion<RobotRegistrar.User, RobotRegistrar.Import>
    {
        private RobotRegistrar()
        {
        }

        public abstract TResult Apply<TResult>(
            [InstantHandle] Func<User, TResult> user,
            [InstantHandle] Func<Import, TResult> import
        );

        [BsonDiscriminator("user")]
        public sealed class User : RobotRegistrar
        {
            public User(UserId id)
            {
                Id = id;
            }

            public UserId Id { get; }

            public override TResult Apply<TResult>(Func<User, TResult> user, Func<Import, TResult> import) =>
                user(this);
        }

        [BsonDiscriminator("import")]
        public sealed class Import : RobotRegistrar
        {
            public static readonly Import Instance = new Import();

            private Import()
            {
            }

            public override TResult Apply<TResult>(Func<User, TResult> user, Func<Import, TResult> import) =>
                import(this);
        }
    }
}