namespace Infrastructure.Garner.Architects.Formulas;
public interface IDescriptiveStatistics
{
    float Median(float[] groups);
    T? Mode<T>(T[] groups) where T : INumber<T?>;
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class DescriptiveStatistics : IDescriptiveStatistics
{
    public float Median(float[] groups)
    {
        Array.Sort(groups);
        if (groups.Length is 0) return default;
        if (groups.Length % 2 is 0) return (groups[groups.Length / 2] + groups[groups.Length / 2 - 1]) / 2;
        return groups[(groups.Length - 1) / 2];
    }
    public T? Mode<T>(T[] groups) where T : INumber<T?>
    {
        if (groups.Any())
        {
            var count = 1;
            var result = groups[default];
            for (var item = 1; item < groups.Length; item++)
            {
                if (count is 0) result = groups[item];
                if (result == groups[item]) ++count;
                else --count;
            }
            return result;
        }
        return default;
    }
}