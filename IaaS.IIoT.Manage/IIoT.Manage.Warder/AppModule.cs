namespace Manage.Warder;

[DependsOn(typeof(ApplicationModule))]
internal sealed class AppModule : AbpModule
{
    public AppModule() => Assembly.GetExecutingAssembly().CreateKanban();
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddHttpClient();
        if (Status is not SystemStatus.Invalid)
        {
            //Array.ForEach(context.Services.GetRequiredService<IEntrance[]>(), item => Task.Run(() => item.Build()));
            if (Status is SystemStatus.Issued)
            {
                context.Services.AddHostedService<GuardianService>();
            }
        }
    }
}