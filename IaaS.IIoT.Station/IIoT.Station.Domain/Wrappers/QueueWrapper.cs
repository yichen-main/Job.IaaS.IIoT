namespace Station.Domain.Wrappers;

[Dependency(ServiceLifetime.Singleton)]
file sealed class QueueWrapper : IQueueWrapper
{
    public IIcpdasQueue Icpdas => new IcpdasQueue(SensorPool);
    public IInteriorQueue Interior => new InteriorQueue(ComponentPool);
    public required ISensorPool SensorPool { get; init; }
    public required IComponentPool ComponentPool { get; init; }
}