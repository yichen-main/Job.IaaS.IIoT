namespace Infrastructure.Host.Develops;
public abstract class AttachDevelop
{
    protected static string ConvertTwoWordHEX(in int quantity)
    {
        var count = 1;
        string before = string.Empty, middle = string.Empty, temp = string.Empty;
        var source = quantity.ToString("X4");
        if (source.Length % 2 is not 0) source = source.PadRight(source.Length + (2 - source.Length % 2));
        for (int upper = default; upper < source.Length; upper += 2)
        {
            for (int lower = default; lower < 2; lower++) temp += source[upper + lower];
            if (count is 1) middle = temp;
            else before = temp;
            temp = string.Empty;
            count++;
        }
        return $"{before}{middle}";
    }
    protected static string ConvertThreeWordHEX(in int quantity)
    {
        var count = 1;
        string before = string.Empty, middle = string.Empty, after = string.Empty, temp = string.Empty;
        var source = quantity.ToString("X6");
        if (source.Length % 2 is not 0) source = source.PadRight(source.Length + (2 - source.Length % 2));
        for (int upper = default; upper < source.Length; upper += 2)
        {
            for (int lower = default; lower < 2; lower++) temp += source[upper + lower];
            if (count is 1) after = temp;
            else if (count is 2) middle = temp;
            else before = temp;
            temp = string.Empty;
            count++;
        }
        return $"{before}{middle}{after}";
    }
}