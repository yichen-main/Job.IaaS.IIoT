namespace Station.Application.Additions.Triggers;

[Dependency(ServiceLifetime.Singleton)]
file sealed class MaintainTrigger : BatchSupplier, IMaintainTrigger
{
    public MaintainTrigger() : base(ServiceType.Platform) { }
    public async ValueTask CreateSwitchFileAsync()
    {
        await StarterAsync();
        await StopperAsync();
    }
}