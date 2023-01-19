using Kuzaine.Domain.Enums;
using Kuzaine.Helpers;
using Kuzaine.Services;



namespace Kuzaine.Builders.NextJs.Domain;

public class DomainIndexBuilder
{
    private readonly IKuzaineUtilities _utilities;

    public DomainIndexBuilder(IKuzaineUtilities utilities)
    {
        _utilities = utilities;
    }

    public void CreateDynamicFeatureIndex(string spaDirectory, string entityPlural)
    {
        var classPath = ClassPathHelper.NextJsSpaFeatureClassPath(spaDirectory, entityPlural, NextJsDomainCategory.Index, "index.ts");
        var fileText = GetDynamicFeatureIndexText();
        _utilities.CreateFile(classPath, fileText);
    }

    public static string GetDynamicFeatureIndexText()
    {
        return @$"export * from ""./api"";
export * from ""./features"";
export * from ""./types"";
export * from ""./validation"";";
    }
}
