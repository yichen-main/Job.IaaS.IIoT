namespace Platform.Application.Endpoints.Machines;
public static class DemoEndpoints
{
    public static void MapDemoEndpoint(this IEndpointRouteBuilder routes)
    {
        //var group = routes.MapGroup("/api/123").WithTags(na)

        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        routes.MapGet("/weatherforecast", () =>
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            )).ToArray();
        }).WithName("GetWeatherForecast").WithOpenApi();
    }
    internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}