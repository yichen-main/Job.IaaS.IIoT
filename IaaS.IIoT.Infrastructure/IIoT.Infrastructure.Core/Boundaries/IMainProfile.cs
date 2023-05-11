namespace Infrastructure.Core.Boundaries;
public interface IMainProfile
{
    ValueTask RefreshAsync();
    ValueTask OverwriteAsync(MainText text);
    ValueTask PushMessageAsync(string topic, string payload);
    MainText? Text { get; }
    MqttServer Transport { get; }
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class MainProfile : IMainProfile
{
    public MainProfile() => Transport = new MqttFactory().CreateMqttServer(new MqttServerOptionsBuilder()
        .WithDefaultEndpoint().WithDefaultEndpointPort(Queue).Build());
    public async ValueTask RefreshAsync()
    {
        MainText entity = new();
        await entity.CreateProfileAaync();
        Text = MainDilation.ReadFile(ref entity, Menu.ProfilePath);
    }
    public async ValueTask OverwriteAsync(MainText text) => await text.CreateProfileAaync(cover: true);
    public async ValueTask PushMessageAsync(string topic, string payload)
    {
        if (Transport.IsStarted) await Transport.InjectApplicationMessage(
        new(new()
        {
            Retain = true,
            Topic = topic,
            PayloadSegment = Encoding.UTF8.GetBytes(payload),
        })
        { SenderClientId = topic.ToMd5() });
    }
    public MainText? Text { get; private set; }
    public required MqttServer Transport { get; init; }
}