using Helpers;
using Services;



namespace Kuzaine.Builders.ExtensionBuilders;

public class ApiVersioningExtensionsBuilder
{
    private readonly IKuzaineUtilities _utilities;

    public ApiVersioningExtensionsBuilder(IKuzaineUtilities utilities)
    {
        _utilities = utilities;
    }

    public void CreateApiVersioningServiceExtension(string solutionDirectory, string projectBaseName)
    {
        var classPath = ClassPathHelper.WebApiServiceExtensionsClassPath(solutionDirectory, $"ApiVersioningServiceExtension.cs", projectBaseName);
        var fileText = GetApiVersioningServiceExtensionText(classPath.ClassNamespace);
        _utilities.CreateFile(classPath, fileText);
    }

    public static string GetApiVersioningServiceExtensionText(string classNamespace)
    {
        return @$"namespace {classNamespace};

using MapsterMapper;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Reflection;

public static class ApiVersioningServiceExtension
{{
    public static void AddApiVersioningExtension(this IServiceCollection services)
    {{
        services.AddApiVersioning(config =>
        {{
            // Default API Version
            config.DefaultApiVersion = new ApiVersion(1, 0);
            // use default version when version is not specified
            config.AssumeDefaultVersionWhenUnspecified = true;
            // Advertise the API versions supported for the particular endpoint
            config.ReportApiVersions = true;
        }});
    }}
}}";
    }
}