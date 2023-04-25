namespace Kuzaine.Builders.Configurations;

using Kuzaine.Helpers;
using Kuzaine.Services;

public class AuthConfigurationsBuilder
{
    private readonly IKuzaineUtilities _utilities;

    public AuthConfigurationsBuilder(IKuzaineUtilities utilities)
    {
        _utilities = utilities;
    }

    public void CreateConfig(string srcDirectory, string projectBaseName)
    {
        var classPath = ClassPathHelper.WebApiConfigurationsClassPath(srcDirectory, $"{FileNames.AuthOptions()}.cs", projectBaseName);
        var fileText = GetConfigText(classPath.ClassNamespace);
        _utilities.CreateFile(classPath, fileText);
    }

    private static string GetConfigText(string classNamespace)
    {
        return @$"namespace {classNamespace};

public class {FileNames.AuthOptions()}
{{
    public const string SectionName = ""Auth"";

    public string Audience {{ get; set; }} = String.Empty;
    public string Authority {{ get; set; }} = String.Empty;
    public string AuthorizationUrl {{ get; set; }} = String.Empty;
    public string TokenUrl {{ get; set; }} = String.Empty;
    public string ClientId {{ get; set; }} = String.Empty;
    public string ClientSecret {{ get; set; }} = String.Empty;
}}

public static class AuthOptionsExtensions
{{
    public static AuthOptions GetAuthOptions(this IConfiguration configuration)
        => configuration.GetSection(AuthOptions.SectionName).Get<AuthOptions>();
}}";
    }
}
