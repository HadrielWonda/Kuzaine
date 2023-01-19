﻿using System;
using Kuzaine.Domain.Enums;
using Kuzaine.Helpers;
using Kuzaine.Services;



namespace Kuzaine.Builders.Tests.IntegrationTests.UserRoles;

public class AddRemoveUserRoleTestsBuilder
{
    private readonly IKuzaineUtilities _utilities;

    public AddRemoveUserRoleTestsBuilder(IKuzaineUtilities utilities)
    {
        _utilities = utilities;
    }

    public void CreateTests(string testDirectory, string srcDirectory, string projectBaseName)
    {
        var classPath = ClassPathHelper.FeatureTestClassPath(testDirectory, $"AddRemoveUserRoleTests.cs", "Users", projectBaseName);
        var fileText = WriteTestFileText(testDirectory, srcDirectory, classPath, projectBaseName);
        _utilities.CreateFile(classPath, fileText);
    }

    private static string WriteTestFileText(string testDirectory, string srcDirectory, ClassPath classPath, string projectBaseName)
    {
        var testFixtureName = FileNames.GetIntegrationTestFixtureName();
        var fakerClassPath = ClassPathHelper.TestFakesClassPath(testDirectory, "", "User", projectBaseName);
        var featuresClassPath = ClassPathHelper.FeaturesClassPath(srcDirectory, "", "Users", projectBaseName);
        var rolesClassPath = ClassPathHelper.EntityClassPath(srcDirectory, "", "Roles", projectBaseName);


        return @$"namespace {classPath.ClassNamespace};

using {rolesClassPath.ClassNamespace};
using {fakerClassPath.ClassNamespace};
using {featuresClassPath.ClassNamespace};
using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Threading.Tasks;
using static {testFixtureName};

public class {classPath.ClassNameWithoutExt} : TestBase
{{    
    [Test]
    public async Task can_add_and_remove_role()
    {{
        // Arrange
        var faker = new Faker();
        var fakeUserOne = FakeUser.Generate(new FakeUserForCreationDto().Generate());
        await InsertAsync(fakeUserOne);

        var user = await ExecuteDbContextAsync(db => db.Users
            .FirstOrDefaultAsync(u => u.Id == fakeUserOne.Id));
        var id = user.Id;
        var role = faker.PickRandom<RoleEnum>(RoleEnum.List).Name;

        // Act - Add
        var command = new AddUserRole.Command(id, role);
        await SendAsync(command);
        var updatedUser = await ExecuteDbContextAsync(db => db.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == id));

        // Assert - Add
        updatedUser.Roles.Count.Should().Be(1);
        updatedUser.Roles.FirstOrDefault().Role.Value.Should().Be(role);
        
        // Act - Remove
        var removeCommand = new RemoveUserRole.Command(id, role);
        await SendAsync(removeCommand);
        
        // Assert - Remove
        updatedUser = await ExecuteDbContextAsync(db => db.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == id));
        updatedUser.Roles.Count.Should().Be(0);
    }}
}}";
    }
}
