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
        if ((!File.Exists(Menu.ProfilePath) || cover) && entity is not null)
        {
            await File.WriteAllTextAsync(Menu.ProfilePath, new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build().Serialize(entity), Encoding.UTF8);
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
        [YamlMember(ApplyNamingConventions = false)] public string MachineID { get; init; } = string.Empty;   
        [YamlMember(ApplyNamingConventions = false)] public string BaseCode { get; init; } = Hash.BaseGate;
        [YamlMember(ApplyNamingConventions = false)] public string UserCode { get; init; } = Hash.UserGate;
        [YamlMember(ApplyNamingConventions = false)] public TextDatabase Database { get; init; } = new();
        [YamlMember(ApplyNamingConventions = false)] public TextController Controller { get; init; } = new();
        [YamlMember(ApplyNamingConventions = false)] public TextSerialEntry SerialEntry { get; init; } = new();
        public sealed class TextDatabase
        {
            [YamlMember(ApplyNamingConventions = false)] public string IP { get; init; } = IPAddress.Loopback.ToString();
            [YamlMember(ApplyNamingConventions = false)] public int InfluxDB { get; init; } = 8086;
            [YamlMember(ApplyNamingConventions = false)] public int PostgreSQL { get; init; } = 5432;
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
            [YamlMember(ApplyNamingConventions = false)] public string IP { get; init; } = IPAddress.Loopback.ToString();
            [YamlMember(ApplyNamingConventions = false)] public int Port { get; init; } = 8193;

            [YamlMember(ApplyNamingConventions = false, Description = "None, Fanuc, Siemens, Mitsubishi, Heidenhain")]
            public TextType Type { get; init; } = TextType.None;
        }
        public sealed class TextSerialEntry
        {
            [YamlMember(ApplyNamingConventions = false)] public bool Enabled { get; init; }
            [YamlMember(ApplyNamingConventions = false)] public int BaudRate { get; init; } = 19200;
            [YamlMember(ApplyNamingConventions = false)] public string Port { get; init; } = "COM1";

            [YamlMember(ApplyNamingConventions = false, Description = $"{nameof(Parity.None)}, {nameof(Parity.Odd)}, {nameof(Parity.Even)}, {nameof(Parity.Mark)}, {nameof(Parity.Space)}")]
            public Parity Parity { get; init; } = Parity.None;

            [YamlMember(ApplyNamingConventions = false, Description = $"{nameof(StopBits.None)}, {nameof(StopBits.One)}, {nameof(StopBits.Two)}, {nameof(StopBits.OnePointFive)}")]
            public StopBits StopBits { get; init; } = StopBits.One;
        }
    }
}