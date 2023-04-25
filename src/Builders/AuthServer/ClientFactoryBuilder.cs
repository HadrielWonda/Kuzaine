namespace Kuzaine.Builders.AuthServer;

using Kuzaine.Helpers;
using Kuzaine.Services;

public class ClientFactoryBuilder
{
    private readonly IKuzaineUtilities _utilities;

    public ClientFactoryBuilder(IKuzaineUtilities utilities)
    {
        _utilities = utilities;
    }

    public void Create(string solutionDirectory, string projectBaseName)
    {
        var classPath = ClassPathHelper.AuthServerFactoriesClassPath(solutionDirectory, "ClientFactory.cs", projectBaseName);
        var fileText = GetFileText(classPath.ClassNamespace);
        _utilities.CreateFile(classPath, fileText);
    }

    private static string GetFileText(string classNamespace)
    {
        return @$"namespace {classNamespace};

using Pulumi;
using Pulumi.Keycloak.OpenId;

public class ClientFactory
{{
    public static Client CreateCodeFlowClient(Output<string> realmId, 
        string clientId, 
        string clientSecret, 
        string clientName, 
        string baseUrl,
        InputList<string>? redirectUris = null,
        InputList<string>? webOrigins = null)
    {{
        return new Client($""{{clientName.ToLower()}}-client"", new ClientArgs()
        {{
            RealmId = realmId,
            ClientId = clientId,
            Name = clientName,
            StandardFlowEnabled = true,
            Enabled = true,
            AccessType = ""CONFIDENTIAL"",
            BaseUrl = baseUrl,
            AdminUrl = baseUrl,
            ValidRedirectUris = redirectUris ?? new InputList<string>()
            {{
                new Uri(new Uri(baseUrl), ""*"").ToString()
            }},
            WebOrigins = webOrigins ?? new InputList<string>()
            {{
                baseUrl
            }},
            PkceCodeChallengeMethod = ""S256"",
            ClientSecret = clientSecret,
            BackchannelLogoutSessionRequired = true,
            BackchannelLogoutUrl = baseUrl
        }});
    }}
    
    public static Client CreateClientCredentialsFlowClient(Output<string> realmId, 
        string clientId, 
        string clientSecret, 
        string clientName, 
        string baseUrl)
    {{
        return new Client($""{{clientName.ToLower()}}-client"", new ClientArgs()
        {{
            RealmId = realmId,
            ClientId = clientId,
            Name = clientName,
            StandardFlowEnabled = false,
            Enabled = true,
            ServiceAccountsEnabled = true,
            AccessType = ""CONFIDENTIAL"",
            BaseUrl = baseUrl,
            AdminUrl = baseUrl,
            ClientSecret = clientSecret,
            BackchannelLogoutSessionRequired = true,
            BackchannelLogoutUrl = baseUrl
        }});
    }}
}}";
    }
}
