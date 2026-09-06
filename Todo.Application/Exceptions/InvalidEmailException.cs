namespace Todo.Application.Exceptions;

internal class InvalidEmailException : Exception
{
    public InvalidEmailException(string message) : base(message)
    {
        
    }
}
