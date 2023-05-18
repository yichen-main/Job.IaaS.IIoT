namespace Infrastructure.Garner.Architects.Formulas;
public interface IOverallEquipmentEffectiveness
{
    byte PlanMachineAvailability(int effectiveMinute, int downtimeMinute);
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class OverallEquipmentEffectiveness : IOverallEquipmentEffectiveness
{
    public byte PlanMachineAvailability(int effectiveMinute, int downtimeMinute)
    {
        var actualWorkingMinutes = effectiveMinute - downtimeMinute;
        return (byte)(Math.Round((double)actualWorkingMinutes / effectiveMinute, 2) * 100);
    }
}