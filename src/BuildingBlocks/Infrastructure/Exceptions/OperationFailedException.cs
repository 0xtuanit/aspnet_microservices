namespace OcelotApiGw.Exceptions;

public class OperationFailedException : ApplicationException
{
    public OperationFailedException(string action) :
        base($"Action \"{action}\" got failed.")
    {
    }
}