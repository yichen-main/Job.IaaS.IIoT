namespace Station.Domain.Shared.Timeseries.Bases.Enrollments;
public interface IOpcUaRegistrant
{
    Task InsertAsync(Data data);
    readonly record struct Data
    {
        public required SessionEventReason Status { get; init; }
        public required string SessionName { get; init; }
    }
}