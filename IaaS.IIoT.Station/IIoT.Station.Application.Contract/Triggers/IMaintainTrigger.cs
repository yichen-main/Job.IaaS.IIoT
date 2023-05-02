namespace Station.Application.Contract.Triggers;
public interface IMaintainTrigger
{
    ValueTask CreateSwitchFileAsync();
}