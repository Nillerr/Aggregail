using System;
using Newtonsoft.Json;

namespace EventSourcing.Demo.Robots
{
    public sealed class EntityListResponse<T>
    {
        [JsonProperty("@odata.context", Required = Required.Always)]
        public Uri OdataContext { get; set; }

        [JsonProperty("value", Required = Required.Always)]
        public T[] Value { get; set; }
    }
}