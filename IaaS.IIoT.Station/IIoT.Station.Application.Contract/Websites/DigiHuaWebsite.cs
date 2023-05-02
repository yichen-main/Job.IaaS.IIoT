namespace Station.Application.Contract.Websites;

[GeneratedCode("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
[ServiceContract(Name = "wsEAISoap", Namespace = "http://www.imestech.com/wsEAI", ConfigurationName = "Newsies.IManufactureClient")]
public interface IDigiHuaWebsite
{
    [OperationContract(Name = "invokeSrvAsync", Action = "http://www.imestech.com/wsEAI/invokeSrv", ReplyAction = "*")] Task<Response> InvokeSrvAsync(Request request);
    [OperationContract(Name = "invokeSrv_ResolvedXMLAsync", Action = "http://www.imestech.com/wsEAI/invokeSrv_ResolvedXML", ReplyAction = "*")] Task<ResolvedResponse> ResolvedAsync(ResolvedRequest request);
    [OperationContract(Name = "callbackSrvAsync", Action = "http://www.imestech.com/wsEAI/callbackSrv", ReplyAction = "*")] Task<CallbackResponse> CallbackAsync(CallbackRequest request);
    [OperationContract(Name = "syncProdAsync", Action = "http://www.imestech.com/wsEAI/syncProd", ReplyAction = "*")] Task<ProdResponse> ProdAsync(ProdRequest request);
    readonly ref struct Label
    {
        public const string Srvver = "1.0";
        public const string Failure = "100";
        public const string Success = "000";
        public const string Protocol = "raw";
        public const string TextType = "xml";
        public const string ParamType = "data";
        public const string ProdName = "WEBACCESS";
        public const string Name = "manufacturing-processes";
        public const string StandardData = "std_data";
        public const string QueryResult = "query_result";
        public const string QueryResultData = "query_result_data";
    }
}

[GeneratedCode("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
[DebuggerStepThrough(), MessageContract(IsWrapped = false), EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed partial class Request
{
    public Request() { }
    public Request(RequestBody body) => Body = body;
    [MessageBodyMember(Name = "invokeSrv", Namespace = "http://www.imestech.com/wsEAI", Order = 0)] public required RequestBody Body { get; init; }
}

[GeneratedCode("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
[DebuggerStepThrough(), DataContract(Namespace = "http://www.imestech.com/wsEAI"), EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed partial class RequestBody
{
    public RequestBody() { }
    public RequestBody(string inXml) => InXml = inXml;
    [DataMember(EmitDefaultValue = false, Order = 0)] public required string InXml { get; init; }
}

[GeneratedCode("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
[DebuggerStepThrough(), MessageContract(IsWrapped = false), EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed partial class Response
{
    public Response() { }
    public Response(ResponseBody body) => Body = body;
    [MessageBodyMember(Name = "invokeSrvResponse", Namespace = "http://www.imestech.com/wsEAI", Order = 0)] public required ResponseBody Body { get; init; }
}

[GeneratedCode("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
[DebuggerStepThrough(), DataContract(Namespace = "http://www.imestech.com/wsEAI"), EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed partial class ResponseBody
{
    public ResponseBody() { }
    public ResponseBody(string result) => Result = result;
    [DataMember(Name = "invokeSrvResult", EmitDefaultValue = false, Order = 0)] public required string Result { get; init; }
}

[GeneratedCode("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
[DebuggerStepThrough(), MessageContract(IsWrapped = false), EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed partial class ResolvedRequest
{
    public ResolvedRequest() { }
    public ResolvedRequest(ResolvedRequestBody body) => Body = body;
    [MessageBodyMember(Name = "invokeSrv_ResolvedXML", Namespace = "http://www.imestech.com/wsEAI", Order = 0)] public required ResolvedRequestBody Body { get; init; }
}

[GeneratedCode("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
[DebuggerStepThrough(), DataContract(Namespace = "http://www.imestech.com/wsEAI"), EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed partial class ResolvedRequestBody
{
    public ResolvedRequestBody() { }
    public ResolvedRequestBody(string methodName, string inXml)
    {
        MethodName = methodName;
        InXml = inXml;
    }
    [DataMember(EmitDefaultValue = false, Order = 0)] public required string MethodName { get; init; }
    [DataMember(EmitDefaultValue = false, Order = 1)] public required string InXml { get; init; }
}

[GeneratedCode("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
[DebuggerStepThrough(), MessageContract(IsWrapped = false), EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed partial class ResolvedResponse
{
    public ResolvedResponse() { }
    public ResolvedResponse(ResolvedResponseBody body) => Body = body;
    [MessageBodyMember(Name = "invokeSrv_ResolvedXMLResponse", Namespace = "http://www.imestech.com/wsEAI", Order = 0)] public required ResolvedResponseBody Body { get; init; }
}

[GeneratedCode("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
[DebuggerStepThrough(), DataContract(Namespace = "http://www.imestech.com/wsEAI"), EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed partial class ResolvedResponseBody
{
    public ResolvedResponseBody() { }
    public ResolvedResponseBody(string resolvedResult) => ResolvedResult = resolvedResult;
    [DataMember(EmitDefaultValue = false, Order = 0)] public required string ResolvedResult { get; init; }
}

[GeneratedCode("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
[DebuggerStepThrough(), MessageContract(IsWrapped = false), EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed partial class CallbackRequest
{
    public CallbackRequest() { }
    public CallbackRequest(CallbackRequestBody body) => Body = body;
    [MessageBodyMember(Name = "callbackSrv", Namespace = "http://www.imestech.com/wsEAI", Order = 0)] public required CallbackRequestBody Body { get; init; }
}

[GeneratedCode("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
[DebuggerStepThrough(), DataContract(Namespace = "http://www.imestech.com/wsEAI"), EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed partial class CallbackRequestBody
{
    public CallbackRequestBody() { }
    public CallbackRequestBody(string inXml) => InXml = inXml;
    [DataMember(EmitDefaultValue = false, Order = 0)] public required string InXml { get; init; }
}

[GeneratedCode("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
[DebuggerStepThrough(), MessageContract(IsWrapped = false), EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed partial class CallbackResponse
{
    public CallbackResponse() { }
    public CallbackResponse(CallbackResponseBody body) => Body = body;
    [MessageBodyMember(Name = "callbackSrvResponse", Namespace = "http://www.imestech.com/wsEAI", Order = 0)] public required CallbackResponseBody Body { get; init; }
}

[GeneratedCode("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
[DebuggerStepThrough(), DataContract(Namespace = "http://www.imestech.com/wsEAI"), EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed partial class CallbackResponseBody
{
    public CallbackResponseBody() { }
    public CallbackResponseBody(string callbackResult) => CallbackResult = callbackResult;
    [DataMember(EmitDefaultValue = false, Order = 0)] public required string CallbackResult { get; init; }
}

[GeneratedCode("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
[DebuggerStepThrough(), MessageContract(IsWrapped = false), EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed partial class ProdRequest
{
    public ProdRequest() { }
    public ProdRequest(ProdRequestBody body) => Body = body;
    [MessageBodyMember(Name = "syncProd", Namespace = "http://www.imestech.com/wsEAI", Order = 0)] public required ProdRequestBody Body { get; init; }
}

[GeneratedCode("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
[DebuggerStepThrough(), DataContract(Namespace = "http://www.imestech.com/wsEAI"), EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed partial class ProdRequestBody
{
    public ProdRequestBody() { }
    public ProdRequestBody(string inXml) => InXml = inXml;
    [DataMember(EmitDefaultValue = false, Order = 0)] public required string InXml { get; init; }
}

[GeneratedCode("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
[DebuggerStepThrough(), MessageContract(IsWrapped = false), EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed partial class ProdResponse
{
    public ProdResponse() { }
    public ProdResponse(ProdResponseBody body) => Body = body;
    [MessageBodyMember(Name = "syncProdResponse", Namespace = "http://www.imestech.com/wsEAI", Order = 0)] public required ProdResponseBody Body { get; init; }
}

[GeneratedCode("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
[DebuggerStepThrough(), DataContract(Namespace = "http://www.imestech.com/wsEAI"), EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed partial class ProdResponseBody
{
    public ProdResponseBody() { }
    public ProdResponseBody(string prodResult) => ProdResult = prodResult;
    [DataMember(EmitDefaultValue = false, Order = 0)] public required string ProdResult { get; init; }
}
[GeneratedCode("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")] public interface IManufactureChannel : IDigiHuaWebsite, IClientChannel { }

[DebuggerStepThrough(), GeneratedCode("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
public partial class DigihuaWebsite : ClientBase<IDigiHuaWebsite>, IDigiHuaWebsite
{
    static partial void ConfigureEndpoint(ServiceEndpoint serviceEndpoint, ClientCredentials clientCredentials);
    public DigihuaWebsite(EndpointConfiguration endpointConfiguration, string remoteAddress) : base(GetBindingForEndpoint(endpointConfiguration), new EndpointAddress(remoteAddress))
    {
        Endpoint.Name = endpointConfiguration.ToString();
        ConfigureEndpoint(Endpoint, ClientCredentials);
    }
    public DigihuaWebsite(Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress) { }
    [EditorBrowsable(EditorBrowsableState.Advanced)] Task<Response> IDigiHuaWebsite.InvokeSrvAsync(Request request) => Channel.InvokeSrvAsync(request);
    public Task<Response> LinkAsync(string inXml) => ((IDigiHuaWebsite)this).InvokeSrvAsync(new Request()
    {
        Body = new RequestBody
        {
            InXml = inXml
        }
    });
    [EditorBrowsable(EditorBrowsableState.Advanced)] Task<ResolvedResponse> IDigiHuaWebsite.ResolvedAsync(ResolvedRequest request) => Channel.ResolvedAsync(request);
    public Task<ResolvedResponse> invokeSrv_ResolvedXMLAsync(string methodName, string inXml) => ((IDigiHuaWebsite)this).ResolvedAsync(new()
    {
        Body = new()
        {
            MethodName = methodName,
            InXml = inXml
        }
    });

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    Task<CallbackResponse> IDigiHuaWebsite.CallbackAsync(CallbackRequest request) => Channel.CallbackAsync(request);
    public Task<CallbackResponse> CallbackAsync(string inXml) => ((IDigiHuaWebsite)this).CallbackAsync(new()
    {
        Body = new()
        {
            InXml = inXml
        }
    });

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    Task<ProdResponse> IDigiHuaWebsite.ProdAsync(ProdRequest request) => Channel.ProdAsync(request);
    public Task<ProdResponse> syncProdAsync(string inXml) => ((IDigiHuaWebsite)this).ProdAsync(new()
    {
        Body = new()
        {
            InXml = inXml
        }
    });
    public virtual Task OpenAsync() => Task.Factory.FromAsync(((ICommunicationObject)this).BeginOpen(null, null), new Action<IAsyncResult>(((ICommunicationObject)this).EndOpen));
    private static Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration)
    {
        if (endpointConfiguration is EndpointConfiguration.wsEAISoap)
        {
            BasicHttpBinding result = new();
            result.MaxBufferSize = int.MaxValue;
            result.ReaderQuotas = XmlDictionaryReaderQuotas.Max;
            result.MaxReceivedMessageSize = int.MaxValue;
            result.AllowCookies = true;
            return result;
        }
        if (endpointConfiguration is EndpointConfiguration.wsEAISoap12)
        {
            CustomBinding result = new();
            TextMessageEncodingBindingElement textBindingElement = new();
            textBindingElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
            result.Elements.Add(textBindingElement);
            HttpTransportBindingElement httpBindingElement = new();
            httpBindingElement.AllowCookies = true;
            httpBindingElement.MaxBufferSize = int.MaxValue;
            httpBindingElement.MaxReceivedMessageSize = int.MaxValue;
            result.Elements.Add(httpBindingElement);
            return result;
        }
        throw new InvalidOperationException(string.Format("Could not find an endpoint named \'{0}\'", endpointConfiguration));
    }
    public enum EndpointConfiguration
    {
        wsEAISoap,
        wsEAISoap12,
    }

    [XmlRoot(ElementName = "request")]
    public sealed class EaiRequest
    {
        [XmlElement(ElementName = "host")] public TextHost? Host { get; set; }
        [XmlElement(ElementName = "service")] public TextService? Service { get; set; }
        [XmlElement(ElementName = "payload")] public TextPayload? Payload { get; set; }
        [XmlAttribute(AttributeName = "key")] public string? Key { get; set; }
        [XmlAttribute(AttributeName = "type")] public string? Type { get; set; }

        [XmlRoot(ElementName = "host")]
        public sealed class TextHost
        {
            [XmlAttribute(AttributeName = "prod")] public string? Prod { get; set; }
            [XmlAttribute(AttributeName = "ver")] public string? Ver { get; set; }
            [XmlAttribute(AttributeName = "ip")] public string? Ip { get; set; }
            [XmlAttribute(AttributeName = "id")] public string? Id { get; set; }
            [XmlAttribute(AttributeName = "acct")] public string? Acct { get; set; }
            [XmlAttribute(AttributeName = "lang")] public string? Lang { get; set; }
            [XmlAttribute(AttributeName = "timezone")] public string? Timezone { get; set; }
            [XmlAttribute(AttributeName = "timestamp")] public string? Timestamp { get; set; }
        }

        [XmlRoot(ElementName = "service")]
        public sealed class TextService
        {
            [XmlAttribute(AttributeName = "prod")] public string? Prod { get; set; }
            [XmlAttribute(AttributeName = "name")] public string? Name { get; set; }
            [XmlAttribute(AttributeName = "srvver")] public string? Srvver { get; set; }
            [XmlAttribute(AttributeName = "ip")] public string? Ip { get; set; }
            [XmlAttribute(AttributeName = "id")] public string? Id { get; set; }
        }

        [XmlRoot(ElementName = "payload")]
        public sealed class TextPayload
        {
            [XmlElement(ElementName = "param")] public Param? Param { get; set; }
        }

        [XmlRoot(ElementName = "param")]
        public sealed class Param
        {
            [XmlElement(ElementName = "data_request")] public DataRequest? DataRequest { get; set; }
            [XmlAttribute(AttributeName = "key")] public string? Key { get; set; }
            [XmlAttribute(AttributeName = "type")] public string? Type { get; set; }
        }

        [XmlRoot(ElementName = "data_request")]
        public sealed class DataRequest
        {
            [XmlElement(ElementName = "datainfo")] public Datainfo? Datainfo { get; set; }
        }

        [XmlRoot(ElementName = "datainfo")]
        public sealed class Datainfo
        {
            [XmlElement(ElementName = "parameter")] public DataParameter? DataParameter { get; set; }
        }

        [XmlRoot(ElementName = "parameter")]
        public sealed class DataParameter
        {
            [XmlElement(ElementName = "data")] public Data? Data { get; set; }
            [XmlAttribute(AttributeName = "key")] public string? Key { get; set; }
            [XmlAttribute(AttributeName = "type")] public string? Type { get; set; }
        }

        [XmlRoot(ElementName = "data")]
        public sealed class Data
        {
            [XmlElement(ElementName = "row")] public List<ParameterRow>? ParameterRows { get; set; }
            [XmlAttribute(AttributeName = "name")] public string? Name { get; set; }
        }

        [XmlRoot(ElementName = "row")]
        public sealed class ParameterRow
        {
            [XmlAttribute(AttributeName = "seq")] public string? Seq { get; set; }
            [XmlElement(ElementName = "field")] public List<Field>? Fields { get; set; }
            [XmlElement(ElementName = "detail")] public Detail? Detail { get; set; }
        }

        [XmlRoot(ElementName = "detail")]
        public sealed class Detail
        {
            [XmlElement(ElementName = "row")] public List<DetailRow>? DetailRows { get; set; }
            [XmlAttribute(AttributeName = "name")] public string? Name { get; set; }
        }

        [XmlRoot(ElementName = "row")]
        public sealed class DetailRow
        {
            [XmlAttribute(AttributeName = "seq")] public string? Seq { get; set; }
            [XmlElement(ElementName = "field")] public List<Field>? Fields { get; set; }
        }

        [XmlRoot(ElementName = "field")]
        public sealed class Field
        {
            [XmlAttribute(AttributeName = "name")] public string? Name { get; set; }
            [XmlAttribute(AttributeName = "type")] public string? Type { get; set; }
            [XmlText] public string? Text { get; set; }
            [XmlAttribute(AttributeName = "format")] public string? Format { get; set; }
        }
    }

    [XmlRoot(ElementName = "response")]
    public sealed class ParticularResponse
    {
        [XmlElement(ElementName = "srvver")] public string? Srvver { get; set; }
        [XmlElement(ElementName = "srvcode")] public string? Srvcode { get; set; }
        [XmlElement(ElementName = "payload")] public TextPayload Payload { get; set; } = new();

        [XmlRoot(ElementName = "payload")]
        public sealed class TextPayload
        {
            [XmlElement(ElementName = "response")] public TextResponse? Response { get; set; }
        }

        [XmlRoot(ElementName = "response")]
        public sealed class TextResponse
        {
            [XmlElement(ElementName = "result")] public string? Result { get; set; }
            [XmlElement(ElementName = "message")] public string? Message { get; set; }
            [XmlElement(ElementName = "returnvalue")] public string? Returnvalue { get; set; }
            [XmlElement(ElementName = "identity")] public TextIdentity? Identity { get; set; }
            [XmlElement(ElementName = "detail")] public TextException? Exception { get; set; }
        }

        [XmlRoot(ElementName = "identity")]
        public sealed class TextIdentity
        {
            [XmlElement(ElementName = "transactionid")] public string? Transactionid { get; set; }
            [XmlElement(ElementName = "moduleid")] public string? Moduleid { get; set; }
            [XmlElement(ElementName = "functionid")] public string? Functionid { get; set; }
            [XmlElement(ElementName = "computername")] public string? Computername { get; set; }
            [XmlElement(ElementName = "curuserno")] public string? Curuserno { get; set; }
            [XmlElement(ElementName = "sendtime")] public string? Sendtime { get; set; }
        }

        [XmlRoot(ElementName = "detail")]
        public sealed class TextException
        {
            [XmlElement(ElementName = "code")] public string? Code { get; set; }
            [XmlElement(ElementName = "sysmsg")] public string? Sysmsg { get; set; }
            [XmlElement(ElementName = "mesmsg")] public string? Mesmsg { get; set; }
            [XmlElement(ElementName = "stack")] public string? Stack { get; set; }
        }
    }

    [XmlRoot(ElementName = "response")]
    public sealed class EaiResponse
    {
        [XmlElement(ElementName = "srvver")] public string? Srvver { get; set; }
        [XmlElement(ElementName = "srvcode")] public string? Srvcode { get; set; }
        [XmlElement(ElementName = "payload")] public TextPayload Payload { get; set; } = new();

        [XmlRoot(ElementName = "payload")]
        public sealed class TextPayload
        {
            [XmlElement(ElementName = "param")] public TextParam Param { get; set; } = new();
        }

        [XmlRoot(ElementName = "param")]
        public sealed class TextParam
        {
            [XmlElement(ElementName = "data_response")] public TextDataResponse DataResponse { get; set; } = new();
            [XmlAttribute(AttributeName = "key")] public string Key { get; set; } = IDigiHuaWebsite.Label.StandardData;
            [XmlAttribute(AttributeName = "type")] public string Type { get; set; } = IDigiHuaWebsite.Label.TextType;
            [XmlText] public string? Text { get; set; }
        }

        [XmlRoot(ElementName = "data_response")]
        public sealed class TextDataResponse
        {
            [XmlElement(ElementName = "execution")] public TextExecution Execution { get; set; } = new();
            [XmlElement(ElementName = "datainfo")] public TextDatainfo? Datainfo { get; set; }
        }

        [XmlRoot(ElementName = "datainfo")]
        public sealed class TextDatainfo
        {
            [XmlElement(ElementName = "parameter")] public TextParameter? Parameter { get; set; }
        }

        [XmlRoot(ElementName = "parameter")]
        public sealed class TextParameter
        {
            [XmlElement(ElementName = "data")] public TextData? Data { get; set; }
            [XmlAttribute(AttributeName = "key")] public string? Key { get; set; }
            [XmlAttribute(AttributeName = "type")] public string? Type { get; set; }
            [XmlText] public string? Text { get; set; }
        }

        [XmlRoot(ElementName = "data")]
        public sealed class TextData
        {
            [XmlElement(ElementName = "row")] public List<TextRow>? Rows { get; set; }
            [XmlAttribute(AttributeName = "name")] public string? Name { get; set; }
            [XmlText] public string? Text { get; set; }
        }

        [XmlRoot(ElementName = "row")]
        public sealed class TextRow
        {
            [XmlElement(ElementName = "field")] public List<TextField>? Field { get; set; }
            [XmlAttribute(AttributeName = "seq")] public int Seq { get; set; }
            [XmlText] public string? Text { get; set; }
        }

        [XmlRoot(ElementName = "field")]
        public sealed class TextField
        {
            [XmlAttribute(AttributeName = "name")] public string Name { get; set; } = string.Empty;
            [XmlAttribute(AttributeName = "type")] public string Type { get; set; } = string.Empty;
            [XmlText] public string? Text { get; set; }
        }

        [XmlRoot(ElementName = "execution")]
        public sealed class TextExecution
        {
            [XmlElement(ElementName = "status")] public TextStatus Status { get; set; } = new();
        }

        [XmlRoot(ElementName = "status")]
        public sealed class TextStatus
        {
            [XmlAttribute(AttributeName = "code")] public string Code { get; set; } = string.Empty;
            [XmlAttribute(AttributeName = "sql_code")] public string SqlCode { get; set; } = string.Empty;
            [XmlAttribute(AttributeName = "description")] public string Description { get; set; } = string.Empty;
        }
    }

    [XmlRoot(ElementName = "request")]
    public sealed class StandardRequest
    {
        [XmlElement(ElementName = "service")] public required TextService Service { get; init; }
        [XmlElement(ElementName = "payload")] public required TextPayload Payload { get; init; }

        [XmlRoot(ElementName = "service")]
        public sealed class TextService
        {
            [XmlAttribute(AttributeName = "name")] public required string Name { get; init; }
            [XmlAttribute(AttributeName = "language")] public required string Language { get; init; }
        }

        [XmlRoot(ElementName = "payload")]
        public sealed class TextPayload
        {
            [XmlAttribute(AttributeName = "name")] public required string Name { get; init; }
            [XmlElement(ElementName = "equipment")] public required List<StandardEquipment> Equipments { get; init; }
        }
    }

    [XmlRoot(ElementName = "response")]
    public sealed class StandardResponse
    {
        [XmlElement(ElementName = "execution")] public required TextExecution Execution { get; init; }
        [XmlElement(ElementName = "payload")] public required TextPayload? Payload { get; init; }

        [XmlRoot(ElementName = "execution")]
        public sealed class TextExecution
        {
            [XmlElement(ElementName = "status")] public required TextStatus Status { get; init; }
        }

        [XmlRoot(ElementName = "status")]
        public sealed class TextStatus
        {
            [XmlAttribute(AttributeName = "code")] public required string Code { get; init; }
            [XmlAttribute(AttributeName = "description")] public required string Description { get; init; }
        }

        [XmlRoot(ElementName = "payload")]
        public sealed class TextPayload
        {
            [XmlElement(ElementName = "equipment")] public required List<StandardEquipment> Equipments { get; init; }
        }
    }

    [XmlRoot(ElementName = "equipment")]
    public sealed class StandardEquipment
    {
        [XmlAttribute(AttributeName = "name")] public required string Name { get; init; }
        [XmlElement(ElementName = "row")] public required List<StandardRow> Rows { get; init; }
    }

    [XmlRoot(ElementName = "row")]
    public sealed class StandardRow
    {
        [XmlAttribute(AttributeName = "seq")] public required string Seq { get; init; }
        [XmlElement(ElementName = "field")] public required List<StandardField> Fields { get; init; }
    }

    [XmlRoot(ElementName = "field")]
    public sealed class StandardField
    {
        [XmlAttribute(AttributeName = "name")] public required string Name { get; init; }
        [XmlAttribute(AttributeName = "type")] public required string Type { get; init; }
        [XmlText] public required string Text { get; init; }
    }
}