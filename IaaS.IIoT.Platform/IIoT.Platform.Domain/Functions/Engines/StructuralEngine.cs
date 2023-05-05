namespace Platform.Domain.Functions.Engines;

[Dependency(ServiceLifetime.Singleton)]
file sealed class StructuralEngine : IStructuralEngine
{
    public StructuralEngine()
    {
        Transport = new MqttFactory().CreateMqttServer(new MqttServerOptionsBuilder().WithDefaultEndpoint().WithDefaultEndpointPort(Queue).Build());
    }
    public required MqttServer Transport { get; init; }
    public ArrayPool<byte> BytePool { get; } = ArrayPool<byte>.Shared;
    public string Version { get; } = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ?? string.Empty;
}