namespace Platform.Domain.Shared.Functions.Engines;
public interface IHistoryEngine
{
    void Record(in OperatorPayload payload);
    void Record(in FavorerPayload payload);
    void Record(in SenderPayload payload);

    [StructLayout(LayoutKind.Auto)]
    readonly record struct OperatorPayload
    {
        public required string Name { get; init; }
        public required string Message { get; init; }
    }

    [StructLayout(LayoutKind.Auto)]
    readonly record struct FavorerPayload
    {
        public required string Name { get; init; }
        public required string Message { get; init; }
        public required string? Source { get; init; }
    }

    [StructLayout(LayoutKind.Auto)]
    readonly record struct SenderPayload
    {
        public required string Name { get; init; }
        public required string Message { get; init; }
    }
}