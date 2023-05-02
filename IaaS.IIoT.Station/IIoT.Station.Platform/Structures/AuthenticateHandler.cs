namespace Station.Platform.Structures;
internal sealed class AuthenticateHandler : AuthenticationHandler<AuthenticateOption>
{
    const string _authorization = "Authorization";
    public AuthenticateHandler(IOptionsMonitor<AuthenticateOption> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock) { }
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
                if (DateTime.UtcNow.Subtract(AuthenticateService.CreateTime).TotalSeconds < AuthenticateService.ExpiresIn)
                {
                    if (AuthenticateService.Token == result) return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(new GenericPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, string.Empty)
                    }, result), roles: null), Scheme.Name)));
                }
                else AuthenticateService.CreateToken();
            }
            return Task.FromResult(AuthenticateResult.NoResult());
        }
        catch (Exception e)
        {
            return Task.FromResult(AuthenticateResult.Fail(e));
        }
    }
    public required IAuthenticateService AuthenticateService { get; init; }
}