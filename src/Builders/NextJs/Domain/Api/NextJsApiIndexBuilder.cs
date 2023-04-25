namespace Kuzaine.Builders.NextJs.Domain.Api;

using Kuzaine.Domain.Enums;
using Kuzaine.Helpers;
using Kuzaine.Services;

public class NextJsApiIndexBuilder
{
    private readonly IKuzaineUtilities _utilities;

    public NextJsApiIndexBuilder(IKuzaineUtilities utilities)
    {
        _utilities = utilities;
    }

    public void CreateDynamicFeatureApiIndex(string spaDirectory, string entityName, string entityPlural)
    {
        var routesIndexClassPath = ClassPathHelper.NextJsSpaFeatureClassPath(spaDirectory, entityPlural, NextJsDomainCategory.Api, "index.ts");
        var routesIndexFileText = GetDynamicFeatureApisIndexText(entityName);
        _utilities.CreateFile(routesIndexClassPath, routesIndexFileText);
    }

    public static string GetDynamicFeatureApisIndexText(string entityName)
    {
        var keysImport = FileNames.NextJsApiKeysFilename(entityName);
        return @$"export * from './{keysImport}';
export * from ""./{FeatureType.AddRecord.NextJsApiName(entityName)}"";
export * from ""./{FeatureType.GetList.NextJsApiName(entityName)}"";
export * from ""./{FeatureType.GetRecord.NextJsApiName(entityName)}"";
export * from ""./{FeatureType.DeleteRecord.NextJsApiName(entityName)}"";
export * from ""./{FeatureType.UpdateRecord.NextJsApiName(entityName)}"";";
    }
}
