namespace Infrastructure.Core.Enactments;
public sealed class MainText
{
    [YamlMember(ApplyNamingConventions = false)] public string MachineID { get; init; } = "m-001";
    [YamlMember(ApplyNamingConventions = false)] public string Organize { get; init; } = Hash.Organize.UseDecryptAES();
    [YamlMember(ApplyNamingConventions = false)] public string TimeZone { get; init; } = Local.TimeZone;
    [YamlMember(ApplyNamingConventions = false)] public string Language { get; init; } = Local.Language;
    [YamlMember(ApplyNamingConventions = false)] public string Grade { get; init; } = Hash.Gate;  
    [YamlMember(ApplyNamingConventions = false)] public TextInfluxDB InfluxDB { get; init; } = new();
    [YamlMember(ApplyNamingConventions = false)] public TextController Controller { get; init; } = new();
    [YamlMember(ApplyNamingConventions = false)] public TextSerialEntry SerialEntry { get; init; } = new();
    public sealed class TextInfluxDB
    {
        [YamlMember(ApplyNamingConventions = false)] public string URL { get; init; } = $"{Uri.UriSchemeHttp}://{IPAddress.Loopback}:{Metadata}";
        [YamlMember(ApplyNamingConventions = false)] public string UserName { get; init; } = Hash.UserName.UseDecryptAES();
        [YamlMember(ApplyNamingConventions = false)] public string Password { get; init; } = Hash.Passkey.UseDecryptAES();
    }
    public sealed class TextController
    {
        public enum TextType
        {
            None = 0,
            Fanuc = 1,
            Siemens = 2,
            Mitsubishi = 3,
            Heidenhain = 4
        }

        [YamlMember(ApplyNamingConventions = false, Description = "None, Fanuc, Siemens, Mitsubishi, Heidenhain")]
        public TextType Type { get; init; } = TextType.Mitsubishi;
        [YamlMember(ApplyNamingConventions = false)] public string IP { get; init; } = IPAddress.Loopback.ToString();
        [YamlMember(ApplyNamingConventions = false)] public int Port { get; init; } = 30000;
    }
    public sealed class TextSerialEntry
    {
        [YamlMember(ApplyNamingConventions = false)] public string SerialPort { get; init; } = "COM3";
        [YamlMember(ApplyNamingConventions = false)] public int BaudRate { get; init; } = 19200;

        [YamlMember(ApplyNamingConventions = false, Description = $"{nameof(Parity.None)}, {nameof(Parity.Odd)}, {nameof(Parity.Even)}, {nameof(Parity.Mark)}, {nameof(Parity.Space)}")] 
        public Parity Parity { get; init; } = Parity.None;

        [YamlMember(ApplyNamingConventions = false, Description = $"{nameof(StopBits.None)}, {nameof(StopBits.One)}, {nameof(StopBits.Two)}, {nameof(StopBits.OnePointFive)}")] 
        public StopBits StopBits { get; init; } = StopBits.One;
    }
}