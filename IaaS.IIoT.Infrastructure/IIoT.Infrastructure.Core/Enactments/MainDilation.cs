namespace Infrastructure.Core.Enactments;
public static class MainDilation
{
    public static ref T ReadFile<T>(ref T entity, string path)
    {
        ConfigurationSource source = new()
        {
            Path = path,
            FileProvider = null,
            Optional = default,
            ReloadOnChange = default
        };
        source.ResolveFileProvider();
        ConfigurationBuilder builder = new();
        builder.Add(source);
        builder.Build().Bind(entity);
        return ref entity;
    }
    public static async ValueTask CreateProfileAaync<T>(this T entity, bool cover = default)
    {
        if ((!System.IO.File.Exists(Menu.ProfilePath) || cover) && entity is not null)
        {
            await System.IO.File.WriteAllTextAsync(Menu.ProfilePath, new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance).Build().Serialize(entity), Encoding.UTF8);
        }
    }
    sealed class ConfigurationSource : FileConfigurationSource
    {
        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            EnsureDefaults(builder);
            return new ConfigurationProvider(this);
        }
    }
    sealed class ConfigurationProvider : FileConfigurationProvider
    {
        public ConfigurationProvider(FileConfigurationSource source) : base(source) { }
        public override void Load(Stream stream)
        {
            YamlStream yamlStream = new();
            Stack<string> stackContext = new();
            yamlStream.Load(new StreamReader(stream));
            SortedDictionary<string, string?> data = new(StringComparer.Ordinal);
            if (yamlStream.Documents.Count > 0) VisitYamlNode(string.Empty, yamlStream.Documents[default].RootNode);
            void VisitYamlNode(string context, YamlNode node)
            {
                string currentPath;
                if (node is YamlScalarNode scalarNode)
                {
                    EnterContext(context);
                    data[currentPath] = scalarNode.Value ?? string.Empty;
                    ExitContext();
                }
                else if (node is YamlMappingNode mappingNode)
                {
                    EnterContext(context);
                    foreach (KeyValuePair<YamlNode, YamlNode> yamlNode in mappingNode.Children)
                    {
                        context = ((YamlScalarNode)yamlNode.Key).Value ?? string.Empty;
                        VisitYamlNode(context, yamlNode.Value);
                    }
                    ExitContext();
                }
                else if (node is YamlSequenceNode sequenceNode)
                {
                    EnterContext(context);
                    for (int item = default; item < sequenceNode.Children.Count; item++) VisitYamlNode(item.ToString(), sequenceNode.Children[item]);
                    ExitContext();
                }
                void EnterContext(string context)
                {
                    if (!string.IsNullOrEmpty(context)) stackContext.Push(context);
                    currentPath = ConfigurationPath.Combine(stackContext.Reverse());
                }
                void ExitContext()
                {
                    if (stackContext.Any()) stackContext.Pop();
                    currentPath = ConfigurationPath.Combine(stackContext.Reverse());
                }
            }
            Data = data;
        }
    }
    public sealed class Profile
    {
        [YamlMember(ApplyNamingConventions = false)] public string MachineID { get; set; } = string.Empty;
        [YamlMember(ApplyNamingConventions = false)] public string BaseCode { get; set; } = Hash.BaseGate;
        [YamlMember(ApplyNamingConventions = false)] public string UserCode { get; set; } = Hash.UserGate;

        [YamlMember(ApplyNamingConventions = false, Description = $"{nameof(TimeZoneType.UTC)}, {nameof(TimeZoneType.CEST)}, {nameof(TimeZoneType.CET)}, {nameof(TimeZoneType.CST)}")]
        public string TimeZone { get; set; } = nameof(TimeZoneType.CST);
        [YamlMember(ApplyNamingConventions = false)] public TextDatabase Database { get; set; } = new();
        [YamlMember(ApplyNamingConventions = false)] public TextController Controller { get; set; } = new();
        [YamlMember(ApplyNamingConventions = false)] public TextSerialEntry SerialEntry { get; set; } = new();
        [YamlMember(ApplyNamingConventions = false)] public TextFormulation Formulation { get; set; } = new();
        public sealed class TextDatabase
        {
            [YamlMember(ApplyNamingConventions = false)] public string IP { get; set; } = IPAddress.Loopback.ToString();
            [YamlMember(ApplyNamingConventions = false)] public int InfluxDB { get; set; } = 8086;
            [YamlMember(ApplyNamingConventions = false)] public int PostgreSQL { get; set; } = 5432;
        }
        public sealed class TextController
        {
            public enum HostType
            {
                None = 0,
                Fanuc = 1,
                Siemens = 2,
                Mitsubishi = 3,
                Heidenhain = 4
            }
            [YamlMember(ApplyNamingConventions = false)] public string IP { get; set; } = IPAddress.Loopback.ToString();
            [YamlMember(ApplyNamingConventions = false)] public int Port { get; set; } = 8193;

            [YamlMember(ApplyNamingConventions = false, Description = "None, Fanuc, Siemens, Mitsubishi, Heidenhain")]
            public HostType Type { get; set; } = HostType.None;
        }
        public sealed class TextSerialEntry
        {
            [YamlMember(ApplyNamingConventions = false)] public bool Enabled { get; set; }
            [YamlMember(ApplyNamingConventions = false)] public int BaudRate { get; set; } = 19200;
            [YamlMember(ApplyNamingConventions = false)] public string Port { get; set; } = "COM1";

            [YamlMember(ApplyNamingConventions = false, Description = $"{nameof(Parity.None)}, {nameof(Parity.Odd)}, {nameof(Parity.Even)}, {nameof(Parity.Mark)}, {nameof(Parity.Space)}")]
            public Parity Parity { get; set; } = Parity.None;

            [YamlMember(ApplyNamingConventions = false, Description = $"{nameof(StopBits.None)}, {nameof(StopBits.One)}, {nameof(StopBits.Two)}, {nameof(StopBits.OnePointFive)}")]
            public StopBits StopBits { get; set; } = StopBits.One;
        }
        public sealed class TextFormulation
        {
            [YamlMember(ApplyNamingConventions = false)] public double CarbonEmissionFactor { get; set; } = 0.509;
            [YamlMember(ApplyNamingConventions = false)] public int GlobalWarmingPotential { get; set; } = 1;

            [YamlMember(ApplyNamingConventions = false)]
            public WorkInterval[] WorkIntervals { get; set; } = Array.Empty<WorkInterval>();
            public sealed class WorkInterval
            {
                [YamlMember(ApplyNamingConventions = false)] public string StartMinute { get; set; } = string.Empty;
                [YamlMember(ApplyNamingConventions = false)] public string EndMinute { get; set; } = string.Empty;
            }
        }
    }
}