﻿namespace Kuzaine.Builders.Bff;

using Helpers;
using Services;

public class BffAppSettingsBuilder
{
    private readonly IKuzaineUtilities _utilities;

    public BffAppSettingsBuilder(IKuzaineUtilities utilities)
    {
        _utilities = utilities;
    }

    public void CreateBffAppSettings(string projectDirectory)
    {
        var classPath = ClassPathHelper.BffProjectRootClassPath(projectDirectory, $"appsettings.json");
        var fileText = @$"{{
  ""AllowedHosts"": ""*""
}}
";
        _utilities.CreateFile(classPath, fileText);
    }
}
