namespace Station.Platform.Apis.Managements;

[Authorize(), EnableCors, ApiExplorerSettings(GroupName = nameof(Managements))]
public class Machines : ControllerBase
{
    [HttpGet("root-informations", Name = nameof(GetRootInformation))]
    public IActionResult GetRootInformation([FromHeader] Header header)
    {
        using (CultureHelper.Use(header.Language.ConvertFamily()))
        {
            try
            {
                return Ok(new
                {
                    MitsubishiPool.MachineStatus,
                    PartGates = MitsubishiPool.Part,
                    FixtureGates = MitsubishiPool.Fixture,
                    AlarmGates = MitsubishiPool.Alarm
                });
            }
            catch (Exception e)
            {
                return NotFound(new { e.Message });
            }
        }
    }

    [HttpGet("maintenance-cycles", Name = nameof(GetMaintenanceCycle))]
    public IActionResult GetMaintenanceCycle([FromHeader] Header header, [FromQuery] Query query)
    {
        using (CultureHelper.Use(header.Language.ConvertFamily()))
        {
            try
            {
                return query.Cycle switch
                {
                    IMaintenanceCycle.Data.IntervalType.Weekly => Ok(new { Ranges = MitsubishiPool.MaintenanceCycle.Weeklies }),
                    IMaintenanceCycle.Data.IntervalType.Monthly => Ok(new { Ranges = MitsubishiPool.MaintenanceCycle.Monthlies }),
                    IMaintenanceCycle.Data.IntervalType.Year => Ok(new { Ranges = MitsubishiPool.MaintenanceCycle.Years }),
                    _ => throw new Exception(FielderLocaliz["field.value.mismatch", nameof(IMaintenanceCycle.Data.IntervalType)])
                };
            }
            catch (Exception e)
            {
                return NotFound(new { e.Message });
            }
        }
    }
    public sealed class Query
    {
        public required IMaintenanceCycle.Data.IntervalType Cycle { get; init; }
    }
    public required IMitsubishiPool MitsubishiPool { get; init; }
    public required IStringLocalizer<Fielder> FielderLocaliz { get; init; }
}