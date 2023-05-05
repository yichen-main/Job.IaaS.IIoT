namespace Infrastructure.Base.Utilities.Develops;
public static class ProcessDevelop
{
    public enum TimeZoneType
    {
        [Description("UTC")] UniversalTimeCoordinated = 0,
        [Description("CEST")] CentralEuropeanSummerTime = -2,
        [Description("CET")] CentralEuropeanTime = -1,
        [Description("CST")] ChinaStandardTime = 8
    }
    public enum LanguageType
    {
        [Description("en-US")] English,
        [Description("zh-CN")] Simplified,
        [Description("zh-TW")] Traditional
    }
    public record Header
    {
        [FromHeader(Name = "Accept-Language")] public string? Language { get; init; }
        [FromHeader(Name = "Time-Format")] public string? TimeFormat { get; init; }
        [FromHeader(Name = "Time-Zone")] public string? TimeZone { get; init; }
    }
    public ref struct Sign
    {
        public const string ProductName = "IIoT-Gateway";
        public const string ManufacturerName = "FFG-iMDS";
        public const string RootPath = "/ffg-imds/service";
        public const string SoapPath = $"{RootPath}.asmx";
        public const string OpcUaPath = $"{RootPath}.opcua";
        public const string Namespace = "https://imds.ffg-tw.com";
    }
    public ref struct Front
    {
        public static string Grade { get; set; } = string.Empty;
    }
    public ref struct Hash
    {
        public static string Organize => "9aX5XoJDA+Ct1d/1aACijwIvHuCk+ppOn7/bpsMHBDo=";
        public static string UserName => "4UHowqjbTFLl1MC+4ZxYpzrjdIn7zIsLCfWxelp12/I=";
        public static string Passkey => "lVf3jEWtkoXU2T9yS7k4MmkT757Ty39YV5uosUX9Yt0=";
        public static string Gate => "XazQDntDOdgS75poAiBGyffN7W3/KKgRSBvjnNXRzf6uFEA1901OAGzsM7zbZ3lb+W+zy39u8g318M9LhEd8EA==";
    }
    public ref struct Menu
    {
        const string _toolFileName = "Bottom";
        public static string Title => "[{0}] {1}";
        public static string HourFormat => "yyyy/MM/dd HH";
        public static string DefaultFormat => "yyyy/MM/dd HH:mm:ss";
        public static TimeSpan RefreshTime => TimeSpan.FromSeconds(1);
        public static string HistoryPath => Path.Combine(Local.Native, "..", "..", "..", "Logs");
        public static string ProfilePath => Path.Combine(Local.Native, "..", "..", "..", "Main.yml");
        public static string GetToolPath() => Path.Combine(Local.Native, "..", "..", _toolFileName);
    }
    public ref struct Local
    {
        public static int DayToSeconds => 86400;
        public static int CalibrationHour { get; set; }
        public static string Language { get; set; } = LanguageType.English.GetDescription();
        public static string TimeZone { get; set; } = TimeZoneType.ChinaStandardTime.GetDescription();
        public static string Native => Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ?? string.Empty;
    }
    public static int Queue => 1883;
    public static int OpcUa => 4840;
    public static int Metadata => 8086;
    public static int Postgres => 5432;
    public static SystemStatus Status { get; set; }
    public static string StartupKey { get; set; } = string.Empty;
}