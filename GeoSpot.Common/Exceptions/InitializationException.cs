namespace GeoSpot.Common.Exceptions;

public class InitializationException : Exception
{
    public InitializationException() :  base() { }
    public InitializationException(string message) : base(message) { }
    public InitializationException(string message, Exception inner) : base(message, inner) { }
}