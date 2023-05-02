namespace Station.Domain.Shared.Functions.Engines;
public interface IStructuralEngine
{
    ValueTask InitialDataPoolAsync(string url, string organize, string userName, string password, string bucket);
    abstract class MetaBase
    {
        [Column("_machine_id", IsTag = true)] public required string MachineID { get; init; }
        [Column("_identifier", IsTag = true)] public required string Identifier { get; init; }
        [Column(IsTimestamp = true)] public required DateTime Timestamp { get; init; }
    }
    bool EnableStorage { get; set; }
    MqttServer Transport { get; init; }
    ArrayPool<byte> BytePool { get; }
    string Version { get; }
}