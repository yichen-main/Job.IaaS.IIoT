namespace Infrastructure.Core.Boundaries;
public interface IBaseLoader
{
    Task WaitAsync();
    void UseMarquee();
    void Record(in RecordType type, in RecordTemplate template);
    ValueTask RefreshProfileAsync();
    ValueTask OverwriteProfileAsync(MainDilation.Profile profile);
    ValueTask PushBrokerAsync(string path, string message);
    string? UserName { get; }
    string? Password { get; }
    MqttServer Transport { get; }
    MainDilation.Profile? Profile { get; }
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class BaseLoader : IBaseLoader
{
    const int _keep = 5;
    const string extension = ".log";
    const RollingInterval _interval = RollingInterval.Day;
    const string _template = "[{Timestamp:HH:mm:ss}] {Message:lj}{Exception}{NewLine}";
    public BaseLoader()
    {
        BasicSettings ??= new LoggerConfiguration().Enrich.FromLogContext().MinimumLevel
            .Fatal().WriteTo.File(Path.Combine(Menu.HistoryPath, RecordType.BasicSettings.GetDESC(), extension),
            outputTemplate: _template, rollingInterval: _interval, retainedFileCountLimit: _keep).CreateLogger();
        MachineParts ??= new LoggerConfiguration().Enrich.FromLogContext().MinimumLevel
            .Fatal().WriteTo.File(Path.Combine(Menu.HistoryPath, RecordType.MachineParts.GetDESC(), extension),
            outputTemplate: _template, rollingInterval: _interval, retainedFileCountLimit: _keep).CreateLogger();
        Transport ??= new MqttFactory().CreateMqttServer(new MqttServerOptionsBuilder().WithDefaultEndpoint().WithDefaultEndpointPort(Queue).Build());
    }
    public async Task WaitAsync()
    {
        Executioner = true;
        if (Marquee is not null) await Marquee;
    }
    public void UseMarquee() => Marquee = Task.Run(async () =>
    {
        var result = string.Empty;
        PeriodicTimer periodic = new(TimeSpan.FromMilliseconds(100));
        while (await periodic.WaitForNextTickAsync())
        {
            Console.SetCursorPosition(default, Console.CursorTop);
            Console.Write(result switch
            {
                "\\" => result = "|",
                "|" => result = "/",
                "/" => result = "-",
                _ => result = "\\",
            });
            if (Executioner) periodic.Dispose();
        }
        Console.SetCursorPosition(default, Console.CursorTop);
        Console.Write(new string('\u00A0', Console.WindowWidth));
    });
    public void Record(in RecordType type, in RecordTemplate template)
    {
        switch (type)
        {
            case RecordType.BasicSettings:
                BasicSettings.Fatal(Menu.Title, template.Title, new { template.Name, template.Message });
                break;

            case RecordType.MachineParts:
                MachineParts.Fatal(Menu.Title, template.Title, new { template.Name, template.Message });
                break;
        }
    }
    public async ValueTask RefreshProfileAsync()
    {
        MainDilation.Profile entity = new();
        await entity.CreateProfileAaync();
        Profile = MainDilation.ReadFile(ref entity, Menu.ProfilePath);
        var code = Profile.BaseCode.UseDecryptAES().Split('/');
        UserName = code[0];
        Password = code[1];
    }
    public async ValueTask OverwriteProfileAsync(MainDilation.Profile profile) => await profile.CreateProfileAaync(cover: true);
    public async ValueTask PushBrokerAsync(string path, string message)
    {
        if (Transport.IsStarted) await Transport.InjectApplicationMessage(
        new(new()
        {
            Retain = true,
            Topic = path,
            PayloadSegment = Encoding.UTF8.GetBytes(message),
        })
        { SenderClientId = path.ToMd5() });
    }
    ILogger BasicSettings { get; init; }
    ILogger MachineParts { get; init; }
    Task? Marquee { get; set; }
    bool Executioner { get; set; }
    public string? UserName { get; private set; }
    public string? Password { get; private set; }
    public required MqttServer Transport { get; init; }
    public MainDilation.Profile? Profile { get; private set; }
}