﻿namespace Kuzaine.Builders.Projects;

using Helpers;
using Services;

public class SharedKernelCsProjBuilder
{
    private readonly IKuzaineUtilities _utilities;

    public SharedKernelCsProjBuilder(IKuzaineUtilities utilities)
    {
        _utilities = utilities;
    }

    public void CreateSharedKernelCsProj(string solutionDirectory)
    {
        var classPath = ClassPathHelper.SharedKernelProjectClassPath(solutionDirectory);
        var fileText = GetMessagesCsProjFileText();
        _utilities.CreateFile(classPath, fileText);
    }

    public static string GetMessagesCsProjFileText()
    {
        return @$"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <TargetFramework>netstandard2.1</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include=""FluentValidation"" Version=""11.2.2"" />
  </ItemGroup>

</Project>";
    }
}
