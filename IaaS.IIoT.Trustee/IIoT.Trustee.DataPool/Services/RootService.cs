namespace Trustee.DataPool.Services;

[Dependency(ServiceLifetime.Singleton)]
internal sealed class RootService
{
    public async Task CreateAsync()
    {
        InitialOperate.UseMarquee();
        await InitialConstructor.CreateSwitchFileAsync();
        await InitialOperate.WaitAsync();
    }
    public required IInitialOperate InitialOperate { get; init; }
    public required IInitialConstructor InitialConstructor { get; init; }
}