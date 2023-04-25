namespace Kuzaine.Builders.Tests.Utilities;

using Helpers;
using Services;

public class IntegrationTestBaseBuilder
{
    private readonly IKuzaineUtilities _utilities;

    public IntegrationTestBaseBuilder(IKuzaineUtilities utilities)
    {
        _utilities = utilities;
    }

    public void CreateBase(string solutionDirectory, string projectBaseName)
    {
        var classPath = ClassPathHelper.IntegrationTestProjectRootClassPath(solutionDirectory, "TestBase.cs", projectBaseName);
        var fileText = GetBaseText(classPath.ClassNamespace);
        _utilities.CreateFile(classPath, fileText);
    }

    public static string GetBaseText(string classNamespace)
    {
        return @$"namespace {classNamespace};

using AutoBogus;
using Xunit;

[Collection(nameof(TestFixture))]
public class TestBase : IDisposable
{{
    public TestBase()
    {{
        AutoFaker.Configure(builder =>
        {{
            // configure global autobogus settings here
            builder.WithDateTimeKind(DateTimeKind.Utc)
                .WithRecursiveDepth(3)
                .WithTreeDepth(1)
                .WithRepeatCount(1);
        }});
    }}
    
    public void Dispose()
    {{
    }}
}}";
    }
}
