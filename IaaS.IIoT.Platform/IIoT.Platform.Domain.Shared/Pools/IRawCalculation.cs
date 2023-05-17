namespace Platform.Domain.Shared.Pools;
internal interface IRawCalculation
{

}

[Dependency(ServiceLifetime.Singleton)]
file sealed class RawCalculation : IRawCalculation
{
    readonly IBaseLoader _baseLoader;
    readonly ITimelineWrapper _timelineWrapper;
    public RawCalculation(IBaseLoader baseLoader, ITimelineWrapper timelineWrapper)
    {
        _baseLoader = baseLoader;
        _timelineWrapper = timelineWrapper;
    }
    public async ValueTask BasisSstatistics()
    {
        var oneDayMachineStatusMinutes = _timelineWrapper.RootInformation.OneDayMachineStatusMinutes();


        await _baseLoader.PushBrokerAsync("basis/statistics/data", string.Empty);
    }
}