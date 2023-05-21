namespace Infrastructure.Garner.Architects.Expertise;
public interface IInfluxExpert
{
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
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class InfluxExpert(IBaseLoader baseLoader) : IInfluxExpert
{
    readonly IBaseLoader _baseLoader = baseLoader;
    public async ValueTask WriteAsync<T>(T entity, string bucket)
    {
        if (_baseLoader.StorageEnabled && _baseLoader.Profile is not null)
        {
            var address = _baseLoader.GetStorageAddress(_baseLoader.Profile.Database.IP, _baseLoader.Profile.Database.InfluxDB);
            using var result = new InfluxDBClient(address, _baseLoader.UserName, _baseLoader.Password);
            await result.GetWriteApiAsync().WriteMeasurementAsync(entity, WritePrecision.Ns, bucket, Hash.Organize.UseDecryptAES());
        }
    }
    public async ValueTask WriteAsync<T>(T[] entities, string bucket)
    {
        if (_baseLoader.StorageEnabled && _baseLoader.Profile is not null)
        {
            var address = _baseLoader.GetStorageAddress(_baseLoader.Profile.Database.IP, _baseLoader.Profile.Database.InfluxDB);
            using var result = new InfluxDBClient(address, _baseLoader.UserName, _baseLoader.Password);
            await result.GetWriteApiAsync().WriteMeasurementsAsync(entities, WritePrecision.Ns, bucket, Hash.Organize.UseDecryptAES());
        }
    }
    public T[] Read<T>(in string bucket, string identifier, DateTime startTime, DateTime endTime) where T : IInfluxExpert.MetaBase
    {
        if (_baseLoader.StorageEnabled && _baseLoader.Profile is not null)
        {
            var address = _baseLoader.GetStorageAddress(_baseLoader.Profile.Database.IP, _baseLoader.Profile.Database.InfluxDB);
            using var result = new InfluxDBClient(address, _baseLoader.UserName, _baseLoader.Password);
            return InfluxDBQueryable<T>.Queryable(bucket, Hash.Organize.UseDecryptAES(), result.GetQueryApiSync()).Where(item =>
            item.Identifier == identifier && item.Timestamp > startTime.ToUniversalTime() && item.Timestamp < endTime.ToUniversalTime())
            .OrderByDescending(item => item.Timestamp).ToArray();
        }
        return Array.Empty<T>();
    }

}