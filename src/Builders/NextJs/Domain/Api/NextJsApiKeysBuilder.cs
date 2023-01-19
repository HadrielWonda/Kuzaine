using Kuzaine.Domain.Enums;
using Kuzaine.Helpers;
using Kuzaine.Services;



namespace Kuzaine.Builders.NextJs.Domain.Api;

public class NextJsApiKeysBuilder
{
    private readonly IKuzaineUtilities _utilities;

    public NextJsApiKeysBuilder(IKuzaineUtilities utilities)
    {
        _utilities = utilities;
    }

    public void CreateDynamicFeatureKeys(string spaDirectory, string entityName, string entityPlural)
    {
        var routesIndexClassPath = ClassPathHelper.NextJsSpaFeatureClassPath(spaDirectory,
            entityPlural,
            NextJsDomainCategory.Api,
            $"{FileNames.NextJsApiKeysFilename(entityName)}.ts");
        var routesIndexFileText = GetDynamicFeatureKeysText(entityName);
        _utilities.CreateFile(routesIndexClassPath, routesIndexFileText);
    }

    public static string GetDynamicFeatureKeysText(string entityName)
    {
        var keyExportName = FileNames.NextJsApiKeysExport(entityName);
        return @$"const {keyExportName} = {{
  all: [""{entityName.UppercaseFirstLetter()}s""] as const,
  lists: () => [...{keyExportName}.all, ""list""] as const,
  list: (queryParams: string) => 
    [...{keyExportName}.lists(), {{ queryParams }}] as const,
  details: () => [...{keyExportName}.all, ""detail""] as const,
  detail: (id: string) => [...{keyExportName}.details(), id] as const,
}}

export {{ {keyExportName} }};
";
    }
}
