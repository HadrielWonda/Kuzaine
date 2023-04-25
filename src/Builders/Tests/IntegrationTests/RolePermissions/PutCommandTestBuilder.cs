﻿namespace Kuzaine.Builders.Tests.IntegrationTests.RolePermissions;

using Kuzaine.Builders.Tests.IntegrationTests.Services;
using Kuzaine.Domain;
using Kuzaine.Domain.Enums;
using Kuzaine.Helpers;
using Kuzaine.Services;

public class PutCommandTestBuilder
{
    private readonly IKuzaineUtilities _utilities;

    public PutCommandTestBuilder(IKuzaineUtilities utilities)
    {
        _utilities = utilities;
    }

    public void CreateTests(string solutionDirectory, string testDirectory, string srcDirectory, Entity entity, string projectBaseName)
    {
        var classPath = ClassPathHelper.FeatureTestClassPath(testDirectory, $"Update{entity.Name}CommandTests.cs", entity.Plural, projectBaseName);
        var fileText = WriteTestFileText(solutionDirectory, testDirectory, srcDirectory, classPath, entity, projectBaseName);
        _utilities.CreateFile(classPath, fileText);
    }

    private static string WriteTestFileText(string solutionDirectory, string testDirectory, string srcDirectory, ClassPath classPath, Entity entity, string projectBaseName)
    {
        var featureName = FileNames.UpdateEntityFeatureClassName(entity.Name);
        var testFixtureName = FileNames.GetIntegrationTestFixtureName();
        var commandName = FileNames.CommandUpdateName();
        var fakeEntity = FileNames.FakerName(entity.Name);
        var fakeUpdateDto = FileNames.FakerName(FileNames.GetDtoName(entity.Name, Dto.Update));
        var fakeCreationDto = FileNames.FakerName(FileNames.GetDtoName(entity.Name, Dto.Creation));
        var fakeEntityVariableName = $"fake{entity.Name}One";
        var lowercaseEntityName = entity.Name.LowercaseFirstLetter();
        var pkName = Entity.PrimaryKeyProperty.Name;
        var lowercaseEntityPk = pkName.LowercaseFirstLetter();

        var fakerClassPath = ClassPathHelper.TestFakesClassPath(testDirectory, "", entity.Name, projectBaseName);
        var dtoClassPath = ClassPathHelper.DtoClassPath(srcDirectory, "", entity.Plural, projectBaseName);
        var featuresClassPath = ClassPathHelper.FeaturesClassPath(srcDirectory, featureName, entity.Plural, projectBaseName);
        var exceptionsClassPath = ClassPathHelper.ExceptionsClassPath(solutionDirectory, projectBaseName);

        var fakeParent = IntegrationTestServices.FakeParentTestHelpersForBuilders(entity, out var fakeParentIdRuleFor);
        var foreignEntityUsings = KuzaineUtilities.GetForeignEntityUsings(testDirectory, entity, projectBaseName);

        return @$"namespace {classPath.ClassNamespace};

using {fakerClassPath.ClassNamespace};
using {dtoClassPath.ClassNamespace};
using {exceptionsClassPath.ClassNamespace};
using {featuresClassPath.ClassNamespace};
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;{foreignEntityUsings}

public class {classPath.ClassNameWithoutExt} : TestBase
{{
    [Fact]
    public async Task can_update_existing_{entity.Name.ToLower()}_in_db()
    {{
        // Arrange
        var testingServiceScope = new {FileNames.TestingServiceScope()}();
        {fakeParent}var {fakeEntityVariableName} = new {FileNames.FakeBuilderName(entity.Name)}(){fakeParentIdRuleFor}.Build();
        var updated{entity.Name}Dto = new {fakeUpdateDto}(){fakeParentIdRuleFor}.Generate();
        await testingServiceScope.InsertAsync({fakeEntityVariableName});

        var {lowercaseEntityName} = await testingServiceScope.ExecuteDbContextAsync(db => db.{entity.Plural}
            .FirstOrDefaultAsync({entity.Lambda} => {entity.Lambda}.Id == {fakeEntityVariableName}.Id));
        var {lowercaseEntityPk} = {lowercaseEntityName}.{pkName};

        // Act
        var command = new {featureName}.{commandName}({lowercaseEntityPk}, updated{entity.Name}Dto);
        await testingServiceScope.SendAsync(command);
        var updated{entity.Name} = await testingServiceScope.ExecuteDbContextAsync(db => db.{entity.Plural}.FirstOrDefaultAsync({entity.Lambda} => {entity.Lambda}.{pkName} == {lowercaseEntityPk}));

        // Assert
        updated{entity.Name}?.Permission.Should().Be(updated{entity.Name}Dto.Permission);
        updated{entity.Name}?.Role.Value.Should().Be(updated{entity.Name}Dto.Role);
    }}
}}";
    }
}
