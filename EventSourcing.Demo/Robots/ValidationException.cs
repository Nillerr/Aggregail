using System;

namespace EventSourcing.Demo.Robots
{
    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message)
        {
        }
    }
}