namespace Station.Domain.Timeseries.Bases.Enrollments;
internal sealed class OpcUaRegistrant : DepotEngine<OpcUaRegistrant.Entity>, IOpcUaRegistrant
{
    readonly string _machineID;
    public OpcUaRegistrant(IStructuralEngine engine, IMainProfile profile) : base(engine, profile)
    {
        _machineID = profile.Text?.MachineID ?? string.Empty;
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
    static string Bucket => ITimeserieWrapper.BucketTag.Enrollment.GetDescription();
}