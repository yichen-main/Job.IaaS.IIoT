namespace Platform.Station.Apis.Foundations;

[ApiExplorerSettings(GroupName = nameof(Foundations))]
public class Settings : ControllerBase
{
    const string _carbonEmissionTag = "carbon-emission";

    readonly IBaseLoader _baseLoader;
    public Settings(IBaseLoader baseLoader) => _baseLoader = baseLoader;

    [HttpGet(_carbonEmissionTag, Name = nameof(GetCarbonEmission))]
    public IActionResult GetCarbonEmission()
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

    [HttpPut(_carbonEmissionTag, Name = nameof(UpdateCarbonEmissionAsync))]
    public async Task<IActionResult> UpdateCarbonEmissionAsync([FromBody] CarbonEmissionBody body)
    {
        try
        {
            if (_baseLoader.Profile is not null)
            {
                _baseLoader.FileLock = true;
                _baseLoader.Profile.Formulation.CarbonEmissionFactor = body.CarbonEmissionFactor;
                _baseLoader.Profile.Formulation.GlobalWarmingPotential = body.GlobalWarmingPotential;
                await _baseLoader.OverwriteProfileAsync(_baseLoader.Profile);
            }
            return NoContent();
        }
        catch (Exception e)
        {
            return NotFound(new { e.Message });
        }
    }
    public sealed class CarbonEmissionBody
    {
        public required double CarbonEmissionFactor { get; init; }
        public required int GlobalWarmingPotential { get; init; }
    }
}