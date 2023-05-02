namespace Station.Platform.Apis.Components;

[ApiExplorerSettings(GroupName = nameof(Components))]
public class Spindles : ControllerBase
{
    [HttpGet("speed-odometers", Name = nameof(GetSpindleSpeedOdometer))]
    public IActionResult GetSpindleSpeedOdometer([FromHeader] Header header)
    {
        using (CultureHelper.Use(header.Language.ConvertFamily()))
        {
            try
            {
                return Ok(new { Ranges = MitsubishiPool.SpindleSpeedOdometers });
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
                    AbstractPool.SpindleThermalCompensationChart.TemperatureFirst,
                    AbstractPool.SpindleThermalCompensationChart.TemperatureSecond,
                    AbstractPool.SpindleThermalCompensationChart.TemperatureThird,
                    AbstractPool.SpindleThermalCompensationChart.TemperatureFourth,
                    AbstractPool.SpindleThermalCompensationChart.TemperatureFifth,
                    RunCharts = AbstractPool.SpindleThermalCompensationChart.RunCharts.Select(item => new
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
    public required IAbstractPool AbstractPool { get; init; }
    public required IMitsubishiPool MitsubishiPool { get; init; }
}