using Helpers;
using Services;


namespace Kuzaine.Builders.Bff.Src;

public class CustomCssBuilder
{
    private readonly IKuzaineUtilities _utilities;

    public CustomCssBuilder(IKuzaineUtilities utilities)
    {
        _utilities = utilities;
    }

    public void CreateCustomCss(string spaDirectory)
    {
        var classPath = ClassPathHelper.BffSpaSrcClassPath(spaDirectory, "custom.css");
        var fileText = GetCustomCssText();
        _utilities.CreateFile(classPath, fileText);
    }

    public static string GetCustomCssText()
    {
        return @$"@tailwind base;
@tailwind components;
@tailwind utilities;";
    }
}
