using static Infrastructure.Garner.Architects.Pools.IComponentPool;

namespace Infrastructure.Garner.Architects.Pools;
public interface IComponentPool
{
    void Push(in SpindleThermalCompensationData value);

    [StructLayout(LayoutKind.Auto)]
    readonly record struct SpindleThermalCompensationData
    {
        public float TemperatureFirst { get; init; }
        public float TemperatureSecond { get; init; }
        public float TemperatureThird { get; init; }
        public float TemperatureFourth { get; init; }
        public float TemperatureFifth { get; init; }
        public float CompensationX { get; init; }
        public float CompensationY { get; init; }
        public float CompensationZ { get; init; }
    }
    SpindleThermalCompensationData SpindleThermalCompensation { get; }
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class ComponentPool : IComponentPool
{
    public void Push(in SpindleThermalCompensationData value) => SpindleThermalCompensation = value;
    public SpindleThermalCompensationData SpindleThermalCompensation { get; private set; } = new();
}