using Kuzaine.Helpers;
using Kuzaine.Services;


namespace Kuzaine.Builders.AuthServer;

public class ScopeFactoryBuilder
{
    private readonly IKuzaineUtilities _utilities;

    public ScopeFactoryBuilder(IKuzaineUtilities utilities)
    {
        _utilities = utilities;
    }

    public void Create(string solutionDirectory, string projectBaseName)
    {
        var classPath = ClassPathHelper.AuthServerFactoriesClassPath(solutionDirectory, "ScopeFactory.cs", projectBaseName);
        var fileText = GetFileText(classPath.ClassNamespace);
        _utilities.CreateFile(classPath, fileText);
    }

    private static string GetFileText(string classNamespace)
    {
        return @$"namespace {classNamespace};

using Pulumi;
using Pulumi.Keycloak.OpenId;

public class ScopeFactory
{{
    public static ClientScope CreateScope(Output<string> realmId, string scopeName)
    {{
        return new ClientScope($""{{scopeName}}-scope"", new ClientScopeArgs()
        {{
            Name = scopeName,
            RealmId = realmId,
        }});
    }}
}}";
    }
}
