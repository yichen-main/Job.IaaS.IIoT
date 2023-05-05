namespace Platform.Station.Blocks;
internal sealed class AuthenticateHandler : AuthenticationHandler<AuthenticateOption>
{
    const string _authorization = "Authorization";
    readonly IAuthenticateService _authenticateService;
    public AuthenticateHandler(
        UrlEncoder encoder,
        ISystemClock clock,
        ILoggerFactory logger,
        IOptionsMonitor<AuthenticateOption> options,
        IAuthenticateService authenticateService) : base(options, logger, encoder, clock)
    {
        _authenticateService = authenticateService;
    }
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        try
        {
            if (Request.Headers.ContainsKey(_authorization))
            {
                var result = string.Empty;
                string? header = Request?.Headers[_authorization];
                if (string.IsNullOrEmpty(header)) throw new Exception();
                if (!header.StartsWith(JwtBearerDefaults.AuthenticationScheme, StringComparison.OrdinalIgnoreCase)) throw new Exception();
                {
                    result = header[JwtBearerDefaults.AuthenticationScheme.Length..].Trim();
                    if (string.IsNullOrEmpty(result)) throw new Exception();
                }
                if (DateTime.UtcNow.Subtract(_authenticateService.CreateTime).TotalSeconds < _authenticateService.ExpiresIn)
                {
                    if (_authenticateService.Token == result) return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(new GenericPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, string.Empty)
                    }, result), roles: null), Scheme.Name)));
                }
                else _authenticateService.CreateToken();
            }
            return Task.FromResult(AuthenticateResult.NoResult());
        }
        catch (Exception e)
        {
            return Task.FromResult(AuthenticateResult.Fail(e));
        }
    }
}