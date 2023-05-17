namespace Trustee.DataPool.Services;

[Dependency(ServiceLifetime.Singleton)]
internal sealed class RootService
{
    readonly IBaseLoader _baseLoader;
    readonly IInitialConstructor _initialConstructor;
    public RootService(IBaseLoader baseLoader, IInitialConstructor initialConstructor)
    {
        _baseLoader = baseLoader;
        _initialConstructor = initialConstructor;
    }
    public async Task CreateAsync()
    {
        _baseLoader.UseMarquee();
        await _initialConstructor.CreateSwitchFileAsync();
        await _baseLoader.WaitAsync();
    }
}