using static Infrastructure.Pillbox.Bounds.IModbusHelper;

namespace Infrastructure.Pillbox.Bounds;
internal interface IModbusHelper
{
    Task ReadAsync(MainText.TextSerialEntry serialEntry);

    [StructLayout(LayoutKind.Auto)]
    readonly record struct MainElectricityBucket
    {
        public required uint AverageVoltage { get; init; }
        public required uint AverageCurrent { get; init; }
        public required short PowerFactor { get; init; }
        public required int ReactivePower { get; init; }
        public required int ReactiveEnergy { get; init; }
        public required int ActivePower { get; init; }
        public required int ActiveEnergy { get; init; }
        public required int ApparentPower { get; init; }
        public required int ApparentEnergy { get; init; }
    }
    MainElectricityBucket MainElectricity { get; }
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class ModbusHelper : IModbusHelper
{
    public async Task ReadAsync(MainText.TextSerialEntry serialEntry)
    {
        try
        {
            Master ??= new() { Parity = serialEntry.Parity, BaudRate = serialEntry.BaudRate, StopBits = serialEntry.StopBits };
            Master.Connect(serialEntry.SerialPort);
            await MainElectricityAsync(0x01);
            Master.Dispose();
            Master = null;
        }
        catch (Exception e)
        {
            await Console.Out.WriteLineAsync(e.Message);
        }
    }
    async ValueTask MainElectricityAsync(int slaveId)
    {
        var datas = await Master!.ReadInputRegistersAsync<uint>(slaveId, 0x1233, 17);
    }
    ModbusRtuClient? Master { get; set; }
    public MainElectricityBucket MainElectricity { get; private set; }
}