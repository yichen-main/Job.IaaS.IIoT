try
{
    await Host.CreateDefaultBuilder(args).ConfigureServices(async item =>
    {
        await item.AddApplicationAsync<AppModule>();
    }).Build().RunAsync();
}
catch (Exception e)
{
    Log.Fatal(Menu.Title, nameof(Program), new { e.Message, e.StackTrace });
}
finally
{
    EndLocker();
}