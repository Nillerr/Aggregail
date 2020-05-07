using System;

namespace EventSourcing.Demo.Cases
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class JsonDiscriminatorAttribute : Attribute
    {
        public JsonDiscriminatorAttribute(string discriminator)
        {
            Discriminator = discriminator;
        }

        public string Discriminator { get; }
    }
}