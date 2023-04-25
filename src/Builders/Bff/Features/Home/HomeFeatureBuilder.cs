namespace Kuzaine.Builders.Bff.Features.Home;

using Domain.Enums;
using Helpers;
using Services;

public class HomeFeatureBuilder
{
    private readonly IKuzaineUtilities _utilities;

    public HomeFeatureBuilder(IKuzaineUtilities utilities)
    {
        _utilities = utilities;
    }

    public void CreateHomeFeatureIndex(string spaDirectory)
    {
        var classPath = ClassPathHelper.BffSpaFeatureClassPath(spaDirectory, "Home", BffFeatureCategory.Index, "index.ts");
        var fileText = GetHomeFeatureIndexText();
        _utilities.CreateFile(classPath, fileText);
    }

    public static string GetHomeFeatureIndexText()
    {
        return @$"export * from './routes';";
    }
}
