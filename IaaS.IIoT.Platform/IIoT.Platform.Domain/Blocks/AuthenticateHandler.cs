namespace Platform.Domain.Blocks;
public sealed class AuthenticateHandler : AuthenticationHandler<AuthenticateOption>
{
    readonly IAuthenticateEngine _authenticateEngine;
    public AuthenticateHandler(
        UrlEncoder encoder,
        ISystemClock clock,
        ILoggerFactory logger,
        IOptionsMonitor<AuthenticateOption> options,
        IAuthenticateEngine authenticateEngine) : base(options, logger, encoder, clock)
    {
        _authenticateEngine = authenticateEngine;
    }
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        try
        {
            var authorization = "Authorization";
            if (Request.Headers.ContainsKey(authorization))
            {
                var result = string.Empty;
                string? header = Request?.Headers[authorization];
                if (string.IsNullOrEmpty(header)) throw new Exception();
                if (!header.StartsWith(JwtBearerDefaults.AuthenticationScheme, StringComparison.OrdinalIgnoreCase)) throw new Exception();
                {
                    result = header[JwtBearerDefaults.AuthenticationScheme.Length..].Trim();
                    if (string.IsNullOrEmpty(result)) throw new Exception();
                }
                if (DateTime.UtcNow.Subtract(_authenticateEngine.CreateTime).TotalSeconds < _authenticateEngine.ExpiresIn)
                {
                    if (_authenticateEngine.Token == result) return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(new GenericPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, string.Empty)
                    }, result), roles: null), Scheme.Name)));
                }
                else _authenticateEngine.CreateToken();
            }
            return Task.FromResult(AuthenticateResult.NoResult());
        }
        catch (Exception e)
        {
            return Task.FromResult(AuthenticateResult.Fail(e));
        }
    }
}