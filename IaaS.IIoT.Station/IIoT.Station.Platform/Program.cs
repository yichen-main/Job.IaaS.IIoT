try
{
    var website = await Entrance.UseWebsite<AppModule>();
    {
        website.Services.AddSwaggerGen();
        website.Services.AddControllers();
        website.Services.AddAuthenticationCore();
        website.Services.AddEndpointsApiExplorer();
        website.Services.AddAuthentication("FFG").AddScheme<AuthenticateOption, AuthenticateHandler>("FFG", configureOptions: default);
        website.Services.AddCors(item => item.AddDefaultPolicy(item =>
        {
            item.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().WithExposedHeaders("*");
        }));
    }
    var apply = website.Build();
    await apply.InitializeApplicationAsync();
    if (Status is not SystemStatus.Invalid)
    {
        apply.UseTriggers();
        if (Status is SystemStatus.Issued)
        {
            apply.MapControllers();
            apply.MapDemoEndpoint();
            apply.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                options.RoutePrefix = string.Empty;
            });
            apply.UseSwagger();
            apply.UseEndpoints(item => item.UseSoapEndpoint<IWebsiteTrigger>(Sign.SoapPath, new SoapEncoderOptions
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