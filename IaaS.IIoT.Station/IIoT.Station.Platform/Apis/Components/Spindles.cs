namespace Station.Platform.Apis.Components;

[ApiExplorerSettings(GroupName = nameof(Components))]
public class Spindles : ControllerBase
{
    readonly IAbstractPool _abstractPool;
    readonly IMitsubishiPool _mitsubishiPool;
    public Spindles(IAbstractPool abstractPool, IMitsubishiPool mitsubishiPool)
    {
        _abstractPool = abstractPool;
        _mitsubishiPool = mitsubishiPool;
    }

    [HttpGet("speed-odometers", Name = nameof(GetSpindleSpeedOdometer))]
    public IActionResult GetSpindleSpeedOdometer([FromHeader] Header header)
    {
        using (CultureHelper.Use(header.Language.ConvertFamily()))
        {
            try
            {
                return Ok(new { Ranges = _mitsubishiPool.SpindleSpeedOdometers });
                //return Ok(new
                //{
                //    Ranges = AbstractPool.SpindleSpeedOdometerCharts.Select(item => new
                //    {
                //        item.SerialNo,
                //        item.TotalHour,
                //        item.TotalMinute,
                //        item.TotalSecond,
                //        item.Description,
                //        Details = item.Details.Select(detail => new
                //        {
                //            detail.Hour,
                //            detail.Minute,
                //            detail.Second,
                //            EventTime = detail.EventTime.ToTimestamp(header.DatetimeFormat)
                //        }),
                //    })
                //});
            }
            catch (Exception e)
            {
                return NotFound(new { e.Message });
            }
        }
    }

    [HttpGet("thermal-compensations", Name = nameof(GetSpindleThermalCompensation))]
    public IActionResult GetSpindleThermalCompensation([FromHeader] Header header)
    {
        using (CultureHelper.Use(header.Language.ConvertFamily()))
        {
            try
            {
                return Ok(new
                {
                    _abstractPool.SpindleThermalCompensationChart.TemperatureFirst,
                    _abstractPool.SpindleThermalCompensationChart.TemperatureSecond,
                    _abstractPool.SpindleThermalCompensationChart.TemperatureThird,
                    _abstractPool.SpindleThermalCompensationChart.TemperatureFourth,
                    _abstractPool.SpindleThermalCompensationChart.TemperatureFifth,
                    RunCharts = _abstractPool.SpindleThermalCompensationChart.RunCharts.Select(item => new
                    {
                        item.XAxis,
                        item.YAxis,
                        item.ZAxis,
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
}