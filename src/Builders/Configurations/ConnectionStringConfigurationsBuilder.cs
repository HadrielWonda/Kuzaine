namespace Kuzaine.Builders.Configurations;

using Kuzaine.Helpers;
using Kuzaine.Services;

public class ConnectionStringConfigurationsBuilder
{
    private readonly IKuzaineUtilities _utilities;

    public ConnectionStringConfigurationsBuilder(IKuzaineUtilities utilities)
    {
        _utilities = utilities;
    }

    public void CreateConfig(string srcDirectory, string projectBaseName)
    {
        var classPath = ClassPathHelper.WebApiConfigurationsClassPath(srcDirectory, $"{FileNames.ConnectionStringOptions()}.cs", projectBaseName);
        var fileText = GetConfigText(classPath.ClassNamespace, projectBaseName);
        _utilities.CreateFile(classPath, fileText);
    }

    private static string GetConfigText(string classNamespace, string projectBaseName)
    {
        return @$"namespace {classNamespace};

public class {FileNames.ConnectionStringOptions()}
{{
    public const string SectionName = ""ConnectionStrings"";
    public const string {FileNames.ConnectionStringOptionKey(projectBaseName)} = nameof({projectBaseName});

    public string {projectBaseName} {{ get; set; }} = String.Empty;
}}

public static class ConnectionStringOptionsExtensions
{{
    public static ConnectionStringOptions GetConnectionStringOptions(this IConfiguration configuration)
        => configuration.GetSection(ConnectionStringOptions.SectionName).Get<ConnectionStringOptions>();
}}";
    }
}
