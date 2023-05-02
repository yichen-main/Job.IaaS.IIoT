namespace Station.Application.Structures.Verifiers;
public class UserAccountService
{
    readonly List<UserAccount> _users;
    public UserAccountService()
    {
        _users = new List<UserAccount>
        {
            new UserAccount{ UserName = "admin", Password = "admin", Role = "Administrator" },
            new UserAccount{ UserName = "user", Password = "user", Role = "User" }
        };
    }
    public UserAccount? GetByUserName(string? userName) => _users.Find(item => item.UserName == userName);
}