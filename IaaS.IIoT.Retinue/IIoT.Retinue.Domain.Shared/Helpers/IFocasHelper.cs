namespace Retinue.Domain.Shared.Helpers;
public interface IFocasHelper
{
    void Close();
    bool Open(string ip, ushort port);
    void ConnectionStatus(bool enable);
    SystemInfo GetSystemInfo();
    bool Enable { get; }
}