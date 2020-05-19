namespace Aggregail
{
    /// <summary>
    /// Parses a string <paramref name="input"/> to the type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="input">String to parse.</param>
    /// <typeparam name="T">Type to parse as.</typeparam>
    public delegate T Parser<out T>(string input);
}