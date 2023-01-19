using Helpers;
using Services;



namespace Kuzaine.Builders;

public class WebApiLaunchSettingsBuilder
{
    private readonly IKuzaineUtilities _utilities;

    public WebApiLaunchSettingsBuilder(IKuzaineUtilities utilities)
    {
        _utilities = utilities;
    }

    public void CreateLaunchSettings(string srcDirectory, string projectBaseName)
    {
        var classPath = ClassPathHelper.WebApiLaunchSettingsClassPath(srcDirectory, $"launchSettings.json", projectBaseName);
        var fileText = GetLaunchSettingsText();
        _utilities.CreateFile(classPath, fileText);
    }

    public static string GetLaunchSettingsText()
    {
        return @$"{{
  ""profiles"": {{
  }}
}}";
    }
}
