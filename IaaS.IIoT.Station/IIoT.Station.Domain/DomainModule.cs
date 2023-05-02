namespace Station.Domain;

[DependsOn(typeof(DomainSharedModule))]
public sealed class DomainModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddSingleton<ISensorPool, SensorPool>();
        context.Services.AddSingleton<IAbstractPool, AbstractPool>();
        context.Services.AddSingleton<IComponentPool, ComponentPool>();
        context.Services.AddSingleton<IFoundationPool, FoundationPool>();
        context.Services.AddSingleton<IMitsubishiPool, MitsubishiPool>();
        context.Services.AddSingleton<IMitsubishiHost, MitsubishiHost>();
        {
            context.Services.AddSingleton<TemplateEngine>();
            context.Services.AddSingleton<IHistoryEngine, HistoryEngine>();
            context.Services.AddSingleton<IStructuralEngine, StructuralEngine>();
        }
    }
}