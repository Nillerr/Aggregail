namespace Aggregail
{
    /// <summary>
    /// Serializes events to / from JSON
    /// </summary>
    public interface IJsonEventSerializer
    {
        /// <summary>
        /// Deserializes UTF-8 encoded bytes of JSON to an event of type <typeparamref name="T"/> 
        /// </summary>
        /// <param name="source">UTF-8 encoded bytes of JSON</param>
        /// <typeparam name="T">The type of event</typeparam>
        /// <returns>An event</returns>
        T Deserialize<T>(byte[] source)
            where T : class;
        
        /// <summary>
        /// Serializes an event of type <typeparamref name="T"/> to UTF-8 encoded bytes of JSON
        /// </summary>
        /// <param name="source">An event</param>
        /// <typeparam name="T">The type of event</typeparam>
        /// <returns>UTF-8 encoded bytes of JSON</returns>
        byte[] Serialize<T>(T source)
            where T : class;
    }
}