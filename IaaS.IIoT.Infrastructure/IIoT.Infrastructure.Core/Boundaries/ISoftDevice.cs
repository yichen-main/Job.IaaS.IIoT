namespace Infrastructure.Core.Boundaries;
public interface ISoftDevice
{
    bool IsEnable(string name);
    string Execute(string name, string path);
    readonly ref struct Tag
    {
        public const string Extension = "cmd";
        public const string Officer = "service";
        public const string RootPath = "cd %~dp0";
        public const string Boot = "\u00A0[START]";
        public const string Shutdown = "\u00A0[STOP]";
        public const string Reboot = "\u00A0[RESTART]";
        public const string BatchTitle = "@echo off<nul 3>nul";
    }
}

[Dependency(ServiceLifetime.Transient)]
file sealed class SoftDevice : ISoftDevice
{
    public bool IsEnable(string name)
    {
        bool status = default;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            try
            {
                if (!new ServiceController(name).Status.Equals(ServiceControllerStatus.Stopped))
                {
                    status = true;
                }
            }
            catch (Exception)
            {
                return status;
            }
        }
        return status;
    }
    public string Execute(string name, string path)
    {
        using Process process = new();
        try
        {
            process.StartInfo.Verb = "runas";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.FileName = $"{path}{name}";
            {
                process.Start();
                process.WaitForExit();
                var result = process.StandardError.ReadLine();
                if (process.ExitCode is not 0 && !string.IsNullOrEmpty(result))
                {
                    result = result.Replace(Environment.NewLine, string.Empty);
                    result = result.Replace("n", string.Empty);
                    throw new Exception(result);
                }
                process.Close();
                return string.Empty;
            }
        }
        catch (Exception e)
        {
            process.Kill();
            return e.Message;
        }
    }
}