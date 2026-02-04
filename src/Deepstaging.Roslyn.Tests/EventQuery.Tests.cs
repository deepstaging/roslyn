// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Tests;

public class EventQueryTests : RoslynTestBase
{
    [Test]
    public async Task Can_filter_public_events()
    {
        var code = """
            using System;
            
            public class TestClass
            {
                public event EventHandler PublicEvent;
                private event EventHandler PrivateEvent;
            }
            """;

        var events = SymbolsFor(code)
            .RequireNamedType("TestClass")
            .QueryEvents()
            .ThatArePublic()
            .GetAll();

        await Assert.That(events.Any(e => e.Value.Name == "PublicEvent")).IsTrue();
    }

    [Test]
    public async Task Can_filter_static_events()
    {
        var code = """
            using System;
            
            public class TestClass
            {
                public static event EventHandler StaticEvent;
                public event EventHandler InstanceEvent;
            }
            """;

        var staticEvents = SymbolsFor(code)
            .RequireNamedType("TestClass")
            .QueryEvents()
            .ThatAreStatic()
            .GetAll();

        await Assert.That(staticEvents.Any(e => e.Value.Name == "StaticEvent")).IsTrue();
    }
}
