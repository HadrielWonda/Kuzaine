namespace Kuzaine.Builders;

using Helpers;
using Services;

public class ConstsResourceBuilder
{
    private readonly IKuzaineUtilities _utilities;

    public ConstsResourceBuilder(IKuzaineUtilities utilities)
    {
        _utilities = utilities;
    }

    public void CreateLocalConfig(string srcDirectory, string projectBaseName)
    {
        var classPath = ClassPathHelper.WebApiResourcesClassPath(srcDirectory, "Consts.cs", projectBaseName);
        var fileText = GetConfigText(classPath.ClassNamespace);
        _utilities.CreateFile(classPath, fileText);
    }

    private static string GetConfigText(string classNamespace)
    {
        return @$"namespace {classNamespace};

public static class Consts
{{
    public static class Testing
    {{
        public const string IntegrationTestingEnvName = ""LocalIntegrationTesting"";
        public const string FunctionalTestingEnvName = ""LocalFunctionalTesting"";
    }}
}}";
    }
}
