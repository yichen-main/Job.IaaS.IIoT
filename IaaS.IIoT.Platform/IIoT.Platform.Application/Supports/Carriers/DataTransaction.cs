namespace Platform.Application.Supports.Carriers;
internal sealed class DataTransaction(IBaseLoader baseLoader, IMessagePublisher messagePublisher) : BackgroundService
{
    readonly IBaseLoader _baseLoader = baseLoader;
    readonly IMessagePublisher _messagePublisher = messagePublisher;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await new PeriodicTimer(TimeSpan.FromSeconds(1)).WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                _messagePublisher.Token = stoppingToken;
                await Task.WhenAll(new[]
                {
                    _messagePublisher.BaseGroupAsync(),
                    _messagePublisher.PartGroupAsync()
                });
                if (Histories.Any()) Histories.Clear();
            }
            catch (Exception e)
            {
                if (!Histories.Contains(e.Message))
                {
                    Histories.Add(e.Message);
                    _baseLoader.Record(RecordType.BasicSettings, new()
                    {
                        Title = $"{nameof(DataTransaction)}.{nameof(ExecuteAsync)}",
                        Name = nameof(Exception),
                        Message = e.Message
                    });
                }
            }
        }
    }
    List<string> Histories { get; init; } = new();
}