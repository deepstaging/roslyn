// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Tests.Projections.Extensions;

public class OptionalSymbolEventsTests : RoslynTestBase
{
    [Test]
    public async Task GetEventType_returns_event_handler_type()
    {
        var code = """
            using System;
            
            public class TestClass
            {
                public event EventHandler MyEvent;
            }
            """;

        var myEvent = SymbolsFor(code)
            .GetNamedType("TestClass")
            .QueryEvents()
            .WithName("MyEvent")
            .FirstOrDefault();

        var eventType = myEvent.Symbol!.Type;

        await Assert.That(eventType).IsNotNull();
        await Assert.That(eventType.Name).IsEqualTo("EventHandler");
    }

    [Test]
    public async Task IsStaticEvent_detects_static_events()
    {
        var code = """
            using System;
            
            public class TestClass
            {
                public static event EventHandler StaticEvent;
                public event EventHandler InstanceEvent;
            }
            """;

        var context = SymbolsFor(code);

        var staticEvent = context.GetNamedType("TestClass").QueryEvents().WithName("StaticEvent").FirstOrDefault();
        var instanceEvent = context.GetNamedType("TestClass").QueryEvents().WithName("InstanceEvent").FirstOrDefault();

        await Assert.That(staticEvent.IsStatic).IsTrue();
        await Assert.That(instanceEvent.IsStatic).IsFalse();
    }

    [Test]
    public async Task GetAddMethod_returns_add_accessor()
    {
        var code = """
            using System;
            
            public class TestClass
            {
                public event EventHandler MyEvent;
            }
            """;

        var myEvent = SymbolsFor(code).GetNamedType("TestClass").QueryEvents().WithName("MyEvent").FirstOrDefault();
        var addMethod = myEvent.GetAddMethod();

        await Assert.That(addMethod.HasValue).IsTrue();
    }

    [Test]
    public async Task GetRemoveMethod_returns_remove_accessor()
    {
        var code = """
            using System;
            
            public class TestClass
            {
                public event EventHandler MyEvent;
            }
            """;

        var myEvent = SymbolsFor(code).GetNamedType("TestClass").QueryEvents().WithName("MyEvent").FirstOrDefault();
        var removeMethod = myEvent.GetRemoveMethod();

        await Assert.That(removeMethod.HasValue).IsTrue();
    }
}
