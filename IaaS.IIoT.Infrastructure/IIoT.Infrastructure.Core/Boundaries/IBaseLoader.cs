namespace Infrastructure.Core.Boundaries;
public interface IBaseLoader
{
    Task WaitAsync();
    void UseMarquee();
    void Record(in RecordType type, in RecordTemplate template);
    ValueTask RefreshProfileAsync();
    ValueTask OverwriteProfileAsync(MainDilation.Profile profile);
    Task PushBrokerAsync(string path, string message);
    TimeZoneType GetTimeZone();
    ValueTask InitialStorageAsync(string bucket);
    string GetStorageAddress(string ip, int port);
    bool StorageEnabled { get; set; }
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
    public async ValueTask OverwriteProfileAsync(MainDilation.Profile profile)
    {
        if (!profile.Formulation.WorkIntervals.Any()) profile.Formulation.WorkIntervals = new[]
        {
            new MainDilation.Profile.TextFormulation.WorkInterval
            {
                StartMinute = "0800",
                EndMinute = "1200"
            },
            new MainDilation.Profile.TextFormulation.WorkInterval
            {
                StartMinute = "1300",
                EndMinute = "1700"
            }
        };
        await profile.CreateProfileAaync(cover: true);
    }
    public async Task PushBrokerAsync(string path, string message)
    {
        if (Transport.IsStarted) await Transport.InjectApplicationMessage(
        new(new()
        {
            Topic = path,
            Retain = true,
            QualityOfServiceLevel = MqttQualityOfServiceLevel.AtLeastOnce,
            PayloadSegment = Encoding.UTF8.GetBytes(message),
        })
        { SenderClientId = path.ToMd5() });
    }
    public TimeZoneType GetTimeZone()
    {
        if (Profile is not null) return Profile.TimeZone switch
        {
            nameof(TimeZoneType.CEST) => TimeZoneType.CEST,
            nameof(TimeZoneType.CET) => TimeZoneType.CET,
            nameof(TimeZoneType.CST) => TimeZoneType.CST,
            _ => TimeZoneType.UTC
        };
        return TimeZoneType.UTC;
    }
    public async ValueTask InitialStorageAsync(string bucket)
    {
        if (Profile is not null)
        {
            var address = GetStorageAddress(Profile.Database.IP, Profile.Database.InfluxDB);
            using var result = new InfluxDBClient(address, UserName, Password);
            var entity = await result.GetBucketsApi().FindBucketByNameAsync(bucket);
            if (entity is null)
            {
                var organizations = await result.GetOrganizationsApi().FindOrganizationsAsync(org: Hash.Organize.UseDecryptAES());
                BucketRetentionRules rule = new(BucketRetentionRules.TypeEnum.Expire, 30 * 86400);
                await result.GetBucketsApi().CreateBucketAsync(bucket, rule, organizations[default].Id);
            }
        }
    }
    public string GetStorageAddress(string ip, int port) => $"{Uri.UriSchemeHttp}://{ip}:{port}";
    ILogger BasicSettings { get; init; }
    ILogger MachineParts { get; init; }
    Task? Marquee { get; set; }
    bool Executioner { get; set; }
    public bool StorageEnabled { get; set; }
    public string? UserName { get; private set; }
    public string? Password { get; private set; }
    public required MqttServer Transport { get; init; }
    public MainDilation.Profile? Profile { get; private set; }
}