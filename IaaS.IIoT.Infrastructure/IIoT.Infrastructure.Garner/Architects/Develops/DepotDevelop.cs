namespace Infrastructure.Garner.Architects.Develops;
public abstract class DepotDevelop<T> where T : IInfluxExpert.MetaBase
{
    readonly bool _enabled;
    readonly string? _address;
    readonly string? _userName;
    readonly string? _password;
    readonly string? _organize;
    protected DepotDevelop(IInfluxExpert influx, IMainProfile profile)
    {
        _enabled = influx.EnableStorage;
        _address = profile.Text?.InfluxDB.URL;
        _userName = profile.Text?.InfluxDB.UserName;
        _password = profile.Text?.InfluxDB.Password;
        _organize = profile.Text?.Organize;
    }
    protected async ValueTask WriteAsync(T meta, string bucket)
    {
        if (_enabled && _address is not null && _userName is not null && _password is not null && _organize is not null)
        {
            using InfluxDBClient result = new(_address, _userName, _password);
            await result.GetWriteApiAsync().WriteMeasurementAsync(meta, WritePrecision.Ns, bucket, _organize);
        }
    }
    protected async ValueTask WriteAsync(T[] metas, string bucket)
    {
        if (_enabled && _address is not null && _userName is not null && _password is not null && _organize is not null)
        {
            using InfluxDBClient result = new(_address, _userName, _password);
            await result.GetWriteApiAsync().WriteMeasurementsAsync(metas, WritePrecision.Ns, bucket, _organize);
        }
    }
    protected T[] Read(in string bucket, string identifier, DateTime startTime, DateTime endTime)
    {
        if (_enabled && _address is not null && _userName is not null && _password is not null && _organize is not null)
        {
            using InfluxDBClient result = new(_address, _userName, _password);
            return InfluxDBQueryable<T>.Queryable(bucket, _organize, result.GetQueryApiSync()).Where(item =>
            item.Identifier == identifier && item.Timestamp > startTime.ToUniversalTime() && item.Timestamp < endTime.ToUniversalTime())
            .OrderByDescending(item => item.Timestamp).ToArray();
        }
        return Array.Empty<T>();
    }
    protected float Median(float[] arraies)
    {
        Array.Sort(arraies);
        if (arraies.Length is 0) return default;
        if (arraies.Length % 2 is 0) return (arraies[arraies.Length / 2] + arraies[arraies.Length / 2 - 1]) / 2;
        return arraies[arraies.Length / 2];
    }
}