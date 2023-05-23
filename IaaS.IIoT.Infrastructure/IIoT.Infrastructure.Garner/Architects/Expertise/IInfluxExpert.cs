using static Infrastructure.Garner.Architects.Expertise.IInfluxExpert;

namespace Infrastructure.Garner.Architects.Expertise;
public interface IInfluxExpert
{
    ValueTask WriteAsync<T>(T entity, string bucket);
    ValueTask WriteAsync<T>(T[] entities, string bucket);
    ValueTask<T[]> ReadAsync<T>(string bucket, string identifier, DateTime startTime, DateTime endTime) where T : MetaBase;
    enum BucketTag
    {
        [Description("base_machines")] BaseMachine = 101,
        [Description("part_controllers")] Controller = 201,
        [Description("part_water_tanks")] WaterTank = 203,
    }
    abstract class MetaBase
    {
        public const string Id = "_identifier";
        [Column(Id, IsTag = true)] public required string Identifier { get; init; }
        [Column(IsTimestamp = true)] public required DateTime Timestamp { get; init; }
    }
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class InfluxExpert(IBaseLoader baseLoader) : IInfluxExpert
{
    readonly IBaseLoader _baseLoader = baseLoader;
    public async ValueTask WriteAsync<T>(T entity, string bucket)
    {
        var url = _baseLoader.GetStorageURL();
        if (_baseLoader.StorageEnabled && url is not null)
        {
            using var client = new InfluxDBClient(url, _baseLoader.UserName, _baseLoader.Password);
            await client.GetWriteApiAsync().WriteMeasurementAsync(entity, WritePrecision.Ns, bucket, Organize);
        }
    }
    public async ValueTask WriteAsync<T>(T[] entities, string bucket)
    {
        var url = _baseLoader.GetStorageURL();
        if (_baseLoader.StorageEnabled && url is not null)
        {
            using var client = new InfluxDBClient(url, _baseLoader.UserName, _baseLoader.Password);
            await client.GetWriteApiAsync().WriteMeasurementsAsync(entities, WritePrecision.Ns, bucket, Organize);
        }
    }
    public async ValueTask<T[]> ReadAsync<T>(string bucket, string identifier, DateTime startTime, DateTime endTime) where T : MetaBase
    {
        var url = _baseLoader.GetStorageURL();
        if (_baseLoader.StorageEnabled && url is not null)
        {
            using var client = new InfluxDBClient(url, _baseLoader.UserName, _baseLoader.Password);
            var result = await client.GetQueryApi().QueryAsync<T>($"""
            from(bucket: "{bucket}")
                |> range(start: {Format(startTime.ToUniversalTime())}, stop: {Format(endTime.ToUniversalTime())})
                |> filter(fn: (r) => (r["{MetaBase.Id}"] == "{identifier}"))
                |> pivot(rowKey:["_time"], columnKey: ["_field"], valueColumn: "_value")
                |> drop(columns: ["_start", "_stop", "_measurement"])
            """, Organize);
            return result.OrderByDescending(item => item.Timestamp).ToArray();
        }
        return Array.Empty<T>();
        static string Format(DateTime time) => $"{time:yyyy-MM-ddTHH:mm:ssZ}";
    }
    static string Organize => Hash.Organize.UseDecryptAES();
}