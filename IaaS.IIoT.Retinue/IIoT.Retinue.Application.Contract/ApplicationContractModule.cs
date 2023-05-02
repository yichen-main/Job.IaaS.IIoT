namespace Retinue.Application.Contract;

[DependsOn(typeof(DomainModule))]
public sealed class ApplicationContractModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {

    }
}