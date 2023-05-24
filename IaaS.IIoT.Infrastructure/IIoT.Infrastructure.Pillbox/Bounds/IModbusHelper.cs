using static Infrastructure.Pillbox.Bounds.IModbusHelper;

namespace Infrastructure.Pillbox.Bounds;
public interface IModbusHelper
{
    Task ReadAsync(CancellationToken token);

    [StructLayout(LayoutKind.Auto)]
    readonly record struct ElectricityBucket
    {
        public required double AverageVoltage { get; init; }
        public required double AverageCurrent { get; init; }
        public required double PowerFactor { get; init; }
        public required double ReactiveEnergy { get; init; }
        public required double ActiveEnergy { get; init; }
        public required double ApparentEnergy { get; init; }
        public required double CarbonEmission { get; init; }
    }
    ElectricityBucket Electricity { get; }
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class ModbusHelper(IBaseLoader baseLoader) : IModbusHelper
{
    readonly IBaseLoader _baseLoader = baseLoader;
    public async Task ReadAsync(CancellationToken token)
    {
        try
        {
            if (_baseLoader.Profile is not null && _baseLoader.Profile.SerialEntry.Enabled)
            {
                Token = token;
                Master ??= new()
                {
                    Parity = _baseLoader.Profile.SerialEntry.Parity,
                    BaudRate = _baseLoader.Profile.SerialEntry.BaudRate,
                    StopBits = _baseLoader.Profile.SerialEntry.StopBits
                };
                Master.Connect(_baseLoader.Profile.SerialEntry.Port, ModbusEndianness.BigEndian);
                await ElectricityAsync(0x01, _baseLoader.Profile.Formulation.CarbonEmissionFactor, _baseLoader.Profile.Formulation.GlobalWarmingPotential);
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
        async ValueTask ElectricityAsync(int slaveId, double carbonEmissionFactor, int globalWarmingPotential)
        {
            var length = 72;
            var decimalPlaces = 2;
            var count = length / 2;
            var floats = new float[count];
            var ushorts = new ushort[length];
            var values = await Master.ReadInputRegistersAsync<ushort>(slaveId, 0x1100, length, Token);
            ushorts = values.ToArray();
            {
                for (int item = default; item < count; item++)
                {
                    var lowOrder = BitConverter.GetBytes(ushorts[2 * item]);
                    var highOrder = BitConverter.GetBytes(ushorts[2 * item + 1]);
                    floats[item] = BitConverter.ToSingle(CollectionUtility.Concat(lowOrder, highOrder), default);
                }
                var activeEnergy = Math.Round(floats[15], decimalPlaces);
                Electricity = new()
                {
                    AverageVoltage = Math.Round(floats[9], decimalPlaces),
                    AverageCurrent = Math.Round(floats[10], decimalPlaces),
                    PowerFactor = Math.Round(floats[14], decimalPlaces),
                    ReactiveEnergy = Math.Round(floats[16], decimalPlaces),
                    ActiveEnergy = activeEnergy,
                    ApparentEnergy = Math.Round(floats[17], decimalPlaces),
                    CarbonEmission = Math.Round(activeEnergy * carbonEmissionFactor * globalWarmingPotential, 2, MidpointRounding.AwayFromZero)
                };
            }
        }
    }
    CancellationToken Token { get; set; }
    ModbusRtuClient? Master { get; set; }
    List<string> Histories { get; init; } = new();
    public ElectricityBucket Electricity { get; private set; }
}