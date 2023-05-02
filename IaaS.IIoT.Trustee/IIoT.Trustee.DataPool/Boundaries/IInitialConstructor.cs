namespace Trustee.DataPool.Boundaries;
public interface IInitialConstructor
{
    ValueTask CreateSwitchFileAsync();
}