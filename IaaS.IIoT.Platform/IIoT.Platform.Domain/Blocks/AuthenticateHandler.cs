namespace Platform.Domain.Blocks;
public sealed class AuthenticateHandler : AuthenticationHandler<AuthenticateOption>
{
    readonly IVerifyConstructor _verifyEngine;
    public AuthenticateHandler(
        UrlEncoder urlEncoder,
        ISystemClock systemClock,
        ILoggerFactory loggerFactory,
        IOptionsMonitor<AuthenticateOption> authenticateOption,
        IVerifyConstructor verifyEngine) : base(authenticateOption, loggerFactory, urlEncoder, systemClock)
    {
        _verifyEngine = verifyEngine;
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
                if (DateTime.UtcNow.Subtract(_verifyEngine.CreateTime).TotalSeconds < _verifyEngine.ExpiresIn)
                {
                    if (_verifyEngine.Token == result) return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(new GenericPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, string.Empty)
                    }, result), roles: null), Scheme.Name)));
                }
                else _verifyEngine.CreateToken();
            }
            return Task.FromResult(AuthenticateResult.NoResult());
        }
        catch (Exception e)
        {
            return Task.FromResult(AuthenticateResult.Fail(e));
        }
    }
}