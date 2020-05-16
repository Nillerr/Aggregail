using Aggregail.Newtonsoft.Json;

namespace EventSourcing.Demo.Robots.Serialization
{
    public sealed class SerialNumberConverter : ValueObjectConverter<SerialNumber, string>
    {
        public SerialNumberConverter()
            : base(value => new SerialNumber(value))
        {
        }
    }
}