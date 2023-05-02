namespace Station.Domain.Timeseries.Parts.Controllers;
internal sealed class MaintenanceCycle : DepotEngine<MaintenanceCycle.Entity>, IMaintenanceCycle
{
    readonly string _machineID;
    public MaintenanceCycle(IStructuralEngine engine, IMainProfile profile) : base(engine, profile)
    {
        _machineID = profile.Text?.MachineID ?? string.Empty;
    }
    public async Task InsertAsync(IMaintenanceCycle.Data[] datas)
    {
        if (datas.Any()) await WriteAsync(datas.Select(item => new Entity
        {
            SerialNo = item.SerialNo,
            CumulativeDay = item.CumulativeDay,
            Interval = item.Interval.GetDescription(),
            MachineID = _machineID,
            Identifier = Identifier,
            Timestamp = DateTime.UtcNow
        }).ToArray(), Bucket);
    }

    [Measurement("maintenance_cycles")]
    internal sealed class Entity : MetaBase
    {
        [Column("serial_no")] public required int SerialNo { get; init; }
        [Column("cumulative_day")] public required int CumulativeDay { get; init; }
        [Column("interval", IsTag = true)] public required string Interval { get; init; }
    }
    static string Identifier => nameof(MaintenanceCycle).ToMd5().ToLower();
    static string Bucket => ITimeserieWrapper.BucketTag.Controller.GetDescription();
}