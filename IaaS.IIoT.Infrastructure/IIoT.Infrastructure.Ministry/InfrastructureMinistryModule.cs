namespace Infrastructure.Ministry;

[DependsOn(typeof(InfrastructureCoreModule), typeof(AbpAspNetCoreModule))]
public sealed class InfrastructureMinistryModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAuthenticationCore();
        context.Services.AddCors(item => item.AddDefaultPolicy(item =>
        {
            item.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().WithExposedHeaders("*");
        }));
        context.Services.AddControllers(item =>
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
            item.SerializerSettings.DateFormatString = Menu.DefaultFormat;
            item.SerializerSettings.NullValueHandling = NullValueHandling.Include;
        }).AddMvcOptions(item => item.Conventions.Add(new ModelConvention())).AddControllersAsServices();
    }
    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var service = context.GetApplicationBuilder();
        service.UseSerilogRequestLogging();
        service.UseRouting();
        service.UseCors();
        service.UseAuthentication();
        service.UseAuthorization();
    }
}