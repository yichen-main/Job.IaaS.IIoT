namespace Platform.Domain.Shared.Services;
public interface IInitialService
{
    ValueTask CreateSwitchFileAsync();
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class InitialService : BatchSupplier, IInitialService
{
    public InitialService() : base(ServiceType.Platform) { }
    public async ValueTask CreateSwitchFileAsync()
    {
        await StarterAsync();
        await StopperAsync();
    }
}