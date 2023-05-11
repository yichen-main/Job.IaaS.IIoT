namespace Retinue.Domain.Shared;

[DependsOn(typeof(InfrastructureCoreModule), typeof(InfrastructurePillboxModule))]
public sealed class DomainSharedModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {

    }
}