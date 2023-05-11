namespace Platform.Domain.Shared.Services;

[ServiceContract(Name = Sign.ManufacturerName, Namespace = Sign.Namespace)]
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