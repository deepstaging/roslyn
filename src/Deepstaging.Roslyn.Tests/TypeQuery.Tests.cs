// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Tests;

public class TypeQueryTests : RoslynTestBase
{
    [Test]
    public async Task Can_find_public_classes()
    {
        var code = """
            public class PublicClass { }
            internal class InternalClass { }
            """;

        var types = SymbolsFor(code)
            .Types()
            .ThatArePublic()
            .GetAll();

        await Assert.That(types.Any(t => t.Value.Name == "PublicClass")).IsTrue();
        await Assert.That(types.Any(t => t.Value.Name == "InternalClass")).IsFalse();
    }

    [Test]
    public async Task Can_filter_interfaces()
    {
        var code = """
            public interface IService { }
            public class ServiceClass { }
            """;

        var interfaces = SymbolsFor(code)
            .Types()
            .ThatAreInterfaces()
            .GetAll();

        await Assert.That(interfaces.Any(t => t.Value.Name == "IService")).IsTrue();
        await Assert.That(interfaces.Any(t => t.Value.Name == "ServiceClass")).IsFalse();
    }

    [Test]
    public async Task Can_filter_by_attribute()
    {
        var code = """
            using System;
            
            [Obsolete]
            public class OldClass { }
            public class NewClass { }
            """;

        var obsoleteTypes = SymbolsFor(code)
            .Types()
            .WithAttribute("Obsolete")
            .GetAll();

        await Assert.That(obsoleteTypes.Any(t => t.Value.Name == "OldClass")).IsTrue();
        await Assert.That(obsoleteTypes.Any(t => t.Value.Name == "NewClass")).IsFalse();
    }
}
