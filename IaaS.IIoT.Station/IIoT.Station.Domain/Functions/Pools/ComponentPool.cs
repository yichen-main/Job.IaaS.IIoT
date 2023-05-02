using static Station.Domain.Shared.Functions.Pools.IComponentPool;

namespace Station.Domain.Functions.Pools;
internal sealed class ComponentPool : IComponentPool
{
    public void Push(in SpindleThermalCompensationData value) => SpindleThermalCompensation = value;
    public SpindleThermalCompensationData SpindleThermalCompensation { get; private set; } = new();
}