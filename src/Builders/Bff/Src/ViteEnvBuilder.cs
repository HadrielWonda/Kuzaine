namespace Kuzaine.Builders.Bff.Src;

using Helpers;
using Services;

public class ViteEnvBuilder
{
    private readonly IKuzaineUtilities _utilities;

    public ViteEnvBuilder(IKuzaineUtilities utilities)
    {
        _utilities = utilities;
    }

    public void CreateViteEnv(string spaDirectory)
    {
        var classPath = ClassPathHelper.BffSpaSrcClassPath(spaDirectory, "vite-env.d.ts");
        var fileText = GetViteEnvText();
        _utilities.CreateFile(classPath, fileText);
    }

    public static string GetViteEnvText()
    {
        return @$"/// <reference types=""vite/client"" />";
    }
}
