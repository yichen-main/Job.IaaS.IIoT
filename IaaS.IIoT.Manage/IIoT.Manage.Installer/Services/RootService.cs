namespace Manage.Installer.Services;

[Dependency(ServiceLifetime.Singleton)]
internal sealed class RootService
{
    public async Task CreateAsync()
    {
        InitialOperate.UseMarquee();


        await Task.Delay(10000);


        await InitialOperate.WaitAsync();
    }
    public required IInitialOperate InitialOperate { get; init; }
}