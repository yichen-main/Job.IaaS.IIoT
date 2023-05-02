namespace Station.Domain.Shared.Timeseries.Tacks.Sensors;
public interface IAttachedSensor
{
    Task InsertAsync(Data data);

    [StructLayout(LayoutKind.Auto)]
    readonly record struct Data
    {
        public required float ElectricalBoxHumidity { get; init; }
        public required float ElectricalBoxTemperature { get; init; }
        public required float WaterTankTemperature { get; init; }
    }
}