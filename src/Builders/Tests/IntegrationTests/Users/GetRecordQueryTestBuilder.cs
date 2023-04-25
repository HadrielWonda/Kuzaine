﻿namespace Kuzaine.Builders.Tests.IntegrationTests.Users;

using Kuzaine.Builders.Tests.IntegrationTests.Services;
using Kuzaine.Domain;
using Kuzaine.Domain.Enums;
using Kuzaine.Helpers;
using Kuzaine.Services;

public class GetRecordQueryTestBuilder
{
    private readonly IKuzaineUtilities _utilities;

    public GetRecordQueryTestBuilder(IKuzaineUtilities utilities)
    {
        _utilities = utilities;
    }

    public void CreateTests(string solutionDirectory, string testDirectory, string srcDirectory, Entity entity, string projectBaseName)
    {
        var classPath = ClassPathHelper.FeatureTestClassPath(testDirectory, $"{entity.Name}QueryTests.cs", entity.Plural, projectBaseName);
        var fileText = WriteTestFileText(solutionDirectory, testDirectory, srcDirectory, classPath, entity, projectBaseName);
        _utilities.CreateFile(classPath, fileText);
    }

    private static string WriteTestFileText(string solutionDirectory, string testDirectory, string srcDirectory, ClassPath classPath, Entity entity, string projectBaseName)
    {
        var featureName = FileNames.GetEntityFeatureClassName(entity.Name);
        var testFixtureName = FileNames.GetIntegrationTestFixtureName();
        var queryName = FileNames.QueryRecordName();

        var fakerClassPath = ClassPathHelper.TestFakesClassPath(testDirectory, "", entity.Name, projectBaseName);
        var featuresClassPath = ClassPathHelper.FeaturesClassPath(srcDirectory, featureName, entity.Plural, projectBaseName);
        var exceptionsClassPath = ClassPathHelper.ExceptionsClassPath(solutionDirectory, "");
        var foreignEntityUsings = KuzaineUtilities.GetForeignEntityUsings(testDirectory, entity, projectBaseName);

        return @$"namespace {classPath.ClassNamespace};

using {fakerClassPath.ClassNamespace};
using {featuresClassPath.ClassNamespace};
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using {exceptionsClassPath.ClassNamespace};
using System.Threading.Tasks;{foreignEntityUsings}

public class {classPath.ClassNameWithoutExt} : TestBase
{{
    {GetTest(queryName, entity, featureName)}{GetWithoutKeyTest(queryName, entity, featureName)}
}}";
    }

    private static string GetTest(string queryName, Entity entity, string featureName)
    {
        var fakeEntityVariableName = $"fake{entity.Name}One";
        var lowercaseEntityName = entity.Name.LowercaseFirstLetter();
        var pkName = Entity.PrimaryKeyProperty.Name;

        var fakeParent = IntegrationTestServices.FakeParentTestHelpersForBuilders(entity, out var fakeParentIdRuleFor);

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

        // Assert
        user.FirstName.Should().Be(fakeUserOne.FirstName);
        user.LastName.Should().Be(fakeUserOne.LastName);
        user.Username.Should().Be(fakeUserOne.Username);
        user.Identifier.Should().Be(fakeUserOne.Identifier);
        user.Email.Should().Be(fakeUserOne.Email.Value);
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
}
