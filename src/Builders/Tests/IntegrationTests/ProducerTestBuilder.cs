﻿using Kuzaine.Services;
using Domain;
using Helpers;



namespace Kuzaine.Builders.Tests.IntegrationTests;

public class ProducerTestBuilder
{
    private readonly IKuzaineUtilities _utilities;

    public ProducerTestBuilder(IKuzaineUtilities utilities)
    {
        _utilities = utilities;
    }

    public void CreateTests(string solutionDirectory, string testDirectory, string srcDirectory, Producer producer, string projectBaseName)
    {
        var classPath = ClassPathHelper.FeatureTestClassPath(testDirectory, $"{producer.ProducerName}Tests.cs", "EventHandlers", projectBaseName);
        var fileText = WriteTestFileText(solutionDirectory, testDirectory, srcDirectory, classPath, producer, projectBaseName);
        _utilities.CreateFile(classPath, fileText);
    }

    private static string WriteTestFileText(string solutionDirectory, string testDirectory, string srcDirectory, ClassPath classPath, Producer producer, string projectBaseName)
    {
        var testFixtureName = FileNames.GetIntegrationTestFixtureName();
        var producerClassPath = ClassPathHelper.ProducerFeaturesClassPath(srcDirectory, "", producer.DomainDirectory, projectBaseName);

        var messagesClassPath = ClassPathHelper.MessagesClassPath(solutionDirectory, "");
        return @$"namespace {classPath.ClassNamespace};

using FluentAssertions;
using NUnit.Framework;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Testing;
using {messagesClassPath.ClassNamespace};
using Microsoft.Extensions.DependencyInjection;
using Moq;
using {producerClassPath.ClassNamespace};
using static {testFixtureName};

public class {producer.ProducerName}Tests : TestBase
{{
    {ProducerTest(producer)}
}}";
    }

    private static string ProducerTest(Producer producer)
    {
        var messageName = FileNames.MessageInterfaceName(producer.MessageName);

        return $@"[Test]
    public async Task can_produce_{producer.MessageName}_message()
    {{
        // Arrange
        var command = new {producer.ProducerName}.{producer.ProducerName}Command();

        // Act
        await SendAsync(command);

        // Assert
        (await IsPublished<{messageName}>()).Should().BeTrue();
    }}";
    }
}
