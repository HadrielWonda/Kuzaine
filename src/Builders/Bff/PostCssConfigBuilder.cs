namespace Kuzaine.Builders.Bff;

using Helpers;
using Services;

public class PostCssBuilder
{
    private readonly IKuzaineUtilities _utilities;

    public PostCssBuilder(IKuzaineUtilities utilities)
    {
        _utilities = utilities;
    }

    public void CreatePostCss(string spaDirectory)
    {
        var classPath = ClassPathHelper.BffSpaRootClassPath(spaDirectory, "postcss.config.js");
        var fileText = GetPostCssText();
        _utilities.CreateFile(classPath, fileText);
    }

    public static string GetPostCssText()
    {
        return @$"module.exports = {{
	plugins: {{
		tailwindcss: {{}},
		autoprefixer: {{}},
	}},
}};";
    }
}
