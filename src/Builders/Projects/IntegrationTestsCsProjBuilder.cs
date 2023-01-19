﻿using Helpers;
using Services;



namespace Kuzaine.Builders.Projects;

public class IntegrationTestsCsProjBuilder
{
    private readonly IKuzaineUtilities _utilities;

    public IntegrationTestsCsProjBuilder(IKuzaineUtilities utilities)
    {
        _utilities = utilities;
    }

    public void CreateTestsCsProj(string solutionDirectory, string projectBaseName)
    {
        var classPath = ClassPathHelper.IntegrationTestProjectClassPath(solutionDirectory, projectBaseName);
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
    <PackageReference Include=""Docker.DotNet"" Version=""3.125.5"" />
    <PackageReference Include=""DotNet.Testcontainers"" Version=""1.5.0"" />
    <PackageReference Include=""Ductus.FluentDocker"" Version=""2.10.57"" />
    <PackageReference Include=""FluentAssertions"" Version=""6.7.0"" />
    <PackageReference Include=""MediatR"" Version=""11.0.0"" />
    <PackageReference Include=""Microsoft.AspNetCore.Mvc.Testing"" Version=""7.0.10"" />
    <PackageReference Include=""Microsoft.EntityFrameworkCore.Relational"" Version=""7.0.10"" />
    <PackageReference Include=""Moq"" Version=""4.18.2"" />
    <PackageReference Include=""Npgsql"" Version=""6.0.7"" />
    <PackageReference Include=""NUnit"" Version=""3.13.3"" />
    <PackageReference Include=""NUnit3TestAdapter"" Version=""4.2.1"" />
    <PackageReference Include=""Microsoft.NET.Test.Sdk"" Version=""17.3.2"" />
    <PackageReference Include=""Respawn"" Version=""6.0.0"" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include=""..\..\src\{webApiClassPath.ClassNamespace}\{webApiClassPath.ClassName}"" />
    <ProjectReference Include=""..\{sharedTestClassPath.ClassNamespace}\{sharedTestClassPath.ClassName}"" />
  </ItemGroup>

</Project>";
    }
}
