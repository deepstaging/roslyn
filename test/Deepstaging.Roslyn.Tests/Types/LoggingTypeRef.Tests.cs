// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Types;

public class LoggerTypeRefTests
{
    [Test]
    public async Task Creates_globally_qualified_type()
    {
        var typeRef = new LoggerTypeRef("MyService");

        await Assert.That((string)typeRef)
            .IsEqualTo("global::Microsoft.Extensions.Logging.ILogger<MyService>");
    }

    [Test]
    public async Task Carries_category_type()
    {
        var typeRef = new LoggerTypeRef("MyService");

        await Assert.That((string)typeRef.CategoryType).IsEqualTo("MyService");
    }
}

public class LoggingTypesTests
{
    [Test]
    public async Task ILogger_produces_correct_type()
    {
        await Assert.That(LoggingTypes.ILogger.Value)
            .IsEqualTo("global::Microsoft.Extensions.Logging.ILogger");
    }

    [Test]
    public async Task ILoggerFactory_produces_correct_type()
    {
        await Assert.That(LoggingTypes.ILoggerFactory.Value)
            .IsEqualTo("global::Microsoft.Extensions.Logging.ILoggerFactory");
    }

    [Test]
    public async Task LogLevel_produces_correct_type()
    {
        await Assert.That(LoggingTypes.LogLevel.Value)
            .IsEqualTo("global::Microsoft.Extensions.Logging.LogLevel");
    }

    [Test]
    public async Task Logger_produces_generic_ILogger()
    {
        var typeRef = LoggingTypes.Logger("MyService");

        await Assert.That((string)typeRef)
            .IsEqualTo("global::Microsoft.Extensions.Logging.ILogger<MyService>");
    }
}
