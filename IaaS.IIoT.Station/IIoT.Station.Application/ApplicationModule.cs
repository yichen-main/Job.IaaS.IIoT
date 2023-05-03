namespace Station.Application;

[DependsOn(typeof(ApplicationContractModule))]
public sealed class ApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddHostedService<ReaderBottom>();
        context.Services.AddHostedService<WriterBottom>();
        context.Services.AddHostedService<ShellerBottom>();
        context.Services.AddHostedService<CourierBottom>();
        context.Services.AddHostedService<MonitorBottom>();
        context.Services.AddHostedService<OpcUaPanoply>();
        context.Services.AddHostedService<ModbusAttach>();
        context.Services.AddHostedService<DigiwinAttach>();
        {
            context.Services.AddSingleton<UserAccountService>();
            context.Services.AddSingleton<IWebsiteTrigger, WebsiteTrigger>();
            context.Services.AddSingleton<IEntrance, QueueTrigger>();
            context.Services.AddSingleton<IEntrance, InitialTrigger>();
            {
                context.Services.AddScoped<ProtectedSessionStorage>();
                context.Services.AddScoped<AuthenticationStateProvider, CustomStateProvider>();
            }
        }
    }
}