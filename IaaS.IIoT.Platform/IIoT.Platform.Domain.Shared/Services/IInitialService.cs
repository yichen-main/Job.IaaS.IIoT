namespace Platform.Domain.Shared.Services;
public interface IInitialService
{
    ValueTask CreateSwitchFileAsync(CancellationToken token);
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class InitialService : BatchSupplier, IInitialService
{
    public InitialService() : base(ServiceType.Platform) { }
    public async ValueTask CreateSwitchFileAsync(CancellationToken token)
    {
        await StarterAsync(token);
        await StopperAsync(token);
    }
}