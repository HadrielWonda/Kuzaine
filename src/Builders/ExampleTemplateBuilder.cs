namespace Kuzaine.Builders;

using System.Text.Json;
using Domain;
using Helpers;
using Services;

public class ExampleTemplateBuilder
{
    private readonly IKuzaineUtilities _utilities;

    public ExampleTemplateBuilder(IKuzaineUtilities utilities)
    {
        _utilities = utilities;
    }

    public void CreateFile(string solutionDirectory, DomainProject domainProject)
    {
        var classPath = ClassPathHelper.ExampleYamlRootClassPath(solutionDirectory, "exampleTemplate.json");
        var fileText = JsonSerializer.Serialize(domainProject);
        _utilities.CreateFile(classPath, fileText);
    }

    public void CreateYamlFile(string solutionDirectory, string domainProject)
    {
        var classPath = ClassPathHelper.ExampleYamlRootClassPath(solutionDirectory, "exampleTemplate.yaml");
        _utilities.CreateFile(classPath, domainProject);
    }
}
