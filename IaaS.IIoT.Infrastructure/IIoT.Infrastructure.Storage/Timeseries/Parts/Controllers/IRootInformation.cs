using static Infrastructure.Storage.Architects.Experts.IInfluxExpert;

namespace Infrastructure.Storage.Timeseries.Parts.Controllers;
public interface IRootInformation
{
    Task InsertAsync(Data data);

    [StructLayout(LayoutKind.Auto)]
    readonly record struct Data
    {
        public enum MachineStatusType
        {
            Idle = 0,
            Run = 1,
            Error = 2,
            Repair = 3
        }
        public required MachineStatusType MachineStatus { get; init; }
    }
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class RootInformation : DepotDevelop<RootInformation.Entity>, IRootInformation
{
    readonly string _machineID;
    public RootInformation(IInfluxExpert influxExpert, IMainProfile mainProfile) : base(influxExpert, mainProfile)
    {
        _machineID = mainProfile.Text?.MachineID ?? string.Empty;
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
    static string Bucket => BucketTag.Controller.GetDescription();
}