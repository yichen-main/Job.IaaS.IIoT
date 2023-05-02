namespace Station.Domain.Functions.Engines;
internal sealed class StructuralEngine : IStructuralEngine
{
    public StructuralEngine()
    {
        Transport = new MqttFactory().CreateMqttServer(new MqttServerOptionsBuilder().WithDefaultEndpoint().WithDefaultEndpointPort(Queue).Build());
    }
    //public async ValueTask RefreshMainProfileAsync()
    //{
    //    MainProfile profile = new();
    //    await profile.CreateFileAaync();
    //    Profile = profile.ReadFile(Menu.ProfilePath);
    //}
    //public async ValueTask OverwriteMainProfileAsync(MainProfile profile) => await profile.CreateFileAaync(cover: true);
    public async ValueTask InitialDataPoolAsync(string url, string organize, string userName, string password, string bucket)
    {
        using var result = new InfluxDBClient(url, userName, password);
        var entity = await result.GetBucketsApi().FindBucketByNameAsync(bucket);
        if (entity is null)
        {
            var organizations = await result.GetOrganizationsApi().FindOrganizationsAsync(org: organize);
            BucketRetentionRules rule = new(BucketRetentionRules.TypeEnum.Expire, 30 * Local.DayToSeconds);
            await result.GetBucketsApi().CreateBucketAsync(bucket, rule, organizations[default].Id);
        }
    }
    public bool EnableStorage { get; set; }
    //public MainProfile? Profile { get; set; }
    public required MqttServer Transport { get; init; }
    public ArrayPool<byte> BytePool { get; } = ArrayPool<byte>.Shared;
    public string Version { get; } = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ?? string.Empty;
}