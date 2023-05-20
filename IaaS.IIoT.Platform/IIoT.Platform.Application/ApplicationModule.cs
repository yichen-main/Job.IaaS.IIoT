namespace Platform.Application;

[DependsOn(typeof(ApplicationContractModule))]
public sealed class ApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddHostedService<ComplexOperator>();
        context.Services.AddHostedService<DataTransaction>();
        context.Services.AddHostedService<EnvironmentLayout>();
    }
}