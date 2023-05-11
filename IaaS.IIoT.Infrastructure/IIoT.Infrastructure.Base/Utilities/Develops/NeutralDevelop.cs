namespace Infrastructure.Base.Utilities.Develops;
public static class NeutralDevelop
{
    public enum SystemStatus
    {
        Invalid,
        Allow,
        Issued
    }
    public static async ValueTask<WebApplication> UseWebsite<T>(this int port) where T : AbpModule
    {
        var builder = WebApplication.CreateBuilder();
        builder.Host.ConfigureHostOptions(item =>
        {
            item.ShutdownTimeout = TimeSpan.FromSeconds(15);
        }).AddAppSettingsSecretsJson().UseSystemd().UseSerilog();
        builder.WebHost.UseKestrel(item => item.ListenAnyIP(port));
        await builder.AddApplicationAsync<T>();
        return builder.Build();
    }
    public static void EndLocker(bool keep = false)
    {
        Log.CloseAndFlush();
        if (keep)
        {
            Console.Write("Hit any key to exit...");
            Console.ReadKey(intercept: true);
        }
    }
    public static void UseRecorder(this string path)
    {
        Log.Logger = new LoggerConfiguration().Enrich.FromLogContext().MinimumLevel.Warning()
        .MinimumLevel.Override("System", LogEventLevel.Error)
        .MinimumLevel.Override("Default", LogEventLevel.Error)
        .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
        .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Error)
        .MinimumLevel.Override("Volo.Abp.Core", LogEventLevel.Error)
        .MinimumLevel.Override("Volo.Abp.AspNetCore", LogEventLevel.Error)
        .WriteTo.File(path, outputTemplate: "[{Timestamp:HH:mm:ss.fff} {Level:u3}] {Message:lj}{Exception}{NewLine}",
        rollingInterval: RollingInterval.Day, retainedFileCountLimit: 10).CreateLogger();
    }
    public static void CreateKanban(this Assembly assembly)
    {
        StartupKey = assembly.GetName().Name ?? string.Empty;
        Status = InitialAuthenticator();
        Console.Title = Plaque(StartupKey);
        Console.CursorVisible = default;
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;
        Output(string.Join(Environment.NewLine, new string[]
        {
            string.Concat("　Hostname        =>   　", Dns.GetHostName()),
            string.Concat("　Username        =>   　", Environment.UserName),
            string.Concat("　Language        =>   　", Thread.CurrentThread.CurrentCulture.IetfLanguageTag),
            string.Concat("　Internet        =>   　", NetworkInterface.GetIsNetworkAvailable()),
            string.Concat("　.NET Version    =>   　", Environment.Version),
            string.Concat("　IPv4 Physical   =>   　", NetworkInterfaceType.Ethernet.AddLocalIPv4()),
            string.Concat("　IPv4 Wireless   =>   　", NetworkInterfaceType.Wireless80211.AddLocalIPv4()),
            string.Concat("　OS Environment  =>   　", Environment.OSVersion)
        }), ConsoleColor.Yellow);
        Output(new[]
        {
            FiggleFonts.Standard.Render("FFG-iMDS".Aggregate(string.Empty.PadLeft(7, '\u00A0'),
            (first, second) => string.Concat(first, second, string.Empty.PadLeft(3, '\u00A0')))),
            new string('*', 90), Environment.NewLine
        }.Concat(), ConsoleColor.White);
        static void Output(string content, ConsoleColor consoleColor = ConsoleColor.White)
        {
            Console.ForegroundColor = consoleColor;
            Console.WriteLine(content);
        }
        string Plaque(string name) => $"{name} v{FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion}";
        SystemStatus InitialAuthenticator()
        {
            var arguments = Environment.GetCommandLineArgs();
            switch (arguments.Length)
            {
                case 2:
                    if (!arguments[1].IsNullOrWhiteSpace())
                    {
                        if (arguments[1] == StartupKey.ToMd5()) return SystemStatus.Allow;
                    }
                    return SystemStatus.Invalid;

                case 3:
                    if (!arguments[1].IsNullOrWhiteSpace())
                    {
                        if (arguments[1] == StartupKey.ToMd5())
                        {
                            if (!arguments[2].IsNullOrWhiteSpace())
                            {
                                if (arguments[2] == "-app") return SystemStatus.Issued;
                            }
                        }
                    }
                    return SystemStatus.Invalid;

                default:
                    return SystemStatus.Invalid;
            }
        }
    }
    public static string Concat(this string[] args)
    {
        DefaultInterpolatedStringHandler handler = new(default, args.Length);
        for (int item = default; item < args.Length; item++) handler.AppendFormatted(args[item]);
        return handler.ToStringAndClear();
    }
    public static string AddLocalIPv4(this NetworkInterfaceType networkInterfaceType)
    {
        var result = string.Empty;
        Array.ForEach(NetworkInterface.GetAllNetworkInterfaces(), item =>
        {
            if (item.NetworkInterfaceType == networkInterfaceType && item.OperationalStatus is OperationalStatus.Up)
            {
                foreach (var info in item.GetIPProperties().UnicastAddresses)
                {
                    if (info.Address.AddressFamily is AddressFamily.InterNetwork) result = info.Address.ToString();
                }
            }
        });
        return result;
    }
    public static string UseEncryptAES(this string text)
    {
        using var aes = Aes.Create();
        using var encryptor = aes.CreateEncryptor(Encoding.UTF8.GetBytes(nameof(Infrastructure).ToMd5()), aes.IV);
        {
            using MemoryStream msEncrypt = new();
            using (CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (StreamWriter swEncrypt = new(csEncrypt)) swEncrypt.Write(text);
            {
                var iv = aes.IV;
                var decryptedContent = msEncrypt.ToArray();
                var result = new byte[iv.Length + decryptedContent.Length];
                Buffer.BlockCopy(iv, default, result, default, iv.Length);
                Buffer.BlockCopy(decryptedContent, default, result, iv.Length, decryptedContent.Length);
                return Convert.ToBase64String(result);
            }
        }
    }
    public static string UseDecryptAES(this string text)
    {
        var iv = new byte[16];
        var fullCipher = Convert.FromBase64String(text);
        var cipher = new byte[fullCipher.Length - iv.Length];
        Buffer.BlockCopy(fullCipher, default, iv, default, iv.Length);
        Buffer.BlockCopy(fullCipher, iv.Length, cipher, default, fullCipher.Length - iv.Length);
        {
            using var aes = Aes.Create();
            using var decryptor = aes.CreateDecryptor(Encoding.UTF8.GetBytes(nameof(Infrastructure).ToMd5()), iv);
            using MemoryStream msDecrypt = new(cipher);
            using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
            using StreamReader srDecrypt = new(csDecrypt);
            return srDecrypt.ReadToEnd();
        }
    }
    public static string ToTimestamp(this DateTime time, string? format) => time.AddHours(Local.CalibrationHour).ToString(format ?? Menu.DefaultFormat);
    public static DateTime ToNowHour(this DateTime time) => DateTime.ParseExact(time.ToString(Menu.HourFormat), Menu.HourFormat, CultureInfo.InvariantCulture);
    public static string Joint(this string front, string latter = "", string tag = ".") => $"{front}{tag}{latter}";
    public static string GetRootNamespace(this Assembly assembly) => assembly.GetName().Name!.Replace("FFG".Joint(), string.Empty);
    public static string GetDescription(this Type type, string name) => type.GetRuntimeField(name)!.GetCustomAttribute<DescriptionAttribute>()!.Description;
    public static string GetDescription(this Enum @enum) => @enum.GetType().GetRuntimeField(@enum.ToString())!.GetCustomAttribute<DescriptionAttribute>()!.Description;
    public static IDictionary<string, (int number, string description)> GetDescription<T>()
    {
        Dictionary<string, (int number, string description)> results = new();
        foreach (Enum @enum in Enum.GetValues(typeof(T))) results.Add(@enum.ToString(), (@enum.GetHashCode(), @enum.GetDescription()));
        return results.ToImmutableDictionary();
    }
    public static T? ToObject<T>(this string content) => JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
    {
        AllowTrailingCommas = true,
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    });
    public static T? ToObject<T>(this byte[] contents) => JsonSerializer.Deserialize<T>(contents, new JsonSerializerOptions
    {
        AllowTrailingCommas = true,
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    });
    public static string ToJson<T>(this T @object, bool indented = false) => JsonSerializer.Serialize(@object, typeof(T), new JsonSerializerOptions
    {
        WriteIndented = indented,
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    });
}