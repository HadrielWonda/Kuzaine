﻿namespace Kuzaine.Builders.Projects;

using Helpers;
using Services;

public class AuthServerProjBuilder
{
    private readonly IKuzaineUtilities _utilities;

    public AuthServerProjBuilder(IKuzaineUtilities utilities)
    {
        _utilities = utilities;
    }

    public void CreateProject(string solutionDirectory, string projectBaseName)
    {
        var classPath = ClassPathHelper.WebApiProjectClassPath(solutionDirectory, projectBaseName);
        var fileText = ProjectFileText();
        _utilities.CreateFile(classPath, fileText);
    }

    public static string ProjectFileText()
    {
        return @$"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net7.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include=""Pulumi"" Version=""3.*"" />
    <PackageReference Include=""Pulumi.Keycloak"" Version=""4.11.0"" />
  </ItemGroup>

</Project>";
    }
}
