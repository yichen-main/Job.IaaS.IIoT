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
}