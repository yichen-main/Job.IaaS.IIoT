namespace Trustee.DataPool.Completes;

[Dependency(ServiceLifetime.Singleton)]
file sealed class InitialConstructor : BatchSupplier, IInitialConstructor
{
    const string _carrier = "influxd.exe";
    public InitialConstructor() : base(ServiceType.Storage, $"""
    {Sign.Declaration}
    cd {Menu.GetToolPath()}
    Taskkill /im {_carrier} /F
    cls & {_carrier}
    """) { }
    public async ValueTask CreateSwitchFileAsync()
    {
        await StarterAsync();
        await StopperAsync();
    }
}