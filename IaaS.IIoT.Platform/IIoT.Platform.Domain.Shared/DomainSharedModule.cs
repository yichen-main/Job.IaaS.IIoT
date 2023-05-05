namespace Platform.Domain.Shared;

[DependsOn(typeof(InfrastructurePillboxModule), typeof(InfrastructureStorageModule))]
public sealed class DomainSharedModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Path.Combine(Menu.HistoryPath, "Systems", "sys-.log").UseRecorder();
    }
}