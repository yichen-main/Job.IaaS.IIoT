namespace Retinue.Vehicle;

[DependsOn(typeof(ApplicationModule))]
internal sealed class AppModule : AbpModule
{
    public AppModule() => Assembly.GetExecutingAssembly().CreateKanban();
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        if (Status is not SystemStatus.Invalid)
        {

            if (Status is SystemStatus.Issued)
            {
                context.Services.AddHostedService<FanucService>();
            }
        }
    }
}