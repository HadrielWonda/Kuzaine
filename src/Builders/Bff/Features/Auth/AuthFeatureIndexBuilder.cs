using Domain.Enums;
using Helpers;
using Services;


namespace Kuzaine.Builders.Bff.Features.Auth;

public class AuthFeatureBuilder
{
    private readonly IKuzaineUtilities _utilities;

    public AuthFeatureBuilder(IKuzaineUtilities utilities)
    {
        _utilities = utilities;
    }

    public void CreateAuthFeatureIndex(string spaDirectory)
    {
        var classPath = ClassPathHelper.BffSpaFeatureClassPath(spaDirectory, "Auth", BffFeatureCategory.Index, "index.ts");
        var fileText = GetAuthFeatureIndexText();
        _utilities.CreateFile(classPath, fileText);
    }

    public static string GetAuthFeatureIndexText()
    {
        return @$"export * from './api/useAuthUser';
export * from './routes';";
    }
}
