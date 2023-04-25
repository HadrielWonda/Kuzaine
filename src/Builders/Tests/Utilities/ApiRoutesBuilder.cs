namespace Kuzaine.Builders.Tests.Utilities;

using Helpers;
using Services;

public class ApiRoutesBuilder
{
    private readonly IKuzaineUtilities _utilities;

    public ApiRoutesBuilder(IKuzaineUtilities utilities)
    {
        _utilities = utilities;
    }

    public void CreateClass(string testDirectory, string projectBaseName)
    {
        var classPath = ClassPathHelper.FunctionalTestUtilitiesClassPath(testDirectory, projectBaseName, "ApiRoutes.cs");
        var fileText = GetBaseText(classPath.ClassNamespace);
        _utilities.CreateFile(classPath, fileText);
    }

    private static string GetBaseText(string classNamespace)
    {
        return @$"namespace {classNamespace};
public class ApiRoutes
{{
    public const string Base = ""api"";
    public const string Health = Base + ""/health"";

    // new api route marker - do not delete
}}";
    }
}
