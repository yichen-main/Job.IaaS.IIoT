namespace Platform.Domain.Functions.Engines;

[Dependency(ServiceLifetime.Singleton)]
file sealed class HistoryEngine : IHistoryEngine
{
    const int _keep = 5;
    const RollingInterval _interval = RollingInterval.Day;
    const string _template = "[{Timestamp:HH:mm:ss}] {Message:lj}{Exception}{NewLine}";
    public HistoryEngine()
    {
        Operator ??= new LoggerConfiguration().Enrich.FromLogContext().MinimumLevel.Fatal().WriteTo.File(Path.Combine(Menu.HistoryPath, "Operators", ".log"),
            outputTemplate: _template, rollingInterval: _interval, retainedFileCountLimit: _keep).CreateLogger();
        Favorer ??= new LoggerConfiguration().Enrich.FromLogContext().MinimumLevel.Fatal().WriteTo.File(Path.Combine(Menu.HistoryPath, "Favorers", ".log"),
            outputTemplate: _template, rollingInterval: _interval, retainedFileCountLimit: _keep).CreateLogger();
        Sender ??= new LoggerConfiguration().Enrich.FromLogContext().MinimumLevel.Fatal().WriteTo.File(Path.Combine(Menu.HistoryPath, "Senders", ".log"),
           outputTemplate: _template, rollingInterval: _interval, retainedFileCountLimit: _keep).CreateLogger();
    }
    public void Record(in IHistoryEngine.OperatorPayload payload) => Operator.Fatal("{0}", new { payload.Name, payload.Message });
    public void Record(in IHistoryEngine.SenderPayload payload) => Sender.Fatal("{0}", new { payload.Name, payload.Message });
    public void Record(in IHistoryEngine.FavorerPayload payload) => Favorer.Fatal("{0}", new { payload.Name, payload.Message });
    ILogger Operator { get; init; }
    ILogger Favorer { get; init; }
    ILogger Sender { get; init; }
}