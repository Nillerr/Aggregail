namespace Aggregail
{
    public readonly struct EventType<T>
    {
        public EventType(string value)
        {
            Value = value;
        }

        public string Value { get; }
        
        public static implicit operator EventType<T>(string value) => new EventType<T>(value);
    }
}