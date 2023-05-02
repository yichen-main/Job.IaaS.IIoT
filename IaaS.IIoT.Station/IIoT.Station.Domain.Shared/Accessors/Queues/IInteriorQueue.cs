namespace Station.Domain.Shared.Accessors.Queues;
public interface IInteriorQueue
{
    void PushSpindleThermalCompensation(in string payload);
}