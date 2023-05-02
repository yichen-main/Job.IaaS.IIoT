namespace Station.Application.Additions.Triggers;
public sealed class WebsiteTrigger : IWebsiteTrigger
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