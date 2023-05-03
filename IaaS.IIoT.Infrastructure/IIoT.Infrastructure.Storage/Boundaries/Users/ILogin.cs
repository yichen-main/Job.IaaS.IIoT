using static Infrastructure.Storage.Boundaries.Users.ILogin;
using DependencyAttribute = Volo.Abp.DependencyInjection.DependencyAttribute;

namespace Infrastructure.Storage.Boundaries.Users;
public interface ILogin
{
    const string Type = "user";
    ValueTask InstallAsync();
    ValueTask AddAsync(Entity entity);
    ValueTask UpdateAsync(Entity entity);
    Task<Entity> GetAsync(Guid id);
    Task<Entity> GetAsync(string account);
    Task<IEnumerable<Entity>> ListAsync();
    Task<IEnumerable<Entity>> ListAsync(Group genre);
    enum Group
    {
        IIoTPlatform = 101
    }
    enum Authority
    {
        Operator = 101,
        Manager = 102,
        Administrator = 103
    }
    enum Operate
    {
        Enable = 101,
        Disable = 102
    }

    [Table(Name = $"{TacticDevelop.Deputy.Manage}_{Type}_{TacticDevelop.Deputy.Foundation}")]
    readonly record struct Entity
    {
        [Field(Name = CurrentSign, PK = true)] public required Guid Id { get; init; }
        [Field(Name = "group_type")] public required Group GroupType { get; init; }
        [Field(Name = "license_type")] public required Authority LicenseType { get; init; }
        [Field(Name = "operate_type")] public required Operate OperateType { get; init; }
        [Field(Name = "account")] public required string Account { get; init; }
        [Field(Name = "username")] public required string Username { get; init; }
        [Field(Name = "password")] public required string Password { get; init; }
        [Field(Name = "creator")] public required string Creator { get; init; }
        [Field(Name = "create_time")] public required DateTime CreateTime { get; init; }
    }
    string TableName { get; init; }
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class Login : TacticDevelop, ILogin
{
    public async ValueTask InstallAsync()
    {
        if (!await ExistTableAsync(TableName))
        {
            await ExecuteAsync(NpgsqlExpert.MarkTable<Entity>(new()
            {
                Uniques = new[]
                {
                    nameof(Entity.Account).FieldInfo<Entity>().Name,
                    nameof(Entity.Username).FieldInfo<Entity>().Name
                }
            }), default);
            await AddAsync(new()
            {
                Id = Guid.NewGuid(),
                GroupType = Group.IIoTPlatform,
                LicenseType = Authority.Administrator,
                OperateType = Operate.Enable,
                Account = "",
                Username = "",
                Password = "".UseEncryptAES(),
                Creator = "",
                CreateTime = DateTime.UtcNow
            });
        }
    }
    public async ValueTask AddAsync(Entity entity) => await ExecuteAsync(NpgsqlExpert.MarkInsert<Entity>(), entity);
    public async ValueTask UpdateAsync(Entity entity) => await ExecuteAsync(NpgsqlExpert.MarkUpdate<Entity>(new[]
    {
        nameof(Entity.GroupType),
        nameof(Entity.LicenseType),
        nameof(Entity.OperateType),
        nameof(Entity.Username),
        nameof(Entity.Password),
        nameof(Entity.Creator),
        nameof(Entity.CreateTime)
    }), entity);
    public async Task<Entity> GetAsync(Guid id)
    {
        var result = await SingleQueryAsync<Entity>(NpgsqlExpert.MarkQuery<Entity>(new[]
        {
            nameof(Entity.Id),
            nameof(Entity.GroupType),
            nameof(Entity.LicenseType),
            nameof(Entity.OperateType),
            nameof(Entity.Account),
            nameof(Entity.Username),
            nameof(Entity.Password),
            nameof(Entity.Creator),
            nameof(Entity.CreateTime)
        }).AddObjectFilter(nameof(Entity.Id).To<Entity>(), nameof(id)), new { id });
        return new()
        {
            Id = result.Id,
            GroupType = result.GroupType,
            LicenseType = result.LicenseType,
            OperateType = result.OperateType,
            Account = result.Account,
            Username = result.Username,
            Password = result.Password.UseDecryptAES(),
            Creator = result.Creator,
            CreateTime = result.CreateTime
        };
    }
    public async Task<Entity> GetAsync(string account)
    {
        var result = await SingleQueryAsync<Entity>(NpgsqlExpert.MarkQuery<Entity>(new[]
        {
            nameof(Entity.Id),
            nameof(Entity.GroupType),
            nameof(Entity.LicenseType),
            nameof(Entity.OperateType),
            nameof(Entity.Account),
            nameof(Entity.Username),
            nameof(Entity.Password),
            nameof(Entity.Creator),
            nameof(Entity.CreateTime)
        }).AddObjectFilter(nameof(Entity.Account).To<Entity>(), nameof(account)), new { account });
        return new()
        {
            Id = result.Id,
            GroupType = result.GroupType,
            LicenseType = result.LicenseType,
            OperateType = result.OperateType,
            Username = result.Username,
            Account = result.Account,
            Password = result.Password.UseDecryptAES(),
            Creator = result.Creator,
            CreateTime = result.CreateTime
        };
    }
    public async Task<IEnumerable<Entity>> ListAsync()
    {
        var result = await QueryAsync<Entity>(NpgsqlExpert.MarkQuery<Entity>(new[]
        {
            nameof(Entity.Id),
            nameof(Entity.GroupType),
            nameof(Entity.LicenseType),
            nameof(Entity.OperateType),
            nameof(Entity.Account),
            nameof(Entity.Username),
            nameof(Entity.Password),
            nameof(Entity.Creator),
            nameof(Entity.CreateTime)
        }).ToString(), default);
        return result.Select(item => new Entity
        {
            Id = item.Id,
            GroupType = item.GroupType,
            LicenseType = item.LicenseType,
            OperateType = item.OperateType,
            Username = item.Username,
            Account = item.Account,
            Password = item.Password.UseDecryptAES(),
            Creator = item.Creator,
            CreateTime = item.CreateTime
        });
    }
    public async Task<IEnumerable<Entity>> ListAsync(Group genre)
    {
        var result = await QueryAsync<Entity>(NpgsqlExpert.MarkQuery<Entity>(new[]
        {
            nameof(Entity.Id),
            nameof(Entity.GroupType),
            nameof(Entity.LicenseType),
            nameof(Entity.OperateType),
            nameof(Entity.Account),
            nameof(Entity.Username),
            nameof(Entity.Password),
            nameof(Entity.Creator),
            nameof(Entity.CreateTime)
        }).AddEqualFilter(nameof(Entity.GroupType).To<Entity>(), (int)genre), default);
        return result.Select(item => new Entity
        {
            Id = item.Id,
            GroupType = item.GroupType,
            LicenseType = item.LicenseType,
            OperateType = item.OperateType,
            Username = item.Username,
            Account = item.Account,
            Password = item.Password.UseDecryptAES(),
            Creator = item.Creator,
            CreateTime = item.CreateTime
        });
    }
    public string TableName { get; init; } = TableName<Entity>();
    public required INpgsqlExpert NpgsqlExpert { get; init; }
}