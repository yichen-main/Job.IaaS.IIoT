try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.ConfigureHostOptions(item =>
    {
        item.ShutdownTimeout = TimeSpan.FromSeconds(15);
    }).AddAppSettingsSecretsJson().UseSystemd().UseSerilog();
    builder.WebHost.UseKestrel(item => item.ListenAnyIP(7260));
    await builder.AddApplicationAsync<AppModule>();
    if (Status is not SystemStatus.Invalid)
    {
        var apply = builder.Build();
        if (Status is SystemStatus.Issued)
        {
            apply.UseSerilogRequestLogging();
            apply.UseRouting();
            apply.UseCors();
            apply.UseAuthentication();
            apply.UseAuthorization();
            apply.MapControllers();
            apply.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                options.RoutePrefix = string.Empty;
            });
            apply.UseSwagger();
            //apply.MapDemoEndpoint();
            //apply.UseEndpoints(item => item.UseSoapEndpoint<IWebsiteTrigger>(Sign.SoapPath, new SoapEncoderOptions
            //{
            //    WriteEncoding = Encoding.UTF8,
            //    ReaderQuotas = new XmlDictionaryReaderQuotas
            //    {
            //        MaxStringContentLength = int.MaxValue
            //    }
            //}, SoapSerializer.DataContractSerializer));
        }
        await apply.RunAsync();
    }
}
catch (Exception e)
{
    Log.Fatal(Menu.Title, nameof(Program), new { e.Message, e.StackTrace });
}
finally
{
    EndLocker();
}