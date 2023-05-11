namespace Platform.Station.Apis.Foundations;

[ApiExplorerSettings(GroupName = nameof(Foundations))]
public class Authorizes : ControllerBase
{
    readonly IVerifyConstructor _verifyEngine;
    public Authorizes(IVerifyConstructor verifyEngine) => _verifyEngine = verifyEngine;

    [HttpPost("login", Name = nameof(InsertLogin))]
    public IActionResult InsertLogin([FromForm] LoginBody body)
    {
        try
        {
            if (_verifyEngine.Login(body.Account, body.Password)) return Ok(new LoginRow
            {
                AccessToken = _verifyEngine.Token!,
                ExpiresIn = _verifyEngine.ExpiresIn,
                TokenType = JwtBearerDefaults.AuthenticationScheme,
                RefreshToken = _verifyEngine.RefreshId
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
            if (_verifyEngine.RefreshId == body.RefreshToken)
            {
                var token = _verifyEngine.Token;
                if (token is not null)
                {
                    _verifyEngine.CreateRefreshId();
                    return Ok(new LoginRow
                    {
                        AccessToken = token,
                        ExpiresIn = _verifyEngine.ExpiresIn,
                        TokenType = JwtBearerDefaults.AuthenticationScheme,
                        RefreshToken = _verifyEngine.RefreshId
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
    public readonly record struct LoginRow
    {
        public required string AccessToken { get; init; }
        public required int ExpiresIn { get; init; }
        public required string TokenType { get; init; }
        public required Guid RefreshToken { get; init; }
    }
}