﻿namespace Kuzaine.Builders.Tests.IntegrationTests;

using Kuzaine.Services;
using Domain;
using Domain.Enums;
using Helpers;
using Services;

public class GetRecordQueryTestBuilder
{
    private readonly IKuzaineUtilities _utilities;

    public GetRecordQueryTestBuilder(IKuzaineUtilities utilities)
    {
        _utilities = utilities;
    }

    public void CreateTests(string solutionDirectory, string testDirectory, string srcDirectory, Entity entity, string projectBaseName, string permission, bool featureIsProtected)
    {
        var classPath = ClassPathHelper.FeatureTestClassPath(testDirectory, $"{entity.Name}QueryTests.cs", entity.Plural, projectBaseName);
        var fileText = WriteTestFileText(solutionDirectory, testDirectory, srcDirectory, classPath, entity, projectBaseName, permission, featureIsProtected);
        _utilities.CreateFile(classPath, fileText);
    }

    private static string WriteTestFileText(string solutionDirectory, string testDirectory, string srcDirectory,
        ClassPath classPath, Entity entity, string projectBaseName, string permission, bool featureIsProtected)
    {
        var featureName = FileNames.GetEntityFeatureClassName(entity.Name);
        var testFixtureName = FileNames.GetIntegrationTestFixtureName();
        var queryName = FileNames.QueryRecordName();

        var fakerClassPath = ClassPathHelper.TestFakesClassPath(testDirectory, "", entity.Name, projectBaseName);
        var featuresClassPath = ClassPathHelper.FeaturesClassPath(srcDirectory, featureName, entity.Plural, projectBaseName);
        var exceptionsClassPath = ClassPathHelper.ExceptionsClassPath(solutionDirectory, "");
        var foreignEntityUsings = KuzaineUtilities.GetForeignEntityUsings(testDirectory, entity, projectBaseName);
        var permissionTest = !featureIsProtected ? null : GetPermissionTest(featureName, permission);

        return @$"namespace {classPath.ClassNamespace};

using {fakerClassPath.ClassNamespace};
using {featuresClassPath.ClassNamespace};
using {exceptionsClassPath.ClassNamespace};
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;{foreignEntityUsings}

public class {classPath.ClassNameWithoutExt} : TestBase
{{
    {GetTest(queryName, entity, featureName)}{GetWithoutKeyTest(queryName, entity, featureName)}{permissionTest}
}}";
    }

    private static string GetTest(string queryName, Entity entity, string featureName)
    {
        var fakeEntityVariableName = $"fake{entity.Name}One";
        var lowercaseEntityName = entity.Name.LowercaseFirstLetter();
        var pkName = Entity.PrimaryKeyProperty.Name;

        var fakeParent = IntegrationTestServices.FakeParentTestHelpersForBuilders(entity, out var fakeParentIdRuleFor);
        if (fakeParentIdRuleFor != "")
            fakeParentIdRuleFor += $"{Environment.NewLine}            ";

        return $@"[Fact]
    public async Task can_get_existing_{entity.Name.ToLower()}_with_accurate_props()
    {{
        // Arrange
        var testingServiceScope = new {FileNames.TestingServiceScope()}();
        {fakeParent}var {fakeEntityVariableName} = new {FileNames.FakeBuilderName(entity.Name)}(){fakeParentIdRuleFor}.Build();
        await testingServiceScope.InsertAsync({fakeEntityVariableName});

        // Act
        var query = new {featureName}.{queryName}({fakeEntityVariableName}.{pkName});
        var {lowercaseEntityName} = await testingServiceScope.SendAsync(query);

        // Assert{GetAssertions(entity.Properties, lowercaseEntityName, fakeEntityVariableName)}
    }}";
    }

    private static string GetWithoutKeyTest(string queryName, Entity entity, string featureName)
    {
        var badId = IntegrationTestServices.GetRandomId(Entity.PrimaryKeyProperty.Type);

        return badId == "" ? "" : $@"

    [Fact]
    public async Task get_{entity.Name.ToLower()}_throws_notfound_exception_when_record_does_not_exist()
    {{
        // Arrange
        var testingServiceScope = new {FileNames.TestingServiceScope()}();
        var badId = {badId};

        // Act
        var query = new {featureName}.{queryName}(badId);
        Func<Task> act = () => testingServiceScope.SendAsync(query);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }}";
    }

    private static string GetAssertions(List<EntityProperty> properties, string lowercaseEntityName, string fakeEntityVariableName)
    {
        var entityAssertions = "";
        foreach (var entityProperty in properties.Where(x => x.IsPrimitiveType))
        {
            entityAssertions += entityProperty.Type switch
            {
                "DateTime" or "DateTimeOffset" or "TimeOnly" =>
                    $@"{Environment.NewLine}        {lowercaseEntityName}.{entityProperty.Name}.Should().BeCloseTo({fakeEntityVariableName}.{entityProperty.Name}, 1.Seconds());",
                "DateTime?" =>
                    $@"{Environment.NewLine}        {lowercaseEntityName}.{entityProperty.Name}.Should().BeCloseTo((DateTime){fakeEntityVariableName}.{entityProperty.Name}, 1.Seconds());",
                "DateTimeOffset?" =>
                    $@"{Environment.NewLine}        {lowercaseEntityName}.{entityProperty.Name}.Should().BeCloseTo((DateTimeOffset){fakeEntityVariableName}.{entityProperty.Name}, 1.Seconds());",
                "TimeOnly?" =>
                    $@"{Environment.NewLine}        {lowercaseEntityName}.{entityProperty.Name}.Should().BeCloseTo((TimeOnly){fakeEntityVariableName}.{entityProperty.Name}, 1.Seconds());",
                _ =>
                    $@"{Environment.NewLine}        {lowercaseEntityName}.{entityProperty.Name}.Should().Be({fakeEntityVariableName}.{entityProperty.Name});"
            };
        }

        return entityAssertions;
    }
    
    private static string GetPermissionTest(string featureName, string permission)
    {
        var queryName = FileNames.QueryListName();
        
        return $@"

    [Fact]
    public async Task must_be_permitted()
    {{
        // Arrange
        var testingServiceScope = new {FileNames.TestingServiceScope()}();
        testingServiceScope.SetUserNotPermitted(Permissions.{permission});

        // Act
        var command = new {featureName}.{queryName}(Guid.NewGuid());
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }}";
    }
}
