namespace Station.Domain.Shared;

[DependsOn(typeof(InfrastructureCoreModule), typeof(InfrastructurePillboxModule))]
public sealed class DomainSharedModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Path.Combine(Menu.HistoryPath, "Systems", "sys-.log").UseRecorder();
    }
}