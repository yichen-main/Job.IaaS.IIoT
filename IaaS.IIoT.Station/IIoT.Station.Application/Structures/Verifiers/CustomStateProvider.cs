namespace Station.Application.Structures.Verifiers;
public class CustomStateProvider : AuthenticationStateProvider
{
    readonly ProtectedSessionStorage _sessionStorage;
    readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());
    public CustomStateProvider(ProtectedSessionStorage sessionStorage) => _sessionStorage = sessionStorage;
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var userSessionStorageResult = await _sessionStorage.GetAsync<UserSession>(nameof(UserSession));
            var userSession = userSessionStorageResult.Success ? userSessionStorageResult.Value : null;
            if (userSession is null) return await Task.FromResult(new AuthenticationState(_anonymous));
            return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.Name, userSession.UserName),
                new Claim(ClaimTypes.Role, userSession.Role)
            }, "CustomAuth"))));
        }
        catch
        {
            return await Task.FromResult(new AuthenticationState(_anonymous));
        }
    }
    public async Task UpdateAuthenticationState(UserSession userSession)
    {
        ClaimsPrincipal claimsPrincipal;
        if (userSession is not null)
        {
            await _sessionStorage.SetAsync(nameof(UserSession), userSession);
            claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.Name, userSession.UserName),
                new Claim(ClaimTypes.Role, userSession.Role)
            }));
        }
        else
        {
            await _sessionStorage.DeleteAsync(nameof(UserSession));
            claimsPrincipal = _anonymous;
        }
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
    }
}