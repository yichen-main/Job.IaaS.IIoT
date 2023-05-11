namespace Infrastructure.Core.Boundaries;
public interface IInitialConstructor
{
    ArrayPool<byte> BytePool { get; }
    string Version { get; }
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class InitialConstructor : IInitialConstructor
{
    public ArrayPool<byte> BytePool { get; } = ArrayPool<byte>.Shared;
    public string Version { get; } = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ?? string.Empty;
}