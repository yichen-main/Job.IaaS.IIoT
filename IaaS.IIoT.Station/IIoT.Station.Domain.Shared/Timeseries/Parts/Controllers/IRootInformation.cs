namespace Station.Domain.Shared.Timeseries.Parts.Controllers;
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