namespace Infrastructure.Ministry.Develops;
public static class NodeDevelop
{
    public static async ValueTask<WebApplicationBuilder> UseWebsite<T>(this int port) where T : AbpModule
    {
        var builder = WebApplication.CreateBuilder();
        builder.Host.ConfigureHostOptions(item =>
        {
            item.ShutdownTimeout = TimeSpan.FromSeconds(15);
        }).AddAppSettingsSecretsJson().UseSystemd().UseSerilog();
        builder.WebHost.UseKestrel(item => item.ListenAnyIP(port));
        //await builder.AddApplicationAsync<T>();
        return builder;
    }
    public static void UseTriggers(this IApplicationBuilder app) => Array.ForEach(app.ApplicationServices.GetRequiredService<IEntrance[]>(), item =>
    {
        try
        {
            item.Build();
        }
        catch (Exception e)
        {

        }
    });
}