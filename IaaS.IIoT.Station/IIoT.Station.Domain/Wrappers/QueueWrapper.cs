namespace Station.Domain.Wrappers;

[Dependency(ServiceLifetime.Singleton)]
file sealed class QueueWrapper : IQueueWrapper
{
    readonly ISensorPool _sensorPool;
    readonly IComponentPool _componentPool;
    public QueueWrapper(ISensorPool sensorPool, IComponentPool componentPool)
    {
        _sensorPool = sensorPool;
        _componentPool = componentPool;
    }
    public IIcpdasQueue Icpdas => new IcpdasQueue(_sensorPool);
    public IInteriorQueue Interior => new InteriorQueue(_componentPool);
}