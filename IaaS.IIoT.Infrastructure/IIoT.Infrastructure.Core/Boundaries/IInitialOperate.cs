namespace Infrastructure.Core.Boundaries;
public interface IInitialOperate
{
    void UseMarquee();
    Task WaitAsync();
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class InitialOperate : IInitialOperate
{
    public void UseMarquee() => Marquee = Task.Run(async () =>
    {
        var result = string.Empty;
        PeriodicTimer periodic = new(TimeSpan.FromMilliseconds(100));
        while (await periodic.WaitForNextTickAsync())
        {
            Console.SetCursorPosition(default, Console.CursorTop);
            Console.Write(result switch
            {
                "\\" => result = "|",
                "|" => result = "/",
                "/" => result = "-",
                _ => result = "\\",
            });
            if (Executioner) periodic.Dispose();
        }
        Console.SetCursorPosition(default, Console.CursorTop);
        Console.Write(new string('\u00A0', Console.WindowWidth));
    });
    public async Task WaitAsync()
    {
        Executioner = true;
        if (Marquee is not null) await Marquee;
    }
    Task? Marquee { get; set; }
    bool Executioner { get; set; }
}