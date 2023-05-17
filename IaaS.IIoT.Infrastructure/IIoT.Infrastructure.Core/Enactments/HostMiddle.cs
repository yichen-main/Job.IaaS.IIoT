namespace Infrastructure.Core.Enactments;
public static class HostMiddle
{
    public static IApplicationBuilder MapMessageQueuer(this IApplicationBuilder app)
    {
        var service = app.ApplicationServices.GetRequiredService<IBaseLoader>();
        service.Transport.InterceptingPublishAsync += @event => Task.Run(() =>
        {
            try
            {
                var paths = @event.ApplicationMessage.Topic.Split('/');
                var text = Encoding.UTF8.GetString(@event.ApplicationMessage.PayloadSegment);
                if (Histories.Any()) Histories.Clear();
            }
            catch (Exception e)
            {
                if (!Histories.Contains(e.Message))
                {
                    Histories.Add(e.Message);
                    service.Record(RecordType.BasicSettings, new()
                    {
                        Title = $"{nameof(HostMiddle)}.{nameof(MapMessageQueuer)}",
                        Name = "MQTT Server",
                        Message = e.Message
                    });
                }
            }
        });
        return app;
    }
    public static IApplicationBuilder MapDigitalTwin(this IApplicationBuilder app)
    {
        return app;
    }
    public static IApplicationBuilder UseSymbolizer(this IApplicationBuilder app) => app.UseMiddleware<Symbolizer>();
    sealed class Symbolizer
    {
        readonly RequestDelegate _request;
        public Symbolizer(RequestDelegate request) => _request = request;
        public async Task Invoke(HttpContext context)
        {
            context.Response.Headers.Add("X-Frame-Options", "*");
            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            context.Response.Headers.Add("Access-Control-Allow-Headers", "*");
            context.Response.Headers.Add("Access-Control-Allow-Methods", "*");
            context.Response.Headers.Add("Access-Control-Expose-Headers", "*");
            context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
            context.Response.ContentType = MediaTypeNames.Application.Json;
            await _request(context);
        }
    }
    static List<string> Histories => new();
}