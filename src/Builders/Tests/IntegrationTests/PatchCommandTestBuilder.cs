﻿using Kuzaine.Services;
using Domain;
using Domain.Enums;
using Helpers;
using Services;



namespace Kuzaine.Builders.Tests.IntegrationTests;

public class PatchCommandTestBuilder
{
    private readonly IKuzaineUtilities _utilities;

    public PatchCommandTestBuilder(IKuzaineUtilities utilities)
    {
        _utilities = utilities;
    }

    public void CreateTests(string solutionDirectory, string testDirectory, string srcDirectory, Entity entity, string projectBaseName)
    {
        var classPath = ClassPathHelper.FeatureTestClassPath(testDirectory, $"Patch{entity.Name}CommandTests.cs", entity.Plural, projectBaseName);
        var fileText = WriteTestFileText(solutionDirectory, testDirectory, srcDirectory, classPath, entity, projectBaseName);
        _utilities.CreateFile(classPath, fileText);
    }

    private static string WriteTestFileText(string solutionDirectory, string testDirectory, string srcDirectory, ClassPath classPath, Entity entity, string projectBaseName)
    {
        var featureName = FileNames.PatchEntityFeatureClassName(entity.Name);
        var testFixtureName = FileNames.GetIntegrationTestFixtureName();
        var commandName = FileNames.CommandPatchName();

        var fakerClassPath = ClassPathHelper.TestFakesClassPath(testDirectory, "", entity.Name, projectBaseName);
        var exceptionClassPath = ClassPathHelper.ExceptionsClassPath(testDirectory, "");
        var dtoClassPath = ClassPathHelper.DtoClassPath(srcDirectory, "", entity.Plural, projectBaseName);
        var featuresClassPath = ClassPathHelper.FeaturesClassPath(srcDirectory, featureName, entity.Plural, projectBaseName);

        var myProp = entity.Properties.FirstOrDefault(e => e.Type == "string" && e.CanManipulate);
        var lookupVal = $@"""Easily Identified Value For Test""";

        // if no string properties, do one with an int
        if (myProp == null)
        {
            myProp = entity.Properties.FirstOrDefault(e => e.Type.Contains("int") && e.CanManipulate);
            lookupVal = "999999";
        }

        if (myProp == null)
            return "// no patch tests were created";

        var foreignEntityUsings = KuzaineUtilities.GetForeignEntityUsings(testDirectory, entity, projectBaseName);

        return @$"namespace {classPath.ClassNamespace};

using {fakerClassPath.ClassNamespace};
using {dtoClassPath.ClassNamespace};
using {exceptionClassPath.ClassNamespace};
using {featuresClassPath.ClassNamespace};
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using static {testFixtureName};{foreignEntityUsings}

public class {classPath.ClassNameWithoutExt} : TestBase
{{
    {GetAddCommandTest(commandName, entity, featureName, lookupVal, myProp)}
    {BadKey(commandName, entity, featureName)}{NullPatchDoc(commandName, entity, featureName)}
}}";
    }

    private static string GetAddCommandTest(string commandName, Entity entity, string featureName, string lookupVal, EntityProperty prop)
    {
        var fakeEntity = FileNames.FakerName(entity.Name);
        var updateDto = FileNames.GetDtoName(entity.Name, Dto.Update);
        var fakeEntityVariableName = $"fake{entity.Name}One";
        var lowercaseEntityName = entity.Name.LowercaseFirstLetter();
        var pkName = Entity.PrimaryKeyProperty.Name;
        var lowercaseEntityPk = pkName.LowercaseFirstLetter();
        var fakeCreationDto = FileNames.FakerName(FileNames.GetDtoName(entity.Name, Dto.Creation));

        var fakeParent = IntegrationTestServices.FakeParentTestHelpers(entity, out var fakeParentIdRuleFor);

        return $@"[Test]
    public async Task can_patch_existing_{entity.Name.ToLower()}_in_db()
    {{
        // Arrange
        {fakeParent}var {fakeEntityVariableName} = {fakeEntity}.Generate(new {fakeCreationDto}(){fakeParentIdRuleFor}.Generate());
        await InsertAsync({fakeEntityVariableName});
        var {lowercaseEntityName} = await ExecuteDbContextAsync(db => db.{entity.Plural}
            .FirstOrDefaultAsync({entity.Lambda} => {entity.Lambda}.Id == {fakeEntityVariableName}.Id));
        var {lowercaseEntityPk} = {lowercaseEntityName}.{pkName};

        var patchDoc = new JsonPatchDocument<{updateDto}>();
        var newValue = {lookupVal};
        patchDoc.Replace({entity.Lambda} => {entity.Lambda}.{prop.Name}, newValue);

        // Act
        var command = new {featureName}.{commandName}({lowercaseEntityPk}, patchDoc);
        await SendAsync(command);
        var updated{entity.Name} = await ExecuteDbContextAsync(db => db.{entity.Plural}.FirstOrDefaultAsync({entity.Lambda} => {entity.Lambda}.{pkName} == {lowercaseEntityPk}));

        // Assert
        updated{entity.Name}?.{prop.Name}.Should().Be(newValue);
    }}";
    }

    private static string NullPatchDoc(string commandName, Entity entity, string featureName)
    {
        var randomId = IntegrationTestServices.GetRandomId(Entity.PrimaryKeyProperty.Type);

        return randomId == "" ? "" : $@"

    [Test]
    public async Task passing_null_patchdoc_throws_validationexception()
    {{
        // Arrange
        var randomId = {randomId};

        // Act
        var command = new {featureName}.{commandName}(randomId, null);
        Func<Task> act = () => SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }}";
    }

    private static string BadKey(string commandName, Entity entity, string featureName)
    {
        var badId = IntegrationTestServices.GetRandomId(Entity.PrimaryKeyProperty.Type);
        var updateDto = FileNames.GetDtoName(entity.Name, Dto.Update);

        return badId == "" ? "" : $@"
    [Test]
    public async Task patch_{entity.Name.ToLower()}_throws_notfound_exception_when_record_does_not_exist()
    {{
        // Arrange
        var badId = {badId};
        var patchDoc = new JsonPatchDocument<{updateDto}>();

        // Act
        var command = new {featureName}.{commandName}(badId, patchDoc);
        Func<Task> act = () => SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }}";
    }
}
