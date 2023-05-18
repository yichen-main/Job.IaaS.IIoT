namespace Platform.Application.Contract.Wrappers;
public interface IMessagePublisher
{
    IFocasHelper FocasHelper { get; }
    IModbusHelper ModbusHelper { get; }
    IRawCalculation RawCalculation { get; }
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class MessagePublisher : IMessagePublisher
{
    public MessagePublisher(
        IFocasHelper focasHelper,
        IModbusHelper modbusHelper,
        IRawCalculation rawCalculation)
    {
        FocasHelper = focasHelper;
        ModbusHelper = modbusHelper;
        RawCalculation = rawCalculation;
    }
    public required IFocasHelper FocasHelper { get; init; }
    public required IModbusHelper ModbusHelper { get; init; }
    public required IRawCalculation RawCalculation { get; init; }
}