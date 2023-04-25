namespace Kuzaine.Builders.Configurations;

using Kuzaine.Helpers;
using Kuzaine.Services;

public class RootConfigurationsExtensionBuilder
{
    private readonly IKuzaineUtilities _utilities;

    public RootConfigurationsExtensionBuilder(IKuzaineUtilities utilities)
    {
        _utilities = utilities;
    }

    public void CreateConfig(string srcDirectory, string projectBaseName)
    {
        var classPath = ClassPathHelper.WebApiConfigurationsClassPath(srcDirectory, $"{FileNames.RootConfigurationExtensions()}.cs", projectBaseName);
        var fileText = GetConfigText(classPath.ClassNamespace);
        _utilities.CreateFile(classPath, fileText);
    }

    private static string GetConfigText(string classNamespace)
    {
        return @$"namespace {classNamespace};

public static class RootConfigurationExtensions
{{
    public static string GetJaegerHostValue(this IConfiguration configuration)
        => configuration.GetSection(""JaegerHost"").Value;
}}";
    }
}
