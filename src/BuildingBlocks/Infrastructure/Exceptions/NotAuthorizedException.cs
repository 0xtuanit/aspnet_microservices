namespace OcelotApiGw.Exceptions;

public class NotAuthorizedException : ApplicationException
{
    public NotAuthorizedException(string account) :
        base($"Account \"{account}\" was not authorized.")
    {
    }
}