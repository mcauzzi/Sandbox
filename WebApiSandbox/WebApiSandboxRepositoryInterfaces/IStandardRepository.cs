namespace WebApiSandboxRepositoryInterfaces;

public interface IStandardRepository<T>
{
    public Task<IEnumerable<T>> Get(int rows, int offset);
}