namespace Station.Platform.Apis.Managements;

[Authorize(), EnableCors, ApiExplorerSettings(GroupName = nameof(Managements))]
public class Machines : ControllerBase
{
    readonly IMitsubishiPool _mitsubishiPool;
    readonly IStringLocalizer<Fielder> _fielderLocaliz;
    public Machines(
        IMitsubishiPool mitsubishiPool,
        IStringLocalizer<Fielder> fielderLocaliz)
    {
        _mitsubishiPool = mitsubishiPool;
        _fielderLocaliz = fielderLocaliz;
    }

    [HttpGet("root-informations", Name = nameof(GetRootInformation))]
    public IActionResult GetRootInformation([FromHeader] Header header)
    {
        using (CultureHelper.Use(header.Language.ConvertFamily()))
        {
            try
            {
                var gg = _fielderLocaliz["field.value.mismatch", nameof(IMaintenanceCycle.Data.IntervalType)];
                return Ok(new
                {
                    _mitsubishiPool.MachineStatus,
                    PartGates = _mitsubishiPool.Part,
                    FixtureGates = _mitsubishiPool.Fixture,
                    AlarmGates = _mitsubishiPool.Alarm
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
                    IMaintenanceCycle.Data.IntervalType.Weekly => Ok(new { Ranges = _mitsubishiPool.MaintenanceCycle.Weeklies }),
                    IMaintenanceCycle.Data.IntervalType.Monthly => Ok(new { Ranges = _mitsubishiPool.MaintenanceCycle.Monthlies }),
                    IMaintenanceCycle.Data.IntervalType.Year => Ok(new { Ranges = _mitsubishiPool.MaintenanceCycle.Years }),
                    _ => throw new Exception(_fielderLocaliz["field.value.mismatch", nameof(IMaintenanceCycle.Data.IntervalType)])
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
}