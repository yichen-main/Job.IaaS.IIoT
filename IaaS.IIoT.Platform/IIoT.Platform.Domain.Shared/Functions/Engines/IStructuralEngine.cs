namespace Platform.Domain.Shared.Functions.Engines;
public interface IStructuralEngine
{
    MqttServer Transport { get; init; }
    ArrayPool<byte> BytePool { get; }
    string Version { get; }
}