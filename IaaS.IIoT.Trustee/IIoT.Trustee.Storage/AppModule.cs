namespace Trustee.Storage;

[DependsOn(typeof(InfrastructureCoreModule))]
internal sealed class AppModule : AbpModule
{
    public AppModule() => Assembly.GetExecutingAssembly().CreateKanban();
    public override void ConfigureServices(ServiceConfigurationContext context)
    {

    }
}