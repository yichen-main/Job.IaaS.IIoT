namespace Infrastructure.Garner.Architects.Formulas;
public interface IOverallEquipmentEffectiveness
{
    byte MachineAvailability(short workingMnutes, short breakMinutes, short downtimeMinutes);
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class OverallEquipmentEffectiveness : IOverallEquipmentEffectiveness
{
    public byte MachineAvailability(short workingMnutes, short breakMinutes, short downtimeMinutes)
    {
        var planWorkingMinutes = workingMnutes - breakMinutes;
        var actualWorkingMinutes = planWorkingMinutes - downtimeMinutes;
        return (byte)(Math.Round((double)actualWorkingMinutes / planWorkingMinutes, 2) * 100);
    }
}