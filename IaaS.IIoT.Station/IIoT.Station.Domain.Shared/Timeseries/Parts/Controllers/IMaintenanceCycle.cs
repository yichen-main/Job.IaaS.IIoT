namespace Station.Domain.Shared.Timeseries.Parts.Controllers;
public interface IMaintenanceCycle
{
    Task InsertAsync(Data[] datas);

    [StructLayout(LayoutKind.Auto)]
    readonly record struct Data
    {
        public enum IntervalType
        {
            [Description("weekly")] Weekly = 1,
            [Description("monthly")] Monthly = 2,
            [Description("year")] Year = 3
        }
        public required IntervalType Interval { get; init; }
        public required int SerialNo { get; init; }
        public required int CumulativeDay { get; init; }
    }
}