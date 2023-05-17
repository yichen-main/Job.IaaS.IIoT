using static Infrastructure.Pillbox.Bounds.IModbusHelper;

namespace Infrastructure.Pillbox.Bounds;
internal interface IModbusHelper
{
    Task ReadAsync();

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
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class ModbusHelper : IModbusHelper
{
    readonly IBaseLoader _baseLoader;
    public ModbusHelper(IBaseLoader baseLoader) => _baseLoader = baseLoader;
    public async Task ReadAsync()
    {
        try
        {
            if (_baseLoader.Profile is not null && _baseLoader.Profile.SerialEntry.Enabled)
            {
                Master ??= new()
                {
                    Parity = _baseLoader.Profile.SerialEntry.Parity,
                    BaudRate = _baseLoader.Profile.SerialEntry.BaudRate,
                    StopBits = _baseLoader.Profile.SerialEntry.StopBits
                };
                Master.Connect(_baseLoader.Profile.SerialEntry.Port);
                await MainElectricityAsync(0x01);
                if (Histories.Any()) Histories.Clear();
                Master.Dispose();
                Master = null;
            }
        }
        catch (Exception e)
        {
            if (!Histories.Contains(e.Message))
            {
                Histories.Add(e.Message);
                _baseLoader.Record(RecordType.MachineParts, new()
                {
                    Title = $"{nameof(ModbusHelper)}.{nameof(ReadAsync)}",
                    Name = "Modbus RTU",
                    Message = e.Message
                });
            }
        }
        async ValueTask MainElectricityAsync(int slaveId)
        {
            var datas = await Master.ReadInputRegistersAsync<int>(slaveId, 0x1333, 17);
            await _baseLoader.PushBrokerAsync("parts/smart-meters/data", new MainElectricityBucket
            {
                AverageVoltage = 0,
                AverageCurrent = 0,
                PowerFactor = 0,
                ReactivePower = 0,
                ReactiveEnergy = 0,
                ActiveEnergy = 0x01,
                ActivePower = 0x01,
                ApparentPower = 0,
                ApparentEnergy = 0
            }.ToJson());
        }
    }
    ModbusRtuClient? Master { get; set; }
    List<string> Histories { get; init; } = new();
}