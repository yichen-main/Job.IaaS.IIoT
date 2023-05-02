namespace Station.Domain.Shared.Timeseries.Parts.Controllers;
public interface ISpeedOdometer
{
    Task InsertAsync(Data[] datas);
    IDictionary<int, IAbstractPool.SpindleSpeedOdometerChartData.Detail> LatestTimeGroup(DateTime startTime, DateTime endTime);

    [StructLayout(LayoutKind.Auto)]
    readonly record struct Data
    {
        public required int SerialNo { get; init; }
        public required int Hour { get; init; }
        public required int Minute { get; init; }
        public required int Second { get; init; }
    }
}