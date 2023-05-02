namespace Station.Platform.Apis.Managements;

[ApiExplorerSettings(GroupName = nameof(Managements))]
public class Sensors : ControllerBase
{
    [HttpGet("component-informations", Name = nameof(GetSensorComponentInformation))]
    public IActionResult GetSensorComponentInformation([FromHeader] Header header)
    {
        using (CultureHelper.Use(header.Language.ConvertFamily()))
        {
            try
            {
                var (electricalBoxHumidityValue, electricalBoxHumidityTime) = SensorPool.GetElectricalBoxHumidity();
                var (electricalBoxTemperatureValue, electricalBoxTemperatureTime) = SensorPool.GetElectricalBoxTemperature();
                var (electricalBoxAverageVoltageValue, electricalBoxAverageVoltageTime) = SensorPool.GetElectricalBoxAverageVoltage();
                var (electricalBoxAverageCurrentValue, electricalBoxAverageCurrentTime) = SensorPool.GetElectricalBoxAverageCurrent();
                var (electricalBoxApparentPowerValue, electricalBoxApparentPowerTime) = SensorPool.GetElectricalBoxApparentPower();
                var (waterTankTemperatureValue, waterTankTemperatureTime) = SensorPool.GetWaterTankTemperature();
                var (cuttingFluidTemperatureValue, cuttingFluidTemperatureTime) = SensorPool.GetCuttingFluidTemperature();
                var (cuttingFluidPotentialOfHydrogenValue, cuttingFluidPotentialOfHydrogenTime) = SensorPool.GetCuttingFluidPotentialOfHydrogen();
                var (waterPumpMotorAverageVoltageValue, waterPumpMotorAverageVoltageTime) = SensorPool.GetWaterPumpMotorAverageVoltage();
                var (waterPumpMotorAverageCurrentValue, waterPumpMotorAverageCurrentTime) = SensorPool.GetWaterPumpMotorAverageCurrent();
                var (waterPumpMotorApparentPowerValue, waterPumpMotorApparentPowerTime) = SensorPool.GetWaterPumpMotorApparentPower();
                return Ok(new
                {
                    ElectricalBoxes = new[]
                    {
                        new ComponentStatusRow
                        {
                            SerialNo = (int)ElectricalBoxType.ElectricalBoxHumidity,
                            DataName = SensorLocaliz[nameof(ElectricalBoxType.ElectricalBoxHumidity)],
                            DataValue = electricalBoxHumidityValue,
                            EventTime = electricalBoxHumidityTime == default ? string.Empty : electricalBoxHumidityTime.ToTimestamp(header.TimeFormat)
                        },
                        new ComponentStatusRow
                        {
                            SerialNo = (int)ElectricalBoxType.ElectricalBoxTemperature,
                            DataName = SensorLocaliz[nameof(ElectricalBoxType.ElectricalBoxTemperature)],
                            DataValue = electricalBoxTemperatureValue,
                            EventTime = electricalBoxTemperatureTime == default ? string.Empty : electricalBoxTemperatureTime.ToTimestamp(header.TimeFormat)
                        },
                        new ComponentStatusRow
                        {
                            SerialNo = (int)ElectricalBoxType.ElectricalBoxAverageVoltage,
                            DataName = SensorLocaliz[nameof(ElectricalBoxType.ElectricalBoxAverageVoltage)],
                            DataValue = electricalBoxAverageVoltageValue,
                            EventTime = electricalBoxAverageVoltageTime == default ? string.Empty : electricalBoxAverageVoltageTime.ToTimestamp(header.TimeFormat)
                        },
                        new ComponentStatusRow
                        {
                            SerialNo = (int)ElectricalBoxType.ElectricalBoxAverageCurrent,
                            DataName = SensorLocaliz[nameof(ElectricalBoxType.ElectricalBoxAverageCurrent)],
                            DataValue = electricalBoxAverageCurrentValue,
                            EventTime = electricalBoxAverageCurrentTime == default ? string.Empty : electricalBoxAverageCurrentTime.ToTimestamp(header.TimeFormat)
                        },
                        new ComponentStatusRow
                        {
                            SerialNo = (int)ElectricalBoxType.ElectricalBoxApparentPower,
                            DataName = SensorLocaliz[nameof(ElectricalBoxType.ElectricalBoxApparentPower)],
                            DataValue = electricalBoxApparentPowerValue,
                            EventTime = electricalBoxApparentPowerTime == default ? string.Empty : electricalBoxApparentPowerTime.ToTimestamp(header.TimeFormat)
                        }
                    },
                    WaterTanks = new[]
                    {
                        new ComponentStatusRow
                        {
                            SerialNo = (int)WaterTankType.WaterTankTemperature,
                            DataName = SensorLocaliz[nameof(WaterTankType.WaterTankTemperature)],
                            DataValue = waterTankTemperatureValue,
                            EventTime = waterTankTemperatureTime == default ? string.Empty : waterTankTemperatureTime.ToTimestamp(header.TimeFormat)
                        },
                        new ComponentStatusRow
                        {
                            SerialNo = (int)WaterTankType.CuttingFluidTemperature,
                            DataName = SensorLocaliz[nameof(WaterTankType.CuttingFluidTemperature)],
                            DataValue = cuttingFluidTemperatureValue,
                            EventTime = cuttingFluidTemperatureTime == default ? string.Empty : cuttingFluidTemperatureTime.ToTimestamp(header.TimeFormat)
                        },
                        new ComponentStatusRow
                        {
                            SerialNo = (int)WaterTankType.CuttingFluidPotentialOfHydrogen,
                            DataName = SensorLocaliz[nameof(WaterTankType.CuttingFluidPotentialOfHydrogen)],
                            DataValue = cuttingFluidPotentialOfHydrogenValue,
                            EventTime = cuttingFluidPotentialOfHydrogenTime == default ? string.Empty : cuttingFluidPotentialOfHydrogenTime.ToTimestamp(header.TimeFormat)
                        },
                        new ComponentStatusRow
                        {
                            SerialNo = (int)WaterTankType.WaterPumpMotorAverageVoltage,
                            DataName = SensorLocaliz[nameof(WaterTankType.WaterPumpMotorAverageVoltage)],
                            DataValue = waterPumpMotorAverageVoltageValue,
                            EventTime = waterPumpMotorAverageVoltageTime == default ? string.Empty : waterPumpMotorAverageVoltageTime.ToTimestamp(header.TimeFormat)
                        },
                        new ComponentStatusRow
                        {
                            SerialNo = (int)WaterTankType.WaterPumpMotorAverageCurrent,
                            DataName = SensorLocaliz[nameof(WaterTankType.WaterPumpMotorAverageCurrent)],
                            DataValue = waterPumpMotorAverageCurrentValue,
                            EventTime = waterPumpMotorAverageCurrentTime == default ? string.Empty : waterPumpMotorAverageCurrentTime.ToTimestamp(header.TimeFormat)
                        },
                        new ComponentStatusRow
                        {
                            SerialNo = (int)WaterTankType.WaterPumpMotorApparentPower,
                            DataName = SensorLocaliz[nameof(WaterTankType.WaterPumpMotorApparentPower)],
                            DataValue = waterPumpMotorApparentPowerValue,
                            EventTime = waterPumpMotorApparentPowerTime == default ? string.Empty : waterPumpMotorApparentPowerTime.ToTimestamp(header.TimeFormat)
                        }
                    }
                });
            }
            catch (Exception e)
            {
                return NotFound(new { e.Message });
            }
        }
    }

    [HttpGet("electricity-statistics", Name = nameof(GetElectricityStatistic))]
    public IActionResult GetElectricityStatistic([FromHeader] Header header)
    {
        using (CultureHelper.Use(header.Language.ConvertFamily()))
        {
            try
            {
                return Ok(new
                {
                    RunCharts = AbstractPool.ElectricityStatistics.Select(item => new
                    {
                        item.KilowattHour,
                        EventTime = item.EventTime.ToTimestamp(header.TimeFormat)
                    })
                });
            }
            catch (Exception e)
            {
                return NotFound(new { e.Message });
            }
        }
    }
    public readonly record struct ComponentStatusRow
    {
        public required int SerialNo { get; init; }
        public required string DataName { get; init; }
        public required float DataValue { get; init; }
        public required string EventTime { get; init; }
    }
    public required ISensorPool SensorPool { get; init; }
    public required IAbstractPool AbstractPool { get; init; }
    public required IStringLocalizer<Sensor> SensorLocaliz { get; init; }
}