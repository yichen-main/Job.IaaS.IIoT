namespace Station.Platform.Apis.Managements;

[ApiExplorerSettings(GroupName = nameof(Managements))]
public class Sensors : ControllerBase
{
    readonly ISensorPool _sensorPool;
    readonly IAbstractPool _abstractPool;
    readonly IStringLocalizer<Sensor> _sensorLocaliz;
    public Sensors(
        ISensorPool sensorPool,
        IAbstractPool abstractPool,
        IStringLocalizer<Sensor> sensorLocaliz)
    {
        _sensorPool = sensorPool;
        _abstractPool = abstractPool;
        _sensorLocaliz = sensorLocaliz;
    }

    [HttpGet("component-informations", Name = nameof(GetSensorComponentInformation))]
    public IActionResult GetSensorComponentInformation([FromHeader] Header header)
    {
        using (CultureHelper.Use(header.Language.ConvertFamily()))
        {
            try
            {
                var (electricalBoxHumidityValue, electricalBoxHumidityTime) = _sensorPool.GetElectricalBoxHumidity();
                var (electricalBoxTemperatureValue, electricalBoxTemperatureTime) = _sensorPool.GetElectricalBoxTemperature();
                var (electricalBoxAverageVoltageValue, electricalBoxAverageVoltageTime) = _sensorPool.GetElectricalBoxAverageVoltage();
                var (electricalBoxAverageCurrentValue, electricalBoxAverageCurrentTime) = _sensorPool.GetElectricalBoxAverageCurrent();
                var (electricalBoxApparentPowerValue, electricalBoxApparentPowerTime) = _sensorPool.GetElectricalBoxApparentPower();
                var (waterTankTemperatureValue, waterTankTemperatureTime) = _sensorPool.GetWaterTankTemperature();
                var (cuttingFluidTemperatureValue, cuttingFluidTemperatureTime) = _sensorPool.GetCuttingFluidTemperature();
                var (cuttingFluidPotentialOfHydrogenValue, cuttingFluidPotentialOfHydrogenTime) = _sensorPool.GetCuttingFluidPotentialOfHydrogen();
                var (waterPumpMotorAverageVoltageValue, waterPumpMotorAverageVoltageTime) = _sensorPool.GetWaterPumpMotorAverageVoltage();
                var (waterPumpMotorAverageCurrentValue, waterPumpMotorAverageCurrentTime) = _sensorPool.GetWaterPumpMotorAverageCurrent();
                var (waterPumpMotorApparentPowerValue, waterPumpMotorApparentPowerTime) = _sensorPool.GetWaterPumpMotorApparentPower();
                return Ok(new
                {
                    ElectricalBoxes = new[]
                    {
                        new ComponentStatusRow
                        {
                            SerialNo = (int)ElectricalBoxType.ElectricalBoxHumidity,
                            DataName = _sensorLocaliz[nameof(ElectricalBoxType.ElectricalBoxHumidity)],
                            DataValue = electricalBoxHumidityValue,
                            EventTime = electricalBoxHumidityTime == default ? string.Empty : electricalBoxHumidityTime.ToTimestamp(header.TimeFormat)
                        },
                        new ComponentStatusRow
                        {
                            SerialNo = (int)ElectricalBoxType.ElectricalBoxTemperature,
                            DataName = _sensorLocaliz[nameof(ElectricalBoxType.ElectricalBoxTemperature)],
                            DataValue = electricalBoxTemperatureValue,
                            EventTime = electricalBoxTemperatureTime == default ? string.Empty : electricalBoxTemperatureTime.ToTimestamp(header.TimeFormat)
                        },
                        new ComponentStatusRow
                        {
                            SerialNo = (int)ElectricalBoxType.ElectricalBoxAverageVoltage,
                            DataName = _sensorLocaliz[nameof(ElectricalBoxType.ElectricalBoxAverageVoltage)],
                            DataValue = electricalBoxAverageVoltageValue,
                            EventTime = electricalBoxAverageVoltageTime == default ? string.Empty : electricalBoxAverageVoltageTime.ToTimestamp(header.TimeFormat)
                        },
                        new ComponentStatusRow
                        {
                            SerialNo = (int)ElectricalBoxType.ElectricalBoxAverageCurrent,
                            DataName = _sensorLocaliz[nameof(ElectricalBoxType.ElectricalBoxAverageCurrent)],
                            DataValue = electricalBoxAverageCurrentValue,
                            EventTime = electricalBoxAverageCurrentTime == default ? string.Empty : electricalBoxAverageCurrentTime.ToTimestamp(header.TimeFormat)
                        },
                        new ComponentStatusRow
                        {
                            SerialNo = (int)ElectricalBoxType.ElectricalBoxApparentPower,
                            DataName = _sensorLocaliz[nameof(ElectricalBoxType.ElectricalBoxApparentPower)],
                            DataValue = electricalBoxApparentPowerValue,
                            EventTime = electricalBoxApparentPowerTime == default ? string.Empty : electricalBoxApparentPowerTime.ToTimestamp(header.TimeFormat)
                        }
                    },
                    WaterTanks = new[]
                    {
                        new ComponentStatusRow
                        {
                            SerialNo = (int)WaterTankType.WaterTankTemperature,
                            DataName = _sensorLocaliz[nameof(WaterTankType.WaterTankTemperature)],
                            DataValue = waterTankTemperatureValue,
                            EventTime = waterTankTemperatureTime == default ? string.Empty : waterTankTemperatureTime.ToTimestamp(header.TimeFormat)
                        },
                        new ComponentStatusRow
                        {
                            SerialNo = (int)WaterTankType.CuttingFluidTemperature,
                            DataName = _sensorLocaliz[nameof(WaterTankType.CuttingFluidTemperature)],
                            DataValue = cuttingFluidTemperatureValue,
                            EventTime = cuttingFluidTemperatureTime == default ? string.Empty : cuttingFluidTemperatureTime.ToTimestamp(header.TimeFormat)
                        },
                        new ComponentStatusRow
                        {
                            SerialNo = (int)WaterTankType.CuttingFluidPotentialOfHydrogen,
                            DataName = _sensorLocaliz[nameof(WaterTankType.CuttingFluidPotentialOfHydrogen)],
                            DataValue = cuttingFluidPotentialOfHydrogenValue,
                            EventTime = cuttingFluidPotentialOfHydrogenTime == default ? string.Empty : cuttingFluidPotentialOfHydrogenTime.ToTimestamp(header.TimeFormat)
                        },
                        new ComponentStatusRow
                        {
                            SerialNo = (int)WaterTankType.WaterPumpMotorAverageVoltage,
                            DataName = _sensorLocaliz[nameof(WaterTankType.WaterPumpMotorAverageVoltage)],
                            DataValue = waterPumpMotorAverageVoltageValue,
                            EventTime = waterPumpMotorAverageVoltageTime == default ? string.Empty : waterPumpMotorAverageVoltageTime.ToTimestamp(header.TimeFormat)
                        },
                        new ComponentStatusRow
                        {
                            SerialNo = (int)WaterTankType.WaterPumpMotorAverageCurrent,
                            DataName = _sensorLocaliz[nameof(WaterTankType.WaterPumpMotorAverageCurrent)],
                            DataValue = waterPumpMotorAverageCurrentValue,
                            EventTime = waterPumpMotorAverageCurrentTime == default ? string.Empty : waterPumpMotorAverageCurrentTime.ToTimestamp(header.TimeFormat)
                        },
                        new ComponentStatusRow
                        {
                            SerialNo = (int)WaterTankType.WaterPumpMotorApparentPower,
                            DataName = _sensorLocaliz[nameof(WaterTankType.WaterPumpMotorApparentPower)],
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
                    RunCharts = _abstractPool.ElectricityStatistics.Select(item => new
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
}