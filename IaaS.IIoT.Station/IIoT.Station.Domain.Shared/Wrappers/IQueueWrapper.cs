namespace Station.Domain.Shared.Wrappers;
public interface IQueueWrapper
{
    IIcpdasQueue Icpdas { get; }
    IInteriorQueue Interior { get; }
}