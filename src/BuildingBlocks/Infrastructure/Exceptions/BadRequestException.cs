namespace OcelotApiGw.Exceptions;

public class BadRequestException : ApplicationException
{
    public BadRequestException(string request) :
        base($"Unable to process because of this bad request {request}.")
    {
    }
}