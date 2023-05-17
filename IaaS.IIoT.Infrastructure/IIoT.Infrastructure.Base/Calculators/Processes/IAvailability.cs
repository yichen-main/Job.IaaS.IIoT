namespace Infrastructure.Base.Calculators.Processes;
public interface IAvailability<T> where T : IAvailability<T>
{
    public static abstract T operator +(T a, T b);
    public static abstract T Zero { get; }
}