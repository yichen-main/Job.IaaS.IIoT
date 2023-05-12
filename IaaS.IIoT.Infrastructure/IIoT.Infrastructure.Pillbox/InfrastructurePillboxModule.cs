namespace Infrastructure.Pillbox;

[DependsOn(typeof(InfrastructureGarnerModule))]
public sealed class InfrastructurePillboxModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddHostedService<HostCollector>();
    }
}