namespace Lumo.Application.Exceptions;
public class SortFieldException : ArgumentException
{
    public SortFieldException(string message) : base(message)
    {
    }
}
