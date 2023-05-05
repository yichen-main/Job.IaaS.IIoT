﻿namespace Infrastructure.Storage.Architects.Experts;
public interface IInfluxExpert
{
    ValueTask InitialDataPoolAsync(string url, string organize, string userName, string password, string bucket);
    enum BucketTag
    {
        [Description("base_enrollments")] Enrollment = 101,
        [Description("part_controllers")] Controller = 201,
        [Description("part_spindles")] Spindle = 202,
        [Description("part_water_tanks")] WaterTank = 203,
        [Description("tack_sensors")] Sensor = 301
    }
    abstract class MetaBase
    {
        [Column("_machine_id", IsTag = true)] public required string MachineID { get; init; }
        [Column("_identifier", IsTag = true)] public required string Identifier { get; init; }
        [Column(IsTimestamp = true)] public required DateTime Timestamp { get; init; }
    }
    bool EnableStorage { get; set; }
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class InfluxExpert : IInfluxExpert
{
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
}