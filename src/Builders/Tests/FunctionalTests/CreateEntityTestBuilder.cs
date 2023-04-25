﻿namespace Kuzaine.Builders.Tests.FunctionalTests;

using System;
using System.IO;
using Kuzaine.Services;
using Domain;
using Domain.Enums;
using Helpers;

public class CreateEntityTestBuilder
{
    private readonly IKuzaineUtilities _utilities;

    public CreateEntityTestBuilder(IKuzaineUtilities utilities)
    {
        _utilities = utilities;
    }

    public void CreateTests(string solutionDirectory, string testDirectory, Entity entity, bool isProtected, string projectBaseName)
    {
        var classPath = ClassPathHelper.FunctionalTestClassPath(testDirectory, $"Create{entity.Name}Tests.cs", entity.Plural, projectBaseName);
        var fileText = WriteTestFileText(solutionDirectory, testDirectory, classPath, entity, isProtected, projectBaseName);
        _utilities.CreateFile(classPath, fileText);
    }

    private static string WriteTestFileText(string solutionDirectory, string testDirectory, ClassPath classPath, Entity entity, bool isProtected, string projectBaseName)
    {
        var testUtilClassPath = ClassPathHelper.FunctionalTestUtilitiesClassPath(testDirectory, projectBaseName, "");
        var fakerClassPath = ClassPathHelper.TestFakesClassPath(testDirectory, "", entity.Name, projectBaseName);
        var permissionsClassPath = ClassPathHelper.PolicyDomainClassPath(testDirectory, "", projectBaseName);
        var rolesClassPath = ClassPathHelper.SharedKernelDomainClassPath(solutionDirectory, "");
        var foreignEntityUsings = KuzaineUtilities.GetForeignEntityUsings(testDirectory, entity, projectBaseName);

        var permissionsUsing = isProtected
            ? $"{Environment.NewLine}using {permissionsClassPath.ClassNamespace};{Environment.NewLine}using {rolesClassPath.ClassNamespace};"
            : string.Empty;

        var authOnlyTests = isProtected ? $@"
            {CreateEntityTestUnauthorized(entity)}
            {CreateEntityTestForbidden(entity)}" : "";

        return @$"namespace {classPath.ClassNamespace};

using {fakerClassPath.ClassNamespace};
using {testUtilClassPath.ClassNamespace};{permissionsUsing}{foreignEntityUsings}
using FluentAssertions;
using Xunit;
using System.Net;
using System.Threading.Tasks;

public class {Path.GetFileNameWithoutExtension(classPath.FullClassPath)} : TestBase
{{
    {CreateEntityTest(entity, isProtected)}{authOnlyTests}
}}";
    }

    private static string CreateEntityTest(Entity entity, bool isProtected)
    {
        var fakeCreationDto = $"Fake{FileNames.GetDtoName(entity.Name, Dto.Creation)}";
        var fakeEntityVariableName = $"fake{entity.Name}";

        var fakeParent = "";
        var fakeParentIdRuleFor = "";
        foreach (var entityProperty in entity.Properties)
        {
            if (entityProperty.IsForeignKey && !entityProperty.IsMany && entityProperty.IsPrimitiveType)
            {
                var baseVarName = entityProperty.ForeignEntityName != entity.Name
                    ? $"{entityProperty.ForeignEntityName}"
                    : $"{entityProperty.ForeignEntityName}Parent";
                var fakeParentBuilder = FileNames.FakeBuilderName(entityProperty.ForeignEntityName);
                fakeParent += @$"var fake{baseVarName}One = new {fakeParentBuilder}().Build();
        await InsertAsync(fake{baseVarName}One);{Environment.NewLine}{Environment.NewLine}        ";
                fakeParentIdRuleFor +=
                    $"{Environment.NewLine}            .RuleFor({entity.Lambda} => {entity.Lambda}.{entityProperty.Name}, _ => fake{baseVarName}One.Id)";
            }
        }

        var testName = $"create_{entity.Name.ToLower()}_returns_created_using_valid_dto";
        testName += isProtected ? "_and_valid_auth_credentials" : "";
        var clientAuth = isProtected ? @$"

        var user = await AddNewSuperAdmin();
        FactoryClient.AddAuth(user.Identifier);" : "";

        return $@"[Fact]
    public async Task {testName}()
    {{
        // Arrange
        {fakeParent}var {fakeEntityVariableName} = new {fakeCreationDto}(){fakeParentIdRuleFor}.Generate();{clientAuth}

        // Act
        var route = ApiRoutes.{entity.Plural}.Create;
        var result = await FactoryClient.PostJsonRequestAsync(route, {fakeEntityVariableName});

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Created);
    }}";
    }

    private static string CreateEntityTestUnauthorized(Entity entity)
    {
        var fakeEntityVariableName = $"fake{entity.Name}";
        var fakeCreationDto = $"Fake{FileNames.GetDtoName(entity.Name, Dto.Creation)}";

        return $@"
    [Fact]
    public async Task create_{entity.Name.ToLower()}_returns_unauthorized_without_valid_token()
    {{
        // Arrange
        var {fakeEntityVariableName} = new {fakeCreationDto} {{ }}.Generate();

        // Act
        var route = ApiRoutes.{entity.Plural}.Create;
        var result = await FactoryClient.PostJsonRequestAsync(route, {fakeEntityVariableName});

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }}";
    }

    private static string CreateEntityTestForbidden(Entity entity)
    {
        var fakeEntityVariableName = $"fake{entity.Name}";
        var fakeCreationDto = $"Fake{FileNames.GetDtoName(entity.Name, Dto.Creation)}";

        return $@"
    [Fact]
    public async Task create_{entity.Name.ToLower()}_returns_forbidden_without_proper_scope()
    {{
        // Arrange
        var {fakeEntityVariableName} = new {fakeCreationDto} {{ }}.Generate();
        FactoryClient.AddAuth();

        // Act
        var route = ApiRoutes.{entity.Plural}.Create;
        var result = await FactoryClient.PostJsonRequestAsync(route, {fakeEntityVariableName});

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }}";
    }
}
