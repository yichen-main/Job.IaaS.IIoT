namespace Platform.Station.Apis.Machines;

[EnableCors, ApiExplorerSettings(GroupName = nameof(Machines))]
public class Alarms(IBaseLoader baseLoader) : ControllerBase
{
    readonly IBaseLoader _baseLoader = baseLoader;

    [HttpGet("parts", Name = nameof(GetAlarmParts))]
    public IActionResult GetAlarmParts()
    {
        try
        {




            return Ok(new
            {
                _baseLoader.Profile?.Formulation.CarbonEmissionFactor,
                _baseLoader.Profile?.Formulation.GlobalWarmingPotential
            });
        }
        catch (Exception e)
        {
            return NotFound(new { e.Message });
        }
    }
}