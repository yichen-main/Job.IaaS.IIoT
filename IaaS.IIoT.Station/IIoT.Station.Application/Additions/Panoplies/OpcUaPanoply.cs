namespace Station.Application.Additions.Panoplies;
internal sealed class OpcUaPanoply : BackgroundService
{
    readonly IMainProfile _mainProfile;
    readonly IStructuralEngine _structuralEngine;
    readonly ITimeserieWrapper _timeserieWrapper;
    readonly TemplateEngine _templateEngine;
    public OpcUaPanoply(
        IMainProfile mainProfile,
        IStructuralEngine structuralEngine,
        ITimeserieWrapper timeserieWrapper,
        TemplateEngine templateEngine)
    {
        _mainProfile = mainProfile;
        _structuralEngine = structuralEngine;
        _timeserieWrapper = timeserieWrapper;
        _templateEngine = templateEngine;
    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.Run(async () =>
    {
        try
        {
            SpinWait.SpinUntil(() => _structuralEngine.EnableStorage is true);
            ApplicationConfiguration configuration = new()
            {
                ApplicationName = _mainProfile.Text!.MachineID,
                ApplicationUri = Utils.Format($"urn:{Dns.GetHostName()}:OpcUa"),
                ApplicationType = ApplicationType.Server,
                ServerConfiguration = new()
                {
                    BaseAddresses = { $"opc.tcp://localhost:{OpcUa}{Sign.OpcUaPath}" },
                    MinRequestThreadCount = 5,
                    MaxRequestThreadCount = 100,
                    MaxQueuedRequestCount = 200
                },
                SecurityConfiguration = new()
                {
                    ApplicationCertificate = new()
                    {
                        StoreType = "Directory",
                        StorePath = Path.Combine(Local.Native, "OPC Foundation", "CertificateStores", "MachineDefault"),
                        SubjectName = Utils.Format($"CN=OpcUa, DC={Dns.GetHostName()}")
                    },
                    TrustedIssuerCertificates = new()
                    {
                        StoreType = "Directory",
                        StorePath = Path.Combine(Local.Native, "OPC Foundation", "CertificateStores", "UA Certificate Authorities")
                    },
                    TrustedPeerCertificates = new()
                    {
                        StoreType = "Directory",
                        StorePath = Path.Combine(Local.Native, "OPC Foundation", "CertificateStores", "UA Applications")
                    },
                    RejectedCertificateStore = new()
                    {
                        StoreType = "Directory",
                        StorePath = Path.Combine(Local.Native, "OPC Foundation", "CertificateStores", "RejectedCertificates")
                    },
                    AutoAcceptUntrustedCertificates = true,
                    AddAppCertToTrustedStore = true
                },
                TransportConfigurations = new(),
                TransportQuotas = new()
                {
                    OperationTimeout = 15000
                },
                ClientConfiguration = new()
                {
                    DefaultSessionTimeout = 60000
                },
                TraceConfiguration = new()
            };
            await configuration.Validate(ApplicationType.Server);
            if (configuration.SecurityConfiguration.AutoAcceptUntrustedCertificates)
            {
                configuration.CertificateValidator.CertificateValidation += (validator, @event) =>
                {
                    @event.Accept = @event.Error.StatusCode == Opc.Ua.StatusCodes.BadCertificateUntrusted;
                };
            }
            ApplicationInstance application = new()
            {
                ApplicationType = ApplicationType.Server,
                ApplicationConfiguration = configuration
            };
            var isCertificate = await application.CheckApplicationInstanceCertificate(silent: default,
                CertificateFactory.DefaultKeySize, CertificateFactory.DefaultLifeTime).ConfigureAwait(false);
            if (!isCertificate) throw new Exception("Application instance certificate invalid!");
            {
                await application.Start(_templateEngine);
                _templateEngine.CurrentInstance.SessionManager.SessionActivated += ActionViewer;
                _templateEngine.CurrentInstance.SessionManager.SessionClosing += ActionViewer;
                _templateEngine.CurrentInstance.SessionManager.SessionCreated += ActionViewer;
                async void ActionViewer(Session session, SessionEventReason reason) => await _timeserieWrapper.OpcUaRegistrant.InsertAsync(new()
                {
                    Status = reason,
                    SessionName = session.SessionDiagnostics.SessionName
                });
            }
        }
        catch (Exception e)
        {
            Log.Fatal(Menu.Title, nameof(OpcUaPanoply), new
            {
                e.Message,
                e.StackTrace
            });
        }
    }, stoppingToken);
}