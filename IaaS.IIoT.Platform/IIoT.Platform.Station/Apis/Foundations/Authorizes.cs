using Platform.Domain.Shared.Engines;

namespace Platform.Station.Apis.Foundations;

[ApiExplorerSettings(GroupName = nameof(Foundations))]
public class Authorizes : ControllerBase
{
    readonly IAuthenticateEngine _authenticateService;
    public Authorizes(IAuthenticateEngine authenticateService)
    {
        _authenticateService = authenticateService;
    }

    [HttpPost("login", Name = nameof(InsertLogin))]
    public IActionResult InsertLogin([FromForm] LoginBody body)
    {
        try
        {
            if (_authenticateService.Login(body.Account, body.Password)) return Ok(new VerifyRow
            {
                AccessToken = _authenticateService.Token!,
                ExpiresIn = _authenticateService.ExpiresIn,
                TokenType = JwtBearerDefaults.AuthenticationScheme,
                RefreshToken = _authenticateService.RefreshId
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
            if (_authenticateService.RefreshId == body.RefreshToken)
            {
                var token = _authenticateService.Token;
                if (token is not null)
                {
                    _authenticateService.CreateRefreshId();
                    return Ok(new VerifyRow
                    {
                        AccessToken = token,
                        ExpiresIn = _authenticateService.ExpiresIn,
                        TokenType = JwtBearerDefaults.AuthenticationScheme,
                        RefreshToken = _authenticateService.RefreshId
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
}