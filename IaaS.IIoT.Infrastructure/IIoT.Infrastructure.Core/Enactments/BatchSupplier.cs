namespace Infrastructure.Core.Enactments;
public abstract class BatchSupplier
{
    readonly ServiceType _service;
    protected BatchSupplier(ServiceType service, string? text = null)
    {
        _service = service;
        if (WindowsPass) System.IO.File.WriteAllText(ScriptLocation, text ?? $"""
        {Sign.Declaration}
        cls & {StartupKey}.exe {StartupKey.ToMd5()} -app
        """, Encoding.UTF8);
    }
    protected async ValueTask StarterAsync()
    {
        if (WindowsPass) await System.IO.File.WriteAllTextAsync(OutputLocation(ButtonType.Boot), $""""
        {BatchHeader}
        %{NssmTag}% install "%{IdentifyTag}%" "{ScriptLocation}"
        %{NssmTag}% start "%{IdentifyTag}%"
        {BatchEndfield}
        """", Encoding.UTF8);
    }
    protected async ValueTask StopperAsync()
    {
        if (WindowsPass) await System.IO.File.WriteAllTextAsync(OutputLocation(ButtonType.Shutdown), $"""
        {BatchHeader}
        %{NssmTag}% stop %{IdentifyTag}% 
        %{NssmTag}% remove %{IdentifyTag}% confirm
        sc delete %{IdentifyTag}%
        {BatchEndfield}
        """, Encoding.UTF8);
    }
    static string Move(string path) => $"cd {path}";
    static string Variable(string definition) => $"set {definition}";
    string OutputLocation(ButtonType button) => Path.Combine(Native, "..", $"{_service.GetDESC()}{button.GetDESC()}.cmd");
    protected enum ButtonType
    {
        [Description("\u00A0[START]")] Boot,
        [Description("\u00A0[STOP]")] Shutdown
    }
    protected enum ServiceType
    {
        [Description("IIoT.Platform")] Platform,
        [Description("IIoT.Storage")] Storage,
        [Description("IIoT.Warder")] Warder
    }
    protected ref struct Sign
    {
        public static string Declaration => "@echo off<nul 3>nul";
    }
    string BatchHeader => $"""
    {Sign.Declaration}
    {Administrator}
    {Move("\\")}
    {char.ToLower(Native.FirstOrDefault())}:
    {Move(Menu.GetToolPath())}
    {Variable($"{NssmTag}=service")}
    {Variable($"{IdentifyTag}={_service.GetDESC()}")}
    """;
    static string NssmTag => "vehicle";
    static string IdentifyTag => "identify";
    static string BatchEndfield => $"""timeout /t 1""";
    static string Administrator => """
    %1 Mshta vbscript:CreateObject("Shell.Application").ShellExecute("Cmd.exe","/C ""%~0"" ::","","runas",1)(window.close)&&exit
    """;
    static string ScriptLocation => Path.Combine(Native, $"{StartupKey.ToMd5()}.cmd");
}