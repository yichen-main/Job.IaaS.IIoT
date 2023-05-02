namespace Station.Application.Operators.Bottoms;
internal sealed class WriterBottom : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await new PeriodicTimer(Menu.RefreshTime).WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                if (StructuralEngine.EnableStorage) await Task.WhenAll(new[]
                {
                    TimeserieWrapper.RootInformation.InsertAsync(new()
                    {
                        MachineStatus = MitsubishiPool.MachineStatus
                    }),
                    TimeserieWrapper.MaintenanceCycle.InsertAsync(MitsubishiPool.MaintenanceCycle.Weeklies.Select(item => new IMaintenanceCycle.Data
                    {
                        SerialNo = item.SerialNo,
                        CumulativeDay = item.CumulativeDay,
                        Interval = IMaintenanceCycle.Data.IntervalType.Weekly
                    }).ToArray()),
                    TimeserieWrapper.MaintenanceCycle.InsertAsync(MitsubishiPool.MaintenanceCycle.Monthlies.Select(item => new IMaintenanceCycle.Data
                    {
                        SerialNo = item.SerialNo,
                        CumulativeDay = item.CumulativeDay,
                        Interval = IMaintenanceCycle.Data.IntervalType.Monthly
                    }).ToArray()),
                    TimeserieWrapper.MaintenanceCycle.InsertAsync(MitsubishiPool.MaintenanceCycle.Years.Select(item => new IMaintenanceCycle.Data
                    {
                        SerialNo = item.SerialNo,
                        CumulativeDay = item.CumulativeDay,
                        Interval = IMaintenanceCycle.Data.IntervalType.Year
                    }).ToArray()),
                    TimeserieWrapper.SpeedOdometer.InsertAsync(MitsubishiPool.SpindleSpeedOdometers.Select(item => new ISpeedOdometer.Data
                    {
                        SerialNo = item.SerialNo,
                        Hour = item.Hour,
                        Minute = item.Minute,
                        Second = item.Second
                    }).ToArray()),
                    TimeserieWrapper.ThermalCompensation.InsertAsync(new()
                    {
                        ThermalFirst = ComponentPool.SpindleThermalCompensation.TemperatureFirst,
                        ThermalSecond = ComponentPool.SpindleThermalCompensation.TemperatureSecond,
                        ThermalThird = ComponentPool.SpindleThermalCompensation.TemperatureThird,
                        ThermalFourth = ComponentPool.SpindleThermalCompensation.TemperatureFourth,
                        ThermalFifth = ComponentPool.SpindleThermalCompensation.TemperatureFifth,
                        CompensationX = ComponentPool.SpindleThermalCompensation.CompensationX,
                        CompensationY = ComponentPool.SpindleThermalCompensation.CompensationY,
                        CompensationZ = ComponentPool.SpindleThermalCompensation.CompensationZ
                    }),
                    TimeserieWrapper.CuttingFluidInformation.InsertAsync(new()
                    {
                        Temperature = SensorPool.GetCuttingFluidTemperature().value,
                        PowerOfHydrogen = SensorPool.GetCuttingFluidPotentialOfHydrogen().value
                    }),
                    TimeserieWrapper.PumpMotorElectricity.InsertAsync(new()
                    {
                        AverageVoltage = SensorPool.GetWaterPumpMotorAverageVoltage().value,
                        AverageCurrent = SensorPool.GetWaterPumpMotorAverageCurrent().value,
                        ApparentPower = SensorPool.GetWaterPumpMotorApparentPower().value
                    }),
                    TimeserieWrapper.ElectricMeter.InsertAsync(new()
                    {
                        AverageVoltage = SensorPool.GetElectricalBoxAverageVoltage().value,
                        AverageCurrent = SensorPool.GetElectricalBoxAverageCurrent().value,
                        ApparentPower = SensorPool.GetElectricalBoxApparentPower().value
                    }),
                    TimeserieWrapper.AttachedSensor.InsertAsync(new()
                    {
                        ElectricalBoxHumidity = SensorPool.GetElectricalBoxHumidity().value,
                        ElectricalBoxTemperature = SensorPool.GetElectricalBoxTemperature().value,
                        WaterTankTemperature = SensorPool.GetWaterTankTemperature().value
                    })
                });
                if (Histories.Any()) Histories.Clear();
                FoundationPool.PushWriterBottom(DateTime.UtcNow);
            }
            catch (Exception e)
            {
                if (!Histories.Contains(e.Message))
                {
                    Histories.Add(e.Message);
                    HistoryEngine.Record(new IHistoryEngine.FavorerPayload
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
    public required ISensorPool SensorPool { get; init; }
    public required IComponentPool ComponentPool { get; init; }
    public required IFoundationPool FoundationPool { get; init; }
    public required IMitsubishiPool MitsubishiPool { get; init; }
    public required IHistoryEngine HistoryEngine { get; init; }
    public required ITimeserieWrapper TimeserieWrapper { get; init; }
    public required IStructuralEngine StructuralEngine { get; init; }
}