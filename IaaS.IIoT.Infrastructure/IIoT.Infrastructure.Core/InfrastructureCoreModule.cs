namespace Infrastructure.Core;
public sealed class InfrastructureCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(item => item.FileSets.AddEmbedded<InfrastructureCoreModule>(Assembly.GetExecutingAssembly().GetRootNamespace()));
        Configure<AbpLocalizationOptions>(item =>
        {
            item.Resources.Add<Fielder>(LanguageType.English.GetDESC()).AddVirtualJson($"/{string.Join('/', new string[]
            {
                nameof(Languages), nameof(Languages.Fielders)
            })}");
            item.Resources.Add<Sensor>(LanguageType.English.GetDESC()).AddVirtualJson($"/{string.Join('/', new string[]
            {
                nameof(Languages), nameof(Languages.Sensors)
            })}");
        });
    }
}