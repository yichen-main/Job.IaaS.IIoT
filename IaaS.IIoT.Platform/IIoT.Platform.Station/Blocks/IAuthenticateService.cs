namespace Platform.Station.Blocks;
public interface IAuthenticateService
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
file sealed class AuthenticateService : IAuthenticateService
{
    public bool Login(string account, string password)
    {
        if (Front.Grade.UseDecryptAES() == $"{account}{password}".ToMd5())
        {
            CreateToken();
            CreateRefreshId();
            return true;
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