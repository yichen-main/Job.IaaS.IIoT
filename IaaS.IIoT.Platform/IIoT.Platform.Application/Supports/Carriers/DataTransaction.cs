namespace Platform.Application.Supports.Carriers;
internal sealed class DataTransaction : BackgroundService
{
    readonly IBaseLoader _baseLoader;
    readonly IMessagePublisher _messagePublisher;
    public DataTransaction(IBaseLoader baseLoader, IMessagePublisher messagePublisher)
        => (_baseLoader, _messagePublisher) = (baseLoader, messagePublisher);
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await new PeriodicTimer(TimeSpan.FromSeconds(1)).WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await Task.WhenAll(new Task[]
                {
                    _messagePublisher.BasicInformationAsync(),
                    _messagePublisher.PartInformationAsync()
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