try
{
    var provider = await AbpApplicationFactory.CreateAsync<AppModule>();
    if (Status is not SystemStatus.Invalid)
    {
        await provider.InitializeAsync();
        await provider.ServiceProvider.GetRequiredService<RootService>().CreateAsync();
    }
}
catch (Exception e)
{
    Log.Fatal(Menu.Title, nameof(Program), new { e.Message, e.StackTrace });
}
finally
{
    EndLocker(keep: true);
}