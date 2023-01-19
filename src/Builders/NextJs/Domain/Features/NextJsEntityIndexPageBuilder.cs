using Kuzaine.Domain.Enums;
using Kuzaine.Helpers;
using Kuzaine.Services;



namespace Kuzaine.Builders.NextJs.Domain.Features;

public class NextJsEntityFeatureIndexPageBuilder
{
    private readonly IKuzaineUtilities _utilities;

    public NextJsEntityFeatureIndexPageBuilder(IKuzaineUtilities utilities)
    {
        _utilities = utilities;
    }

    public void CreateFile(string nextSrc, string entityName, string entityPlural)
    {
        var routesIndexClassPath = ClassPathHelper.NextJsSpaFeatureClassPath(nextSrc,
            entityPlural,
            NextJsDomainCategory.Features,
            $"index.ts");
        var routesIndexFileText = GetFileText(entityName);
        _utilities.CreateFile(routesIndexClassPath, routesIndexFileText);
    }

    public static string GetFileText(string entityName)
    {
        return @$"export * from ""./{FileNames.NextJsEntityFeatureFormName(entityName)}"";
export * from ""./{FileNames.NextJsEntityFeatureListTableName(entityName)}""; ";
    }
}
