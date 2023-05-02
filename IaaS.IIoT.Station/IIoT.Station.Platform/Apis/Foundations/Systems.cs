namespace Station.Platform.Apis.Foundations;

[ApiExplorerSettings(GroupName = nameof(Foundations))]
public class Systems : ControllerBase
{
    [HttpPost("authentications", Name = nameof(Authentication))]
    public IActionResult Authentication([FromForm] VerifyInsert body)
    {
        try
        {
            if (AuthenticateService.Login(body.Account, body.Password)) return Ok(new VerifyRow
            {
                AccessToken = AuthenticateService.Token!,
                ExpiresIn = AuthenticateService.ExpiresIn,
                TokenType = JwtBearerDefaults.AuthenticationScheme,
                RefreshToken = AuthenticateService.RefreshId
            });
            return Unauthorized();
        }
        catch (Exception e)
        {
            return NotFound(new { e.Message });
        }
    }

    [HttpPost("connect-token", Name = nameof(ConnectToken))]
    public IActionResult ConnectToken([FromForm] VerifyInsert body)
    {
        try
        {
            if (AuthenticateService.RefreshId == body.RefreshToken)
            {
                var token = AuthenticateService.Token;
                if (token is not null)
                {
                    AuthenticateService.CreateRefreshId();
                    return Ok(new VerifyRow
                    {
                        AccessToken = token,
                        ExpiresIn = AuthenticateService.ExpiresIn,
                        TokenType = JwtBearerDefaults.AuthenticationScheme,
                        RefreshToken = AuthenticateService.RefreshId
                    });
                }
            }
            return Forbid();
        }
        catch (Exception e)
        {
            return NotFound(new { e.Message });
        }
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
    public sealed class VerifyInsert
    {
        public required string Account { get; init; }
        public required string Password { get; init; }
        public required Guid RefreshToken { get; init; }
    }
    public readonly record struct VerifyRow
    {
        public required string AccessToken { get; init; }
        public required int ExpiresIn { get; init; }
        public required string TokenType { get; init; }
        public required Guid RefreshToken { get; init; }
    }
    public required IFoundationPool FoundationPool { get; init; }
    public required IAuthenticateService AuthenticateService { get; init; }
}