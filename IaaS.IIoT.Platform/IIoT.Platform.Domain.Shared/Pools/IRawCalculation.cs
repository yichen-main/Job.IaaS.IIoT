﻿using static Platform.Domain.Shared.Pools.IRawCalculation;

namespace Platform.Domain.Shared.Pools;
public interface IRawCalculation
{
    void SstatisticsUnitDay();
    readonly record struct StatisticalUnitDayEntity
    {
        public required byte Availability { get; init; }
        public required IEnumerable<RunChartMinute> RunChartMinutes { get; init; }
        public readonly record struct RunChartMinute
        {
            public required string MachineStatus { get; init; }
            public required string Timestamp { get; init; }
        }
    }
    StatisticalUnitDayEntity StatisticalUnitDay { get; }
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class RawCalculation : IRawCalculation
{
    readonly IBaseLoader _baseLoader;
    readonly ITimelineWrapper _timelineWrapper;
    public RawCalculation(IBaseLoader baseLoader, ITimelineWrapper timelineWrapper)
    {
        _baseLoader = baseLoader;
        _timelineWrapper = timelineWrapper;
    }
    public void SstatisticsUnitDay()
    {
        List<StatisticalUnitDayEntity.RunChartMinute> runChartMinutes = new();
        foreach (var (status, time) in _timelineWrapper.RootInformation.OneDayMachineStatusMinutes())
        {
            runChartMinutes.Add(new()
            {
                MachineStatus = status switch
                {
                    (byte)IRootInformation.MachineStatus.Idle => nameof(IRootInformation.MachineStatus.Idle),
                    (byte)IRootInformation.MachineStatus.Run => nameof(IRootInformation.MachineStatus.Run),
                    (byte)IRootInformation.MachineStatus.Error => nameof(IRootInformation.MachineStatus.Error),
                    _ => nameof(IRootInformation.MachineStatus.Shutdown)
                },
                Timestamp = time.ToTimestamp(_baseLoader.GetTimeZone(), "MM/dd HH:mm")
            });
        }
        StatisticalUnitDay = new StatisticalUnitDayEntity
        {
            Availability = _timelineWrapper.RootInformation.MachineAvailability(),
            RunChartMinutes = runChartMinutes
        };
    }
    public StatisticalUnitDayEntity StatisticalUnitDay { get; private set; }
}