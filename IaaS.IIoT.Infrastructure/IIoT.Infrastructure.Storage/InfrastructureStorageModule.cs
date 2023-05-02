namespace Infrastructure.Storage;
public sealed class InfrastructureStorageModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        JoinNameplate(IPAddress.Loopback, Postgres, Hash.Organize, Hash.UserName.UseDecryptAES(), Hash.Passkey.UseDecryptAES());
    }
}