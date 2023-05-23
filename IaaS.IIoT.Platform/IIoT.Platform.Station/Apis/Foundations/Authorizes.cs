namespace Platform.Station.Apis.Foundations;

[Authorize(), EnableCors, ApiExplorerSettings(GroupName = nameof(Foundations)), EnableRateLimiting("Fixed")]
public class Authorizes(IBaseLoader baseLoader, IPassVerifier passVerifier) : ControllerBase
{
    const string _loginTag = "login";
    readonly IBaseLoader _baseLoader = baseLoader;
    readonly IPassVerifier _passVerifier = passVerifier;

    [AllowAnonymous, HttpPost(_loginTag, Name = nameof(InsertLogin))]
    public IActionResult InsertLogin([FromForm] InputLoginBody body)
    {
        try
        {
            if (_passVerifier.Login(body.Account, body.Password)) return Ok(new LoginRow
            {
                AccessToken = _passVerifier.Token!,
                ExpiresIn = _passVerifier.ExpiresIn,
                TokenType = JwtBearerDefaults.AuthenticationScheme,
                RefreshToken = _passVerifier.RefreshId
            });
            return Unauthorized();
        }
        catch (Exception e)
        {
            return NotFound(new { e.Message });
        }
    }

    [HttpPut(_loginTag, Name = nameof(UpdateLoginAsync))]
    public async Task<IActionResult> UpdateLoginAsync([FromForm] InputLoginBody body)
    {
        try
        {
            if (_baseLoader.Profile is not null)
            {
                _baseLoader.FileLock = true;
                _baseLoader.Profile.UserCode = $"{body.Account}{body.Password}".ToMd5().UseEncryptAES();
                await _baseLoader.OverwriteProfileAsync(_baseLoader.Profile);
            }
            return NoContent();
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
            if (_passVerifier.RefreshId == body.RefreshToken)
            {
                var token = _passVerifier.Token;
                if (token is not null)
                {
                    _passVerifier.CreateRefreshId();
                    return Ok(new LoginRow
                    {
                        AccessToken = token,
                        ExpiresIn = _passVerifier.ExpiresIn,
                        TokenType = JwtBearerDefaults.AuthenticationScheme,
                        RefreshToken = _passVerifier.RefreshId
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
    public sealed class InputLoginBody
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