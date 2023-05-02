namespace Station.Domain.Shared.Functions.Hosts;
public interface IMitsubishiHost
{
    ValueTask CreateAsync(string ip, int port);
    enum DeviceCode
    {
        [Description("90")] M,
        [Description("91")] SM,
        [Description("92")] L,
        [Description("93")] F,
        [Description("94")] V,
        [Description("9C")] X,
        [Description("9D")] Y,
        [Description("A0")] B,
        [Description("A1")] SB,
        [Description("A8")] D,
        [Description("A9")] SD,
        [Description("AF")] R,
        [Description("B0")] ZR,
        [Description("B4")] W,
        [Description("B5")] SW,
        [Description("C0")] TI,
        [Description("C1")] TO,
        [Description("C2")] TA,
        [Description("C3")] CI,
        [Description("C4")] CO,
        [Description("C5")] CA,
        [Description("C6")] STI,
        [Description("C7")] STO,
        [Description("C8")] STA
    }
    ref struct FixedPart
    {
        public const string FixedHead = "500000FFFF0300";
        public const string ReadCommand = "0104";
        public const string WriteCommand = "0114";
        public const string SinglePoint = "0100";
        public const string MultiPoint = "0000";
        public const string DataLength = "0C00";
        public const string DataTimer = "1000";
    }
    readonly record struct ReadText
    {
        public required string Fixed { get; init; }
        public required string Timer { get; init; }
        public required string Length { get; init; }
        public required string Command { get; init; }
        public required string SubCommand { get; init; }
        public required string DeviceCode { get; init; }
        public required string StartPoint { get; init; }
        public required string Quantity { get; init; }
    }
}