namespace Station.Domain.Accessors.Queues;
internal sealed class InteriorQueue : IInteriorQueue
{
    readonly IComponentPool _componentPool;
    public InteriorQueue(IComponentPool componentPool) => _componentPool = componentPool;
    public void PushSpindleThermalCompensation(in string payload)
    {
        var thermalFirst = float.Parse(payload.Split('*')[1]);
        var thermalSecond = float.Parse(payload.Split('*')[3]);
        var thermalThird = float.Parse(payload.Split('*')[5]);
        var thermalFourth = float.Parse(payload.Split('*')[7]);
        var thermalFifth = float.Parse(payload.Split('*')[9]);
        var compensationX = float.Parse(payload.Split('*')[11]);
        var compensationY = float.Parse(payload.Split('*')[13]);
        var compensationZ = float.Parse(payload.Split('*')[15]);
        if (thermalFirst != default && thermalSecond != default && thermalThird != default)
        {
            _componentPool.Push(new IComponentPool.SpindleThermalCompensationData()
            {
                TemperatureFirst = thermalFirst,
                TemperatureSecond = thermalSecond,
                TemperatureThird = thermalThird,
                TemperatureFourth = thermalFourth,
                TemperatureFifth = thermalFifth,
                CompensationX = compensationX,
                CompensationY = compensationY,
                CompensationZ = compensationZ
            });
        }
    }
}