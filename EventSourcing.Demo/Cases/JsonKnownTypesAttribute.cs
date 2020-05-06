using System;

namespace EventSourcing.Demo.Cases
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public sealed class JsonKnownTypesAttribute : Attribute
    {
        public JsonKnownTypesAttribute(params Type[] knownTypes)
        {
            KnownTypes = knownTypes;
        }
        
        public JsonKnownTypesAttribute(string propertyName, params Type[] knownTypes)
        {
            PropertyName = propertyName;
            KnownTypes = knownTypes;
        }

        public string PropertyName { get; set; } = "Kind";
        public Type[] KnownTypes { get; }
    }
}