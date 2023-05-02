namespace Station.Domain.Functions.Hosts;
public sealed class TemplateEngine : StandardServer
{
    protected override MasterNodeManager CreateMasterNodeManager(IServerInternal server, ApplicationConfiguration configuration)
    {
        return new MasterNodeManager(server, configuration, dynamicNamespaceUri: default, new[]
        {
            new NodeManager(server, configuration)
        });
    }
    protected override ServerProperties LoadServerProperties() => new()
    {
        ProductUri = Sign.Namespace,
        ProductName = Sign.ProductName,
        ManufacturerName = Sign.ManufacturerName,
        SoftwareVersion = Utils.GetAssemblySoftwareVersion(),
        BuildNumber = Utils.GetAssemblyBuildNumber(),
        BuildDate = Utils.GetAssemblyTimestamp()
    };
    protected override ResourceManager CreateResourceManager(IServerInternal server, ApplicationConfiguration configuration)
    {//為服務器創建資源管理器
        ResourceManager resourceManager = new(server, configuration);
        var fields = typeof(StatusCodes).GetFields(BindingFlags.Public | BindingFlags.Static);
        foreach (var field in fields)
        {
            var id = field.GetValue(typeof(StatusCodes)) as uint?;
            if (id is not null) resourceManager.Add(id.Value, "en-US", field.Name);
        }
        return resourceManager;
    }
    protected override void OnServerStarting(ApplicationConfiguration configuration)
    {//服務器啟動前調用
        base.OnServerStarting(configuration);

        //創建用於驗證服務器支持的用戶身份令牌的對象
        for (int item = default; item < configuration.ServerConfiguration.UserTokenPolicies.Count; item++)
        {
            var policy = configuration.ServerConfiguration.UserTokenPolicies[item];

            //為證書令牌策略創建驗證器
            if (policy.TokenType == UserTokenType.Certificate)
            {
                //檢查是否在配置中指定了用戶證書信任列表
                if (configuration.SecurityConfiguration.TrustedUserCertificates is not null && configuration.SecurityConfiguration.UserIssuerCertificates is not null)
                {
                    CertificateValidator certificateValidator = new();
                    certificateValidator.Update(configuration.SecurityConfiguration).Wait();
                    certificateValidator.Update(configuration.SecurityConfiguration.UserIssuerCertificates,
                        configuration.SecurityConfiguration.TrustedUserCertificates,
                        configuration.SecurityConfiguration.RejectedCertificateStore);

                    //為用戶證書設置自定義驗證器
                    UserCertificateValidator = certificateValidator.GetChannelValidator();
                }
            }
        }
    }
    protected override void OnServerStarted(IServerInternal server)
    {//服務器啟動後調用
        base.OnServerStarted(server);
        server.SessionManager.ImpersonateUser += new ImpersonateEventHandler((Session session, ImpersonateEventArgs args) =>
        {//當用戶身份更改時請求通知。 默認接受所有有效用戶
            //檢查用戶名令牌
            if (args.NewIdentity is UserNameIdentityToken userNameToken)
            {
                args.Identity = VerifyPassword(userNameToken);

                //為接受的用戶/密碼身份驗證設置 AuthenticatedUser 角色
                args.Identity.GrantedRoleIds.Add(ObjectIds.WellKnownRole_AuthenticatedUser);
                if (args.Identity is SystemConfigurationIdentity)
                {
                    //為有權配置服務器的用戶設置 ConfigureAdmin 角色
                    args.Identity.GrantedRoleIds.Add(ObjectIds.WellKnownRole_ConfigureAdmin);
                    args.Identity.GrantedRoleIds.Add(ObjectIds.WellKnownRole_SecurityAdmin);
                }
                return;
            }

            //檢查 x509 用戶令牌
            if (args.NewIdentity is X509IdentityToken x509Token)
            {
                VerifyUserTokenCertificate(x509Token.Certificate);
                args.Identity = new UserIdentity(x509Token);
                Utils.Trace("X509 Token Accepted: {0}", args.Identity.DisplayName);

                //為接受的證書身份驗證設置 AuthenticatedUser 角色
                args.Identity.GrantedRoleIds.Add(ObjectIds.WellKnownRole_AuthenticatedUser);
                return;
            }

            //允許匿名身份驗證並為此身份驗證設置匿名角色
            args.Identity = new UserIdentity();
            args.Identity.GrantedRoleIds.Add(ObjectIds.WellKnownRole_Anonymous);
        });
        IUserIdentity VerifyPassword(UserNameIdentityToken userNameToken)
        {//驗證用戶名令牌的密碼
            var userName = userNameToken.UserName;

            var password = userNameToken.DecryptedPassword;

            if (string.IsNullOrEmpty(userName))
            {
                //不接受空用戶名
                throw ServiceResultException.Create(StatusCodes.BadIdentityTokenInvalid,
                    "Security token is not a valid username token. An empty username is not accepted.");
            }
            if (string.IsNullOrEmpty(password))
            {
                //不接受空密碼
                throw ServiceResultException.Create(StatusCodes.BadIdentityTokenRejected,
                    "Security token is not a valid username token. An empty password is not accepted.");
            }

            //有權配置服務器的用戶
            if (userName == "sysadmin" && password == "demo")
            {
                return new SystemConfigurationIdentity(new UserIdentity(userNameToken));
            }

            //CTT驗證標準用戶
            if (!(userName == "user1" && password == "password" || userName == "user2" && password == "password1"))
            {
                //使用默認文本構造翻譯對象
                TranslationInfo info = new("InvalidPassword", "en-US", "Invalid username or password.", userName);

                //使用供應商定義的子代碼創建異常
                throw new ServiceResultException(new ServiceResult(StatusCodes.BadUserAccessDenied, "InvalidPassword", LoadServerProperties().ProductUri, new LocalizedText(info)));
            }
            return new UserIdentity(userNameToken);
        }
        void VerifyUserTokenCertificate(X509Certificate2 certificate)
        {//驗證證書用戶令牌是否可信
            try
            {
                if (UserCertificateValidator is not null)
                {
                    UserCertificateValidator.Validate(certificate);
                }
                else
                {
                    CertificateValidator.Validate(certificate);
                }
            }
            catch (Exception e)
            {
                TranslationInfo info;
                StatusCode result = StatusCodes.BadIdentityTokenRejected;
                if (e is ServiceResultException se && se.StatusCode == StatusCodes.BadCertificateUseNotAllowed)
                {
                    info = new TranslationInfo("InvalidCertificate", "en-US", "'{0}' is an invalid user certificate.", certificate.Subject);
                    result = StatusCodes.BadIdentityTokenInvalid;
                }
                else
                {//使用默認文本構造翻譯對象
                    info = new TranslationInfo("UntrustedCertificate", "en-US", "'{0}' is not a trusted user certificate.", certificate.Subject);
                }

                //使用供應商定義的子代碼創建異常
                throw new ServiceResultException(new ServiceResult(result, info.Key, LoadServerProperties().ProductUri, new LocalizedText(info)));
            }
        }
    }
    ICertificateValidator? UserCertificateValidator { get; set; }
}