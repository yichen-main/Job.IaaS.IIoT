namespace Retinue.Domain.Helpers;
internal sealed class FocasHelper : FocasDevelop, IFocasHelper
{
    ushort _handle;
    public void Close() => DisconnectCNC(_handle);
    public bool Open(string ip, ushort port) => ConnectCNC(ip, port, 5, out _handle) == default;
    public void ConnectionStatus(bool enable) => Enable = enable;
    public SystemInfo GetSystemInfo()
    {
        SystemInfo result = new();
        SystemInfoCNC(_handle, result);
        return result;
    }
    public bool Enable { get; private set; }
}