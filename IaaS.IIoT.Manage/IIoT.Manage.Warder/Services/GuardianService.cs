namespace Manage.Warder.Services;
internal sealed class GuardianService : BackgroundService
{
    readonly IHttpClientFactory _httpClientFactory;
    public GuardianService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await new PeriodicTimer(TimeSpan.FromSeconds(1)).WaitForNextTickAsync(stoppingToken))
        {
            try
            {

                var gg = _httpClientFactory;
            }
            catch (Exception e)
            {
                Log.Error("[{0}] {1}", nameof(GuardianService), new { e.Message, e.StackTrace });
            }
        }
    }
    public async Task OnGet()
    {//https://learn.microsoft.com/zh-tw/aspnet/core/fundamentals/http-requests?view=aspnetcore-7.0
     //https://www.cnblogs.com/willick/p/net-core-httpclient.html
     //https://mp.weixin.qq.com/s/9INm3kWXMEYEJfevjUZt6A
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri("https://imds.mynetgear.com:53080");
        httpClient.DefaultRequestVersion = HttpVersion.Version20;
        httpClient.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower;

        // HTTP/2
        using (var resopnse = await httpClient.GetAsync("/feeler"))
        {
            var test = resopnse.Content;
        }

        var httpResponseMessage = await httpClient.SendAsync(new HttpRequestMessage(
        HttpMethod.Get, "https://api.github.com/repos/dotnet/AspNetCore.Docs/branches")
        {
            Headers =
            {
                { HeaderNames.Accept, "application/vnd.github.v3+json" },
                { HeaderNames.UserAgent, "HttpRequestsSample" }
            }
        });

        if (httpResponseMessage.IsSuccessStatusCode)
        {
            await using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();

            // GitHubBranches = await JsonSerializer.DeserializeAsync<IEnumerable<GitHubBranch>>(contentStream);
        }
    }
}