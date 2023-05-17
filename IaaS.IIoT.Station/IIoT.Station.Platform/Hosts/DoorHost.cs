namespace Station.Platform.Hosts;
internal sealed class DoorHost : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var builder = WebApplication.CreateBuilder();
        builder.Host.ConfigureHostOptions(item =>
        {
            item.ShutdownTimeout = TimeSpan.FromSeconds(15);
        }).AddAppSettingsSecretsJson().UseSystemd().UseSerilog();
        builder.WebHost.UseKestrel(item => item.ListenAnyIP(7260));
        builder.Services.AddSoapCore();
        builder.Services.AddSwaggerGen();
        builder.Services.AddAuthenticationCore();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddControllers(item =>
        {
            item.ReturnHttpNotAcceptable = true;
            item.Filters.Add<ExceptionFilter>();
        }).ConfigureApiBehaviorOptions(item =>
        {
            item.SuppressModelStateInvalidFilter = default;
            item.InvalidModelStateResponseFactory = context =>
            {
                List<string> results = new();
                results.AddRange(Refresher());
                return new UnprocessableEntityObjectResult(new { Message = string.Join(",\u00A0", results) })
                {
                    ContentTypes = { MediaTypeNames.Application.Json }
                };
                IEnumerable<string> Refresher()
                {
                    foreach (var entry in context.ModelState.Root.Children ?? Enumerable.Empty<ModelStateEntry>())
                    {
                        for (int i = default; i < entry.Errors.Count; i++) yield return entry.Errors[i].ErrorMessage;
                    }
                }
            };
        }).AddNewtonsoftJson(item =>
        {
            item.SerializerSettings.DateFormatString = DefaultDateFormat;
            item.SerializerSettings.NullValueHandling = NullValueHandling.Include;
        }).AddMvcOptions(item => item.Conventions.Add(new ModelConvention())).AddControllersAsServices();
        builder.Services.AddAuthentication("FFG").AddScheme<AuthenticateOption, AuthenticateHandler>("FFG", configureOptions: default);
        builder.Services.AddCors(item => item.AddDefaultPolicy(item =>
        {
            item.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().WithExposedHeaders("*");
        }));
        var apply = builder.Build();
        if (Status is not SystemStatus.Invalid)
        {
            if (Status is SystemStatus.Issued)
            {
                apply.UseSerilogRequestLogging();
                apply.UseRouting();
                apply.UseCors();
                apply.UseAuthentication();
                apply.UseAuthorization();
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
}