﻿namespace Kuzaine.Tests
{
    using Kuzaine.Helpers;
    using FluentAssertions;
    using System.IO;
    using Services;
    using Xunit;

    public class ClassPathHelperTests
    {
        [Fact]
        public void ControllerClassPath_returns_accurate_path()
        {
            var path = ClassPathHelper.ControllerClassPath("", "ProductName.cs", "Ordering");

            path.ClassDirectory.Should().Be(Path.Combine("Ordering.WebApi", "Controllers", "v1"));
        }
    }
}
