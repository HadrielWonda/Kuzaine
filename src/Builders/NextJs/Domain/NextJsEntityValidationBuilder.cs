namespace Kuzaine.Builders.NextJs.Domain;

using Kuzaine.Domain;
using Kuzaine.Domain.Enums;
using Kuzaine.Helpers;
using Kuzaine.Services;

public class NextJsEntityValidationBuilder
{
    private readonly IKuzaineUtilities _utilities;

    public NextJsEntityValidationBuilder(IKuzaineUtilities utilities)
    {
        _utilities = utilities;
    }

    public void CreateFile(string nextSrc, string entityName, string entityPlural, List<NextJsEntityProperty> properties)
    {
        var routesIndexClassPath = ClassPathHelper.NextJsSpaFeatureClassPath(nextSrc,
            entityPlural,
            NextJsDomainCategory.Index,
            $"validation.tsx");
        var routesIndexFileText = GetFileText(entityName, properties);
        _utilities.CreateFile(routesIndexClassPath, routesIndexFileText);
    }

    public static string GetFileText(string entityName, List<NextJsEntityProperty> properties)
    {
        var validationSchema = FileNames.NextJsEntityValidationName(entityName);

        return @$"import * as yup from ""yup"";

export const {validationSchema} = yup.object({{{GetValidations(properties)}
}});";
    }

    private static string GetValidations(List<NextJsEntityProperty> properties)
    {
        var validations = "";
        foreach (var property in properties)
        {
            validations += property.TypeEnum.YupValidation(property.Name);
        }

        return validations;
    }
}
