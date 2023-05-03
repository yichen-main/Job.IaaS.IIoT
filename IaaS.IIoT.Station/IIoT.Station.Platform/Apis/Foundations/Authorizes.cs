namespace Station.Platform.Apis.Foundations;

[ApiExplorerSettings(GroupName = nameof(Foundations))]
public class Authorizes : ControllerBase
{
    [HttpPost("login", Name = nameof(InsertLogin))]
    public IActionResult InsertLogin([FromForm] LoginBody body)
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

    [HttpPost("extend", Name = nameof(InsertExtend))]
    public IActionResult InsertExtend([FromForm] ExtendBody body)
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
    public sealed class LoginBody
    {
        public required string Account { get; init; }
        public required string Password { get; init; }
    }
    public sealed class ExtendBody
    {
        public required Guid RefreshToken { get; init; }
    }
    public readonly record struct VerifyRow
    {
        public required string AccessToken { get; init; }
        public required int ExpiresIn { get; init; }
        public required string TokenType { get; init; }
        public required Guid RefreshToken { get; init; }
    }
    public required IAuthenticateService AuthenticateService { get; init; }
}