namespace Station.Application.Operators.Bottoms;
internal sealed class WriterBottom : BackgroundService
{
    readonly ISensorPool _sensorPool;
    readonly IHistoryEngine _historyEngine;
    readonly IComponentPool _componentPool;
    readonly IFoundationPool _foundationPool;
    readonly IMitsubishiPool _mitsubishiPool;
    readonly ITimeserieWrapper _timeserieWrapper;
    readonly IStructuralEngine _structuralEngine;
    public WriterBottom(
        ISensorPool sensorPool, 
        IHistoryEngine historyEngine,
        IComponentPool componentPool,
        IFoundationPool foundationPool,
        IMitsubishiPool mitsubishiPool,
        ITimeserieWrapper timeserieWrapper,
        IStructuralEngine structuralEngine)
    {
        _sensorPool = sensorPool;
        _historyEngine = historyEngine;
        _componentPool = componentPool;
        _foundationPool = foundationPool;
        _mitsubishiPool = mitsubishiPool;
        _timeserieWrapper = timeserieWrapper;
        _structuralEngine = structuralEngine;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await new PeriodicTimer(Menu.RefreshTime).WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                if (_structuralEngine.EnableStorage) await Task.WhenAll(new[]
                {
                    _timeserieWrapper.RootInformation.InsertAsync(new()
                    {
                        MachineStatus = _mitsubishiPool.MachineStatus
                    }),
                    _timeserieWrapper.MaintenanceCycle.InsertAsync(_mitsubishiPool.MaintenanceCycle.Weeklies.Select(item => new IMaintenanceCycle.Data
                    {
                        SerialNo = item.SerialNo,
                        CumulativeDay = item.CumulativeDay,
                        Interval = IMaintenanceCycle.Data.IntervalType.Weekly
                    }).ToArray()),
                    _timeserieWrapper.MaintenanceCycle.InsertAsync(_mitsubishiPool.MaintenanceCycle.Monthlies.Select(item => new IMaintenanceCycle.Data
                    {
                        SerialNo = item.SerialNo,
                        CumulativeDay = item.CumulativeDay,
                        Interval = IMaintenanceCycle.Data.IntervalType.Monthly
                    }).ToArray()),
                    _timeserieWrapper.MaintenanceCycle.InsertAsync(_mitsubishiPool.MaintenanceCycle.Years.Select(item => new IMaintenanceCycle.Data
                    {
                        SerialNo = item.SerialNo,
                        CumulativeDay = item.CumulativeDay,
                        Interval = IMaintenanceCycle.Data.IntervalType.Year
                    }).ToArray()),
                    _timeserieWrapper.SpeedOdometer.InsertAsync(_mitsubishiPool.SpindleSpeedOdometers.Select(item => new ISpeedOdometer.Data
                    {
                        SerialNo = item.SerialNo,
                        Hour = item.Hour,
                        Minute = item.Minute,
                        Second = item.Second
                    }).ToArray()),
                    _timeserieWrapper.ThermalCompensation.InsertAsync(new()
                    {
                        ThermalFirst = _componentPool.SpindleThermalCompensation.TemperatureFirst,
                        ThermalSecond = _componentPool.SpindleThermalCompensation.TemperatureSecond,
                        ThermalThird = _componentPool.SpindleThermalCompensation.TemperatureThird,
                        ThermalFourth = _componentPool.SpindleThermalCompensation.TemperatureFourth,
                        ThermalFifth = _componentPool.SpindleThermalCompensation.TemperatureFifth,
                        CompensationX = _componentPool.SpindleThermalCompensation.CompensationX,
                        CompensationY = _componentPool.SpindleThermalCompensation.CompensationY,
                        CompensationZ = _componentPool.SpindleThermalCompensation.CompensationZ
                    }),
                    _timeserieWrapper.CuttingFluidInformation.InsertAsync(new()
                    {
                        Temperature = _sensorPool.GetCuttingFluidTemperature().value,
                        PowerOfHydrogen = _sensorPool.GetCuttingFluidPotentialOfHydrogen().value
                    }),
                    _timeserieWrapper.PumpMotorElectricity.InsertAsync(new()
                    {
                        AverageVoltage = _sensorPool.GetWaterPumpMotorAverageVoltage().value,
                        AverageCurrent = _sensorPool.GetWaterPumpMotorAverageCurrent().value,
                        ApparentPower = _sensorPool.GetWaterPumpMotorApparentPower().value
                    }),
                    _timeserieWrapper.ElectricMeter.InsertAsync(new()
                    {
                        AverageVoltage = _sensorPool.GetElectricalBoxAverageVoltage().value,
                        AverageCurrent = _sensorPool.GetElectricalBoxAverageCurrent().value,
                        ApparentPower = _sensorPool.GetElectricalBoxApparentPower().value
                    }),
                    _timeserieWrapper.AttachedSensor.InsertAsync(new()
                    {
                        ElectricalBoxHumidity = _sensorPool.GetElectricalBoxHumidity().value,
                        ElectricalBoxTemperature = _sensorPool.GetElectricalBoxTemperature().value,
                        WaterTankTemperature = _sensorPool.GetWaterTankTemperature().value
                    })
                });
                if (Histories.Any()) Histories.Clear();
                _foundationPool.PushWriterBottom(DateTime.UtcNow);
            }
            catch (Exception e)
            {
                if (!Histories.Contains(e.Message))
                {
                    Histories.Add(e.Message);
                    _historyEngine.Record(new IHistoryEngine.FavorerPayload
                    {
                        Name = nameof(WriterBottom),
                        Message = e.Message,
                        Source = e.Source
                    });
                }
            }
        }
    }
    internal required List<string> Histories { get; init; } = new();
}