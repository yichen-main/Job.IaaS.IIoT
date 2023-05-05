namespace Station.Platform.Apis.Foundations;

[ApiExplorerSettings(GroupName = nameof(Foundations))]
public class Systems : ControllerBase
{
    readonly IFoundationPool _foundationPool;
    public Systems(IFoundationPool foundationPool)
    {
        _foundationPool = foundationPool;
    }

    [HttpGet("structural-informations", Name = nameof(GetSystemStructuralInformation))]
    public IActionResult GetSystemStructuralInformation([FromHeader] Header header)
    {
        using (CultureHelper.Use(header.Language.ConvertFamily()))
        {
            try
            {
                return Ok(new
                {
                    Runner = new
                    {
                        Host = _foundationPool.CourierBottom.ToTimestamp(header.TimeFormat),
                        Shell = _foundationPool.ShellerBottom.ToTimestamp(header.TimeFormat),
                        Reader = _foundationPool.ReaderBottom.ToTimestamp(header.TimeFormat),
                        Writer = _foundationPool.WriterBottom.ToTimestamp(header.TimeFormat)
                    }
                });
            }
            catch (Exception e)
            {
                return NotFound(new { e.Message });
            }
        }
    }

    [HttpPut("settings", Name = nameof(UpdateSetting))]
    public IActionResult UpdateSetting()
    {
        try
        {
            return NoContent();
        }
        catch (Exception e)
        {
            return NotFound(new { e.Message });
        }
    }
}