namespace OcelotApiGw.Exceptions;

public class AuthenticationFailedException : ApplicationException
{
    public AuthenticationFailedException(string account) :
        base($"Account \"{account}\" was not authenticated.")
    {
    }
}