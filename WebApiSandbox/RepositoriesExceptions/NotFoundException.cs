namespace RepositoriesExceptions;

public class NotFoundException(string typeName, object key)
    : Exception($"The entity of type {typeName} does not exist with the key {key}");