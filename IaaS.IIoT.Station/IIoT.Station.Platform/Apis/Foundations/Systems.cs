namespace Station.Platform.Apis.Foundations;

[ApiExplorerSettings(GroupName = nameof(Foundations))]
public class Systems : ControllerBase
{
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
                        Host = FoundationPool.CourierBottom.ToTimestamp(header.TimeFormat),
                        Shell = FoundationPool.ShellerBottom.ToTimestamp(header.TimeFormat),
                        Reader = FoundationPool.ReaderBottom.ToTimestamp(header.TimeFormat),
                        Writer = FoundationPool.WriterBottom.ToTimestamp(header.TimeFormat)
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
    public required IFoundationPool FoundationPool { get; init; }
}