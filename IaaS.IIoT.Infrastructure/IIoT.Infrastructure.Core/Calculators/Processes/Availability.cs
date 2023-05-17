namespace Infrastructure.Core.Calculators.Processes;
public readonly struct Availability : IAvailability<Availability>
{
    readonly int _value;
    public Availability(int value) => _value = value;
    public static Availability operator +(Availability a, Availability b)
    {
        Availability hh = new(a._value + b._value);

        return new(a._value * b._value);
    }
    public static Availability Zero => new(3);
}