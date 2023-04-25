﻿namespace Kuzaine.Builders.Projects;

using Helpers;
using Services;

public class FunctionalTestsCsProjBuilder
{
    private readonly IKuzaineUtilities _utilities;

    public FunctionalTestsCsProjBuilder(IKuzaineUtilities utilities)
    {
        _utilities = utilities;
    }

    public void CreateTestsCsProj(string solutionDirectory, string projectBaseName)
    {
        var classPath = ClassPathHelper.FunctionalTestProjectClassPath(solutionDirectory, projectBaseName);
        _utilities.CreateFile(classPath, GetTestsCsProjFileText(solutionDirectory, projectBaseName));
    }

    public static string GetTestsCsProjFileText(string solutionDirectory, string projectBaseName)
    {
        var webApiClassPath = ClassPathHelper.WebApiProjectClassPath(solutionDirectory, projectBaseName);
        var sharedTestClassPath = ClassPathHelper.SharedTestProjectClassPath(solutionDirectory, projectBaseName);

        return @$"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <TargetFramework>net7.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>

    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include=""AutoBogusLifesupport"" Version=""2.14.0"" />
    <PackageReference Include=""Bogus"" Version=""34.0.2"" />
    <PackageReference Include=""FluentAssertions"" Version=""6.9.0"" />
    <PackageReference Include=""MediatR"" Version=""11.1.0"" />
    <PackageReference Include=""Microsoft.EntityFrameworkCore.Relational"" Version=""7.0.2"" />
    <PackageReference Include=""Moq"" Version=""4.18.4"" />
    <PackageReference Include=""Microsoft.AspNetCore.Mvc.Testing"" Version=""7.0.2"" />
    <PackageReference Include=""Microsoft.AspNetCore.Mvc.NewtonsoftJson"" Version=""7.0.2"" />
    <PackageReference Include=""Microsoft.NET.Test.Sdk"" Version=""17.4.1"" />
    <PackageReference Include=""WebMotions.Fake.Authentication.JwtBearer"" Version=""7.0.0"" />
    <PackageReference Include=""Testcontainers"" Version=""2.4.0"" />
    <PackageReference Include=""xunit"" Version=""2.4.2"" />
    <PackageReference Include=""xunit.runner.visualstudio"" Version=""2.4.5"" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include=""..\..\src\{webApiClassPath.ClassNamespace}\{webApiClassPath.ClassName}"" />
    <ProjectReference Include=""..\{sharedTestClassPath.ClassNamespace}\{sharedTestClassPath.ClassName}"" />
  </ItemGroup>

</Project>";
    }
}
