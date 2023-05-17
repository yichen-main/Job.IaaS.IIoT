using static Infrastructure.Pillbox.Bounds.IModbusHelper;

namespace Infrastructure.Pillbox.Bounds;
internal interface IModbusHelper
{
    Task ReadAsync();

    [StructLayout(LayoutKind.Auto)]
    readonly record struct MainElectricityBucket
    {
        public required double AverageVoltage { get; init; }
        public required double AverageCurrent { get; init; }
        public required double PowerFactor { get; init; }
        public required double ReactiveEnergy { get; init; }
        public required double ActiveEnergy { get; init; }
        public required double ApparentEnergy { get; init; }
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
                Master.Connect(_baseLoader.Profile.SerialEntry.Port, ModbusEndianness.BigEndian);
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
            var length = 72;
            var decimalPlaces = 2;
            var count = length / 2;
            var floats = new float[count];
            var ushorts = new ushort[length];
            var values = await Master.ReadInputRegistersAsync<ushort>(slaveId, 0x1100, length);
            ushorts = values.ToArray();
            {
                for (var item = 0; item < count; item++)
                {
                    var lowOrder = BitConverter.GetBytes(ushorts[2 * item]);
                    var highOrder = BitConverter.GetBytes(ushorts[2 * item + 1]);
                    floats[item] = BitConverter.ToSingle(CollectionUtility.Concat(lowOrder, highOrder), default);
                }
                await _baseLoader.PushBrokerAsync("parts/smart-meters/data", new MainElectricityBucket
                {
                    AverageVoltage = Math.Round(floats[9], decimalPlaces),
                    AverageCurrent = Math.Round(floats[10], decimalPlaces),
                    PowerFactor = Math.Round(floats[14], decimalPlaces),
                    ReactiveEnergy = Math.Round(floats[16], decimalPlaces),
                    ActiveEnergy = Math.Round(floats[15], decimalPlaces),
                    ApparentEnergy = Math.Round(floats[17], decimalPlaces)
                }.ToJson());
            }
        }
    }
    ModbusRtuClient? Master { get; set; }
    List<string> Histories { get; init; } = new();
}