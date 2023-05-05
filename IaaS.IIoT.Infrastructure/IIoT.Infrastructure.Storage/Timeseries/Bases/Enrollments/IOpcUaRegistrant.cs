using static Infrastructure.Storage.Architects.Experts.IInfluxExpert;

namespace Infrastructure.Storage.Timeseries.Bases.Enrollments;
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
file sealed class OpcUaRegistrant : DepotDevelop<OpcUaRegistrant.Entity>, IOpcUaRegistrant
{
    readonly string _machineID;
    public OpcUaRegistrant(IInfluxExpert influxExpert, IMainProfile mainProfile) : base(influxExpert, mainProfile)
    {
        _machineID = mainProfile.Text?.MachineID ?? string.Empty;
    }
    public async Task InsertAsync(IOpcUaRegistrant.Data data) => await WriteAsync(new Entity
    {
        Status = (byte)data.Status,
        SessionName = data.SessionName,
        MachineID = _machineID,
        Identifier = Identifier,
        Timestamp = DateTime.UtcNow
    }, Bucket);

    [Measurement("opc_ua_registrants")]
    internal sealed class Entity : MetaBase
    {
        [Column("status")] public required byte Status { get; init; }
        [Column("session_name", IsTag = true)] public required string SessionName { get; init; }
    }
    static string Identifier => nameof(OpcUaRegistrant).ToMd5().ToLower();
    static string Bucket => BucketTag.Enrollment.GetDescription();
}