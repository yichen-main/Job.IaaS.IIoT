namespace Station.Application.Contract.Expanses;
public static class StructureExpanse
{
    public static string ConvertFamily(this string? language)
    {
        if (language is null) return LanguageType.English.GetDescription();
        return language switch
        {
            var item when item.Equals(LanguageType.Traditional.GetDescription(), StringComparison.OrdinalIgnoreCase) => LanguageType.Traditional.GetDescription(),
            var item when item.Equals(LanguageType.Simplified.GetDescription(), StringComparison.OrdinalIgnoreCase) => LanguageType.Simplified.GetDescription(),
            _ => LanguageType.English.GetDescription()
        };
    }
}
