namespace Station.Application.Contract.Triggers;

[ServiceContract(Name = Sign.ManufacturerName, Namespace = Sign.Namespace)]
public interface IWebsiteTrigger
{
    [OperationContract(AsyncPattern = true, Name = "invokeSrv", ReplyAction = "*")] Task<string> InvokeAsync(string text);
}