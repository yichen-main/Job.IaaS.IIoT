namespace Platform.Domain.Shared.Services;

[ServiceContract(Name = "FFG-iMDS", Namespace = "https://imds.ffg-tw.com")]
public interface IWebsiteService
{
    [OperationContract(AsyncPattern = true, Name = "invokeSrv", ReplyAction = "*")] Task<string> InvokeAsync(string text);
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class WebsiteService : IWebsiteService
{
    public Task<string> InvokeAsync(string text)
    {
        try
        {
            return Task.FromResult(text);
        }
        catch (Exception e)
        {
            return Task.FromResult(e.Message);
        }
    }
}