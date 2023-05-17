namespace Infrastructure.Base.Utilities.Develops;
public static class ProcessDevelop
{
    public const string DefaultDateFormat = "yyyy/MM/dd HH:mm:ss";
    public enum SystemStatus
    {
        Invalid,
        Allow,
        Issued
    }
    public enum TimeZoneType
    {
        [Description("UniversalTimeCoordinated")] UTC = 0,
        [Description("CentralEuropeanSummerTime")] CEST = -2,
        [Description("CentralEuropeanTime")] CET = -1,
        [Description("ChinaStandardTime")] CST = 8
    }
    public enum LanguageType
    {
        [Description("en-US")] English,
        [Description("zh-CN")] Simplified,
        [Description("zh-TW")] Traditional
    }
    public enum RecordType
    {
        [Description("Basic Settings")] BasicSettings,
        [Description("Machine Parts")] MachineParts
    }
    public readonly record struct RecordTemplate
    {
        public required string Title { get; init; }
        public required string Name { get; init; }
        public required string Message { get; init; }
    }
    public record Header
    {
        [FromHeader(Name = "Accept-Language")] public string? Language { get; init; }
        [FromHeader(Name = "Time-Format")] public string? TimeFormat { get; init; }
        [FromHeader(Name = "Time-Zone")] public string? TimeZone { get; init; }
    }
    public ref struct Sign
    {
        public const string RootPath = "/ffg-imds/service";
        public const string SoapPath = $"{RootPath}.asmx";
        public const string OpcUaPath = $"{RootPath}.opcua";
    }
    public ref struct Menu
    {
        const string _toolFileName = "Bottom";
        public static string Title => "[{0}] {1}";
        public static string HourFormat => "yyyy/MM/dd HH";
        public static string HistoryPath => Path.Combine(Native, "..", "..", "..", "Logs");
        public static string ProfilePath => Path.Combine(Native, "..", "..", "..", "Main.yml");
        public static string GetToolPath() => Path.Combine(Native, "..", "..", _toolFileName);
    }
    public ref struct Hash
    {
        public static string Organize => "9aX5XoJDA+Ct1d/1aACijwIvHuCk+ppOn7/bpsMHBDo=";
        public static string BaseGate => "0mqIq++vMr0tNb/SdC+5emckqU2Pmp2CioYcFf8ejO7R3CaoENxgrn7Y9i99zgBE";
        public static string UserGate => "XazQDntDOdgS75poAiBGyffN7W3/KKgRSBvjnNXRzf6uFEA1901OAGzsM7zbZ3lb+W+zy39u8g318M9LhEd8EA==";
    }
    public static int Queue => 1883;
    public static int OpcUa => 4840;
    public static int Entrance => 7260;
    public static SystemStatus Status { get; set; }
    public static string StartupKey { get; set; } = string.Empty;
    public static bool WindowsPass => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    public static string Native => Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ?? string.Empty;
}