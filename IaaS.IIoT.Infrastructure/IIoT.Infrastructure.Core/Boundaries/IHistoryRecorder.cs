namespace Infrastructure.Core.Boundaries;
public interface IHistoryRecorder
{
    void Record(in OperatorPayload payload);
    void Record(in FavorerPayload payload);
    void Record(in SenderPayload payload);

    [StructLayout(LayoutKind.Auto)]
    readonly record struct OperatorPayload
    {
        public required string Name { get; init; }
        public required string Message { get; init; }
    }

    [StructLayout(LayoutKind.Auto)]
    readonly record struct FavorerPayload
    {
        public required string Name { get; init; }
        public required string Message { get; init; }
        public required string? Source { get; init; }
    }

    [StructLayout(LayoutKind.Auto)]
    readonly record struct SenderPayload
    {
        public required string Name { get; init; }
        public required string Message { get; init; }
    }
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class HistoryRecorder : IHistoryRecorder
{
    const int _keep = 5;
    const RollingInterval _interval = RollingInterval.Day;
    const string _template = "[{Timestamp:HH:mm:ss}] {Message:lj}{Exception}{NewLine}";
    public HistoryRecorder()
    {
        Operator ??= new LoggerConfiguration().Enrich.FromLogContext().MinimumLevel.Fatal().WriteTo.File(Path.Combine(Menu.HistoryPath, "Operators", ".log"),
            outputTemplate: _template, rollingInterval: _interval, retainedFileCountLimit: _keep).CreateLogger();
        Favorer ??= new LoggerConfiguration().Enrich.FromLogContext().MinimumLevel.Fatal().WriteTo.File(Path.Combine(Menu.HistoryPath, "Favorers", ".log"),
            outputTemplate: _template, rollingInterval: _interval, retainedFileCountLimit: _keep).CreateLogger();
        Sender ??= new LoggerConfiguration().Enrich.FromLogContext().MinimumLevel.Fatal().WriteTo.File(Path.Combine(Menu.HistoryPath, "Senders", ".log"),
           outputTemplate: _template, rollingInterval: _interval, retainedFileCountLimit: _keep).CreateLogger();
    }
    public void Record(in IHistoryRecorder.OperatorPayload payload) => Operator.Fatal("{0}", new { payload.Name, payload.Message });
    public void Record(in IHistoryRecorder.SenderPayload payload) => Sender.Fatal("{0}", new { payload.Name, payload.Message });
    public void Record(in IHistoryRecorder.FavorerPayload payload) => Favorer.Fatal("{0}", new { payload.Name, payload.Message });
    ILogger Operator { get; init; }
    ILogger Favorer { get; init; }
    ILogger Sender { get; init; }
}