using Kuzaine.Domain;
using Kuzaine.Domain.Enums;
using Kuzaine.Helpers;
using Kuzaine.Services;



namespace Kuzaine.Builders.NextJs.Domain;

public class NextJsEntityIndexBuilder
{
    private readonly IKuzaineUtilities _utilities;

    public NextJsEntityIndexBuilder(IKuzaineUtilities utilities)
    {
        _utilities = utilities;
    }

    public void CreateFile(string nextSrc, string entityName,
     string entityPlural, List<NextJsEntityProperty> properties
     )
    {
        var routesIndexClassPath = ClassPathHelper.NextJsSpaFeatureClassPath(nextSrc,
            entityPlural,
            NextJsDomainCategory.Index,
            $"index.ts");
        var routesIndexFileText = GetFileText(entityName, properties);
        _utilities.CreateFile(routesIndexClassPath, routesIndexFileText);
    }

    public static string GetFileText(string entityName, List<NextJsEntityProperty> properties)
    {
        var validationSchema = FileNames.NextJsEntityValidationName(entityName);

        return @$"export * from ""./api"";
export * from ""./features"";
export * from ""./types"";
export * from ""./validation"";
";
    }
}
