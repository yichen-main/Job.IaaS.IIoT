namespace Infrastructure.Garner.Architects.Expertise;
public interface IInfluxExpert
{
    ValueTask InitialDataPoolAsync(string bucket);
    ValueTask WriteAsync<T>(T entity, string bucket);
    ValueTask WriteAsync<T>(T[] entities, string bucket);
    T[] Read<T>(in string bucket, string identifier, DateTime startTime, DateTime endTime) where T : MetaBase;
    enum BucketTag
    {
        [Description("base_machines")] BaseMachine = 101,
        [Description("part_controllers")] Controller = 201,
        [Description("part_spindles")] Spindle = 202,
        [Description("part_water_tanks")] WaterTank = 203,
        [Description("tack_sensors")] Sensor = 301
    }
    abstract class MetaBase
    {
        [Column("_identifier", IsTag = true)] public required string Identifier { get; init; }
        [Column(IsTimestamp = true)] public required DateTime Timestamp { get; init; }
    }
    bool Enabled { get; set; }
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class InfluxExpert : IInfluxExpert
{
    readonly IBaseLoader _baseLoader;
    public InfluxExpert(IBaseLoader baseLoader) => _baseLoader = baseLoader;
    public async ValueTask InitialDataPoolAsync(string bucket)
    {
        if (_baseLoader.Profile is not null)
        {
            var address = GetAddress(_baseLoader.Profile.Database.IP, _baseLoader.Profile.Database.InfluxDB);
            using var result = new InfluxDBClient(address, _baseLoader.UserName, _baseLoader.Password);
            var entity = await result.GetBucketsApi().FindBucketByNameAsync(bucket);
            if (entity is null)
            {
                var organizations = await result.GetOrganizationsApi().FindOrganizationsAsync(org: Hash.Organize.UseDecryptAES());
                BucketRetentionRules rule = new(BucketRetentionRules.TypeEnum.Expire, 30 * 86400);
                await result.GetBucketsApi().CreateBucketAsync(bucket, rule, organizations[default].Id);
            }
        }
    }
    public async ValueTask WriteAsync<T>(T entity, string bucket)
    {
        if (Enabled && _baseLoader.Profile is not null)
        {
            var address = GetAddress(_baseLoader.Profile.Database.IP, _baseLoader.Profile.Database.InfluxDB);
            using var result = new InfluxDBClient(address, _baseLoader.UserName, _baseLoader.Password);
            await result.GetWriteApiAsync().WriteMeasurementAsync(entity, WritePrecision.Ns, bucket, Hash.Organize.UseDecryptAES());
        }
    }
    public async ValueTask WriteAsync<T>(T[] entities, string bucket)
    {
        if (Enabled && _baseLoader.Profile is not null)
        {
            var address = GetAddress(_baseLoader.Profile.Database.IP, _baseLoader.Profile.Database.InfluxDB);
            using var result = new InfluxDBClient(address, _baseLoader.UserName, _baseLoader.Password);
            await result.GetWriteApiAsync().WriteMeasurementsAsync(entities, WritePrecision.Ns, bucket, Hash.Organize.UseDecryptAES());
        }
    }
    public T[] Read<T>(in string bucket, string identifier, DateTime startTime, DateTime endTime) where T : IInfluxExpert.MetaBase
    {
        if (Enabled && _baseLoader.Profile is not null)
        {
            var address = GetAddress(_baseLoader.Profile.Database.IP, _baseLoader.Profile.Database.InfluxDB);
            using var result = new InfluxDBClient(address, _baseLoader.UserName, _baseLoader.Password);
            return InfluxDBQueryable<T>.Queryable(bucket, Hash.Organize.UseDecryptAES(), result.GetQueryApiSync()).Where(item =>
            item.Identifier == identifier && item.Timestamp > startTime.ToUniversalTime() && item.Timestamp < endTime.ToUniversalTime())
            .OrderByDescending(item => item.Timestamp).ToArray();
        }
        return Array.Empty<T>();
    }
    static string GetAddress(string ip, int port) => $"{Uri.UriSchemeHttp}://{ip}:{port}";
    public bool Enabled { get; set; }
}