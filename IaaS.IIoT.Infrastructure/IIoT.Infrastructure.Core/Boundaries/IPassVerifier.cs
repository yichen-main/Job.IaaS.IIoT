namespace Infrastructure.Core.Boundaries;
public interface IPassVerifier
{
    bool Login(string account, string password);
    void CreateRefreshId();
    void CreateToken();
    int ExpiresIn { get; }
    string? Token { get; }
    Guid RefreshId { get; }
    DateTime CreateTime { get; }
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class PassVerifier(IBaseLoader baseLoader) : IPassVerifier
{
    readonly IBaseLoader _baseLoader = baseLoader;
    public bool Login(string account, string password)
    {
        if (_baseLoader.Profile is not null)
        {
            if (_baseLoader.Profile.UserCode.UseDecryptAES() == $"{account}{password}".ToMd5())
            {
                CreateToken();
                CreateRefreshId();
                return true;
            }
        }
        return false;
    }
    public void CreateRefreshId() => RefreshId = Guid.NewGuid();
    public void CreateToken()
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("N")));
        var result = new JwtSecurityToken(signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256));
        Token = new JwtSecurityTokenHandler().WriteToken(result);
        CreateTime = DateTime.UtcNow;
    }
    public int ExpiresIn => 1800;
    public string? Token { get; private set; }
    public Guid RefreshId { get; private set; }
    public DateTime CreateTime { get; private set; }
}