﻿using static Station.Domain.Shared.Functions.Pools.IAbstractPool;

namespace Station.Domain.Functions.Pools;
internal sealed class AbstractPool : IAbstractPool
{
    public async Task SetElectricityStatisticAsync() => await Task.Run(() => ElectricityStatistics = TimeserieWrapper.ElectricMeter.ReadElectricityStatistic());
    public async Task SetSpindleSpeedOdometerChartAsync() => await Task.Run(() =>
    {
        var endTime = DateTime.UtcNow.ToNowHour();
        var startTime = endTime.AddDays(-7);
        var intervalTime = endTime.AddDays(Timeout.Infinite);
        List<(int serialNo, SpindleSpeedOdometerChartData.Detail detail)> details = new();
        while (startTime <= intervalTime)
        {
            foreach (var groups in TimeserieWrapper.SpeedOdometer.LatestTimeGroup(intervalTime, intervalTime.AddDays(1)))
            {
                details.Add((groups.Key, groups.Value));
            }
            intervalTime = intervalTime.AddDays(Timeout.Infinite);
        }
        List<SpindleSpeedOdometerChartData> results = new();
        foreach (var detail in details.GroupBy(item => item.serialNo).ToDictionary(item => item.Key, item => item.Select(item => item.detail)))
        {
            var pool = MitsubishiPool.SpindleSpeedOdometers.FirstOrDefault(item => item.SerialNo == detail.Key);
            results.Add(new()
            {
                SerialNo = pool.SerialNo,
                Description = pool.Description,
                TotalHour = pool.Hour,
                TotalMinute = pool.Minute,
                TotalSecond = pool.Second,
                Details = detail.Value
            });
        }
        foreach (var result in results)
        {

        }
        SpindleSpeedOdometerCharts = results;
    });
    public async Task SetSpindleThermalCompensationChartAsync() => await Task.Run(() => SpindleThermalCompensationChart = TimeserieWrapper.ThermalCompensation.ReadStatisticChart());
    public required IMitsubishiPool MitsubishiPool { get; init; }
    public required ITimeserieWrapper TimeserieWrapper { get; init; }
    public SpindleThermalCompensationChartData SpindleThermalCompensationChart { get; private set; } = new();
    public IEnumerable<ElectricityStatisticData> ElectricityStatistics { get; private set; } = Array.Empty<ElectricityStatisticData>();
    public IEnumerable<SpindleSpeedOdometerChartData> SpindleSpeedOdometerCharts { get; private set; } = Array.Empty<SpindleSpeedOdometerChartData>();
}