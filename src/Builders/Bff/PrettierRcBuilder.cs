namespace Kuzaine.Builders.Bff;

using Helpers;
using Services;

public class PrettierRcBuilder
{
    private readonly IKuzaineUtilities _utilities;

    public PrettierRcBuilder(IKuzaineUtilities utilities)
    {
        _utilities = utilities;
    }

    public void CreatePrettierRc(string spaDirectory)
    {
        var classPath = ClassPathHelper.BffSpaRootClassPath(spaDirectory, ".prettierrc");
        var fileText = GetPrettierRcText();
        _utilities.CreateFile(classPath, fileText);
    }

    public static string GetPrettierRcText()
    {
        return @$"{{
  ""singleQuote"": true,
  ""trailingComma"": ""es5"",
  ""printWidth"": 100,
  ""tabWidth"": 2,
  ""useTabs"": true
}}";
    }
}
