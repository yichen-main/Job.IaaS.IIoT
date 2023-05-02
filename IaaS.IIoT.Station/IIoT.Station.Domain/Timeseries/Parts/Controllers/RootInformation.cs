namespace Station.Domain.Timeseries.Parts.Controllers;
internal sealed class RootInformation : DepotEngine<RootInformation.Entity>, IRootInformation
{
    readonly string _machineID;
    public RootInformation(IStructuralEngine engine, IMainProfile profile) : base(engine, profile)
    {
        _machineID = profile.Text?.MachineID ?? string.Empty;
    }
    public async Task InsertAsync(IRootInformation.Data data) => await WriteAsync(new Entity
    {
        MachineStatus = (byte)data.MachineStatus,
        MachineID = _machineID,
        Identifier = Identifier,
        Timestamp = DateTime.UtcNow
    }, Bucket);

    [Measurement("root_informations")]
    internal sealed class Entity : MetaBase
    {
        [Column("machine_status")] public required byte MachineStatus { get; init; }
    }
    static string Identifier => nameof(RootInformation).ToMd5().ToLower();
    static string Bucket => ITimeserieWrapper.BucketTag.Controller.GetDescription();
}