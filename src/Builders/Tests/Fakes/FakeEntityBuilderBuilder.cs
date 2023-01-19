﻿using System;
using System.IO;
using Domain;
using Domain.Enums;
using Helpers;
using Services;



namespace Kuzaine.Builders.Tests.Fakes;

public class FakeEntityBuilderBuilder
{
    private readonly IKuzaineUtilities _utilities;

    public FakeEntityBuilderBuilder(IKuzaineUtilities utilities)
    {
        _utilities = utilities;
    }

    ///<summary>
    /// this test is to show and record possible failures and errors
    ///<summary/>

    public void CreateFakeBuilder(string srcDirectory, string testDirectory, string projectBaseName, Entity entity)
    {
        // ****this class path will have an invalid FullClassPath. just need the directory
        var classPath = ClassPathHelper.TestFakesClassPath(testDirectory, $"", entity.Name, projectBaseName);

        if (!Directory.Exists(classPath.ClassDirectory))
            Directory.CreateDirectory(classPath.ClassDirectory);

        CreateFakeBuilderFile(srcDirectory, testDirectory, entity, projectBaseName);
    }

    private void CreateFakeBuilderFile(string srcDirectory, string testDirectory, Entity entity, string projectBaseName)
    {
        var classPath = ClassPathHelper.TestFakesClassPath(testDirectory, $"{FileNames.FakeBuilderName(entity.Name)}.cs", entity.Name, projectBaseName);
        var fileText = GetCreateFakeBuilderFileText(classPath.ClassNamespace, entity, srcDirectory, testDirectory, projectBaseName);
        _utilities.CreateFile(classPath, fileText);
    }

    private static string GetCreateFakeBuilderFileText(string classNamespace, Entity entity, string srcDirectory, string testDirectory, string projectBaseName)
    {
        var entitiesClassPath = ClassPathHelper.EntityClassPath(testDirectory, "", entity.Plural, projectBaseName);
        var dtoClassPath = ClassPathHelper.DtoClassPath(srcDirectory, "", entity.Plural, projectBaseName);
        var creationDtoName = FileNames.GetDtoName(entity.Name, Dto.Creation);
        var fakeCreationDtoName = $"Fake{creationDtoName}";

        return @$"namespace {classNamespace};

using {entitiesClassPath.ClassNamespace};
using {dtoClassPath.ClassNamespace};

public class {FileNames.FakeBuilderName(entity.Name)}
{{
    private {creationDtoName} _creationData = new {fakeCreationDtoName}().Generate();

    public {FileNames.FakeBuilderName(entity.Name)} WithDto({creationDtoName} dto)
    {{
        _creationData = dto;
        return this;
    }}
    
    public {entity.Name} Build()
    {{
        var result = {entity.Name}.Create(_creationData);
        return result;
    }}
}}";
    }
}
