namespace Manage.Application.Contracts;

[DependsOn(typeof(DomainModule))]
public sealed class ApplicationContractModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {

    }
}