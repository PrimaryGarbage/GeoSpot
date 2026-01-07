namespace GeoSpot.Common.Exceptions;

public class InternalProblemException : Exception
{
    public InternalProblemException() :  base() { }
    public InternalProblemException(string message) : base(message) { }
    public InternalProblemException(string message, Exception inner) : base(message, inner) { }
}