namespace Infrastructure.Garner.Timeliness.Tacks.Bases;
public interface IOpcUaRegistrant
{
    Task InsertAsync(Data data);
    readonly record struct Data
    {
        public required SessionEventReason Status { get; init; }
        public required string SessionName { get; init; }
    }
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class OpcUaRegistrant(IInfluxExpert influxExpert) : IOpcUaRegistrant
{
    readonly IInfluxExpert _influxExpert = influxExpert;
    public async Task InsertAsync(IOpcUaRegistrant.Data data) => await _influxExpert.WriteAsync(new Entity
    {
        Status = (byte)data.Status,
        SessionName = data.SessionName,
        Identifier = Identifier,
        Timestamp = DateTime.UtcNow
    }, Bucket);

    [Measurement("opc_ua_registrants")]
    sealed class Entity : IInfluxExpert.MetaBase
    {
        [Column("status")] public required byte Status { get; init; }
        [Column("session_name", IsTag = true)] public required string SessionName { get; init; }
    }
    static string Identifier => nameof(OpcUaRegistrant).ToMd5().ToLower();
    static string Bucket => IInfluxExpert.BucketTag.BaseMachine.GetDESC();
}