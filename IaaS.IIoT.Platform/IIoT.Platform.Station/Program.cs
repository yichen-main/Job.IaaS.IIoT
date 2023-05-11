try
{
    var apply = await Entrance.UseWebsite<AppModule>();
    if (Status is not SystemStatus.Invalid)
    {
        apply.UseSerilogRequestLogging();
        if (Status is SystemStatus.Issued)
        {
            apply.UseRouting();
            apply.UseCors();
            apply.UseAuthentication();
            apply.UseAuthorization();
            apply.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                options.RoutePrefix = string.Empty;
            });
            apply.UseSwagger();
            apply.UseSymbolizer();
            apply.MapDigitalTwin();
            apply.MapControllers();
            apply.MapMessageQueuer();
            {
                apply.MapDemoEndpoint();
            }
            apply.UseEndpoints(item => item.UseSoapEndpoint<IWebsiteService>(Sign.SoapPath, new SoapEncoderOptions
            {
                WriteEncoding = Encoding.UTF8,
                ReaderQuotas = new XmlDictionaryReaderQuotas
                {
                    MaxStringContentLength = int.MaxValue
                }
            }, SoapSerializer.DataContractSerializer));
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