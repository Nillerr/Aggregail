using System;

namespace EventSourcing.Demo.Cases
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class JsonDiscriminatorAttribute : Attribute
    {
        public JsonDiscriminatorAttribute(object discriminator)
        {
            Discriminator = discriminator;
        }

        public object Discriminator { get; }
    }
}