namespace Manage.Installer;

[DependsOn(typeof(ApplicationModule))]
internal sealed class AppModule : AbpModule
{
    public AppModule() => Assembly.GetExecutingAssembly().CreateKanban();
    public override void ConfigureServices(ServiceConfigurationContext context)
    {

    }
}