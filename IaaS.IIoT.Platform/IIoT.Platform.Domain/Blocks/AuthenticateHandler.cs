namespace Platform.Domain.Blocks;
public sealed class AuthenticateHandler : AuthenticationHandler<AuthenticateHandler.Option>
{
    readonly IPassVerifier _passVerifier;
    public AuthenticateHandler(
        UrlEncoder urlEncoder,
        ISystemClock systemClock,
        ILoggerFactory loggerFactory,
        IOptionsMonitor<Option> authenticateOption,
        IPassVerifier passVerifier) : base(authenticateOption, loggerFactory, urlEncoder, systemClock)
    {
        _passVerifier = passVerifier;
    }
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        try
        {
            var sign = "Authorization";
            if (Request.Headers.ContainsKey(sign))
            {
                var result = string.Empty;
                string? header = Request?.Headers[sign];
                if (string.IsNullOrEmpty(header)) throw new Exception();
                if (!header.StartsWith(JwtBearerDefaults.AuthenticationScheme, StringComparison.OrdinalIgnoreCase)) throw new Exception();
                {
                    result = header[JwtBearerDefaults.AuthenticationScheme.Length..].Trim();
                    if (string.IsNullOrEmpty(result)) throw new Exception();
                }
                if (DateTime.UtcNow.Subtract(_passVerifier.CreateTime).TotalSeconds < _passVerifier.ExpiresIn)
                {
                    if (_passVerifier.Token == result) return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(new GenericPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, string.Empty)
                    }, result), roles: null), Scheme.Name)));
                }
                else _passVerifier.CreateToken();
            }
            return Task.FromResult(AuthenticateResult.NoResult());
        }
        catch (Exception e)
        {
            return Task.FromResult(AuthenticateResult.Fail(e));
        }
    }
    public sealed class Option : AuthenticationSchemeOptions
    {

    }
}