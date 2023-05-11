namespace Infrastructure.Garner;

[DependsOn(typeof(InfrastructureCoreModule))]
public sealed class InfrastructureGarnerModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        JoinNameplate(IPAddress.Loopback, Postgres, Hash.Organize, Hash.UserName.UseDecryptAES(), Hash.Passkey.UseDecryptAES());
    }
}