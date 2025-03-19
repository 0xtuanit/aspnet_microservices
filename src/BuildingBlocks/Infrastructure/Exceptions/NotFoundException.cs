namespace OcelotApiGw.Exceptions;

public class NotFoundException : ApplicationException
{
    public NotFoundException(string entity) :
        base($"Entity \"{entity}\" is not found.")
    {
    }
}