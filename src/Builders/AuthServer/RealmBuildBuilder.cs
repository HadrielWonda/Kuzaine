using Kuzaine.Helpers;
using Kuzaine.Services;
using Domain;
using Domain.Enums;


namespace Kuzaine.Builders.AuthServer;

public class RealmBuildBuilder
{
    private readonly IKuzaineUtilities _utilities;

    public RealmBuildBuilder(IKuzaineUtilities utilities)
    {
        _utilities = utilities;
    }

    public void Create(string solutionDirectory, string projectBaseName, string templateName, List<AuthServerTemplate.AuthClient> clients)
    {
        var classPath = ClassPathHelper.AuthServerProjectRootClassPath(solutionDirectory, "RealmBuild.cs", projectBaseName);
        var fileText = GetFileText(classPath.ClassNamespace, templateName, clients, solutionDirectory, projectBaseName);
        _utilities.CreateFile(classPath, fileText);
    }

    private static string GetFileText(string classNamespace, string realmName, List<AuthServerTemplate.AuthClient> clients, string solutionDirectory, string projectBaseName)
    {
        var realm = @$"var realm = new Realm(""{realmName}-realm"", new RealmArgs
        {{
            RealmName = ""{realmName}"",
            RegistrationAllowed = true,
            ResetPasswordAllowed = true,
            RememberMe = true,
            EditUsernameAllowed = true
        }});";
        var clientsString = "";
        
        var scopesString = "";
        var clientScopes = clients
            .SelectMany(x => x.Scopes)
            .Distinct()
            .ToList();

        var scopesToAdd = new List<string>();
        scopesToAdd.AddRange(clientScopes);
        scopesToAdd = scopesToAdd.Distinct().ToList();
        scopesToAdd.ForEach(scope =>
        {
            var scopeVar = GetScopeVarName(scope);
            scopesString += $@"
        var {scopeVar} = ScopeFactory.CreateScope(realm.Id, ""{scope}"");";
        });
        
        foreach (AuthServerTemplate.AuthClient client in clients)
        {
            clientsString += GetNewClientString(client);
        }

        var extensionsClassPath = ClassPathHelper.AuthServerExtensionsClassPath(solutionDirectory, "", projectBaseName);
        var factoryClassPath = ClassPathHelper.AuthServerFactoriesClassPath(solutionDirectory, "", projectBaseName);
        
        return @$"namespace {classNamespace};

using {extensionsClassPath.ClassNamespace};
using {factoryClassPath.ClassNamespace};
using Pulumi;
using Pulumi.Keycloak;
using Pulumi.Keycloak.Inputs;

class RealmBuild : Stack
{{
    public RealmBuild()
    {{
        {realm}{scopesString}{clientsString}
        
        var bob = new User(""bob"", new UserArgs
        {{
            RealmId = realm.Id,
            Username = ""bob"",
            Enabled = true,
            Email = ""bob@domain.com"",
            FirstName = ""Smith"",
            LastName = ""Bobson"",
            InitialPassword = new UserInitialPasswordArgs
            {{
                Value = ""bob"",
                Temporary = true,
            }},
        }});

        var alice = new User(""alice"", new UserArgs
        {{
            RealmId = realm.Id,
            Username = ""alice"",
            Enabled = true,
            Email = ""alice@domain.com"",
            FirstName = ""Alice"",
            LastName = ""Smith"",
            InitialPassword = new UserInitialPasswordArgs
            {{
                Value = ""alice"",
                Temporary = true,
            }},
        }});
    }}
}}";
    }

    private static string GetScopeVarName(string scope)
    {
        return $"{scope}Scope".LowercaseFirstLetter()
            .Replace("-", "")
            .Replace("_", "")
            .Replace(".", "")
            .Replace(":", "");
    }

    private static string GetNewClientString(AuthServerTemplate.AuthClient client)
    {
        var clientVar = $"{client.Name.Replace(" ", "")}Client".LowercaseFirstLetter();

        string redirectUris = GetRedirectUris(client);
        string webOrigins = GetCors(client.AllowedCorsOrigins);

        var scopeStringList = client.Scopes.Select(scope => $@"{GetScopeVarName(scope)}.Name");
        var clientScopesToAdd = string.Join(",", scopeStringList);
        
        var mapperString = string.Join("", client.Scopes.Select(scope => $@"
        {clientVar}.AddAudienceMapper(""{scope}"");"));

        var clientsString = client.GrantType == GrantType.Code.Name
            ? $@"
        
        var {clientVar} = ClientFactory.CreateCodeFlowClient(realm.Id,
            ""{client.Id}"", 
            ""{client.Secret}"", 
            ""{client.Name}"",
            ""{client.BaseUrl}"",
            {redirectUris},
            {webOrigins}
            );
        {clientVar}.ExtendDefaultScopes({clientScopesToAdd});{mapperString}"
            : $@"
        
        var {clientVar} = ClientFactory.CreateClientCredentialsFlowClient(realm.Id,
            ""{client.Id}"", 
            ""{client.Secret}"", 
            ""{client.Name}"",
            ""{client.BaseUrl}"");
        {clientVar}.ExtendDefaultScopes({clientScopesToAdd});{mapperString}";
        return clientsString;
    }

    private static string GetRedirectUris(AuthServerTemplate.AuthClient client)
    {
        var redirectUrisString = "";
        client.RedirectUris.ForEach(uri =>
        {
            if (uri.EndsWith("/"))
                uri = uri.Substring(0, uri.Length - 1);
            
            redirectUrisString += $@"
                ""{uri}"",";
        });
        var redirectUris = client.RedirectUris.Count <= 0
            ? "redirectUris: null"
            : @$"redirectUris: new InputList<string>() 
                {{{redirectUrisString}
                }}";
        return redirectUris;
    }

    private static string GetCors(List<string> allowedCors)
    {
        var corsString = "";
        allowedCors.ForEach(uri =>
        {
            if (uri.EndsWith("/"))
                uri = uri.Substring(0, uri.Length - 1);
            corsString += $@"
                ""{uri}"",";
        });
        var cors = allowedCors.Count <= 0
            ? "webOrigins: null"
            : @$"webOrigins: new InputList<string>() 
                {{{corsString}
                }}";
        return cors;
    }
}
