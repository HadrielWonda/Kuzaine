namespace Kuzaine.Builders.AuthServer;

using Kuzaine.Helpers;
using Kuzaine.Services;

public class ClientExtensionsBuilder
{
    private readonly IKuzaineUtilities _utilities;

    public ClientExtensionsBuilder(IKuzaineUtilities utilities)
    {
        _utilities = utilities;
    }

    public void Create(string solutionDirectory, string projectBaseName)
    {
        var classPath = ClassPathHelper.AuthServerExtensionsClassPath(solutionDirectory, "ClientExtensions.cs", projectBaseName);
        var fileText = GetFileText(classPath.ClassNamespace);
        _utilities.CreateFile(classPath, fileText);
    }

    private static string GetFileText(string classNamespace)
    {
        return @$"namespace {classNamespace};

using Pulumi;
using Pulumi.Keycloak.OpenId;

public static class ClientExtensions
{{
    public static void ExtendDefaultScopes(this Client client, params Output<string>[] scopeNames)
    {{
        var defaultScopes = client.Name.Apply(clientName =>
            new ClientDefaultScopes($""default-scopes-for-{{clientName}}"", new ClientDefaultScopesArgs()
            {{
                RealmId = client.RealmId,
                ClientId = client.Id,
                DefaultScopes =
                {{
                    ""openid"",
                    ""profile"",
                    ""email"",
                    ""roles"",
                    ""web-origins"",
                    scopeNames,
                }},
            }})
        );
    }}
    
    public static void AddAudienceMapper(this Client client, string audience)
    {{
        var audienceMapper = client.Name.Apply(clientName =>
            new AudienceProtocolMapper($""audienceMapper-{{clientName}}-{{audience}}"", new AudienceProtocolMapperArgs
            {{
                RealmId = client.RealmId,
                ClientId = client.Id,
                IncludedCustomAudience = audience,
                Name = $""{{audience}}-Mapping""
            }})
        );
    }}
}}";
    }
}
