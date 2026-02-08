// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Interfaces.Observable;

namespace Deepstaging.Roslyn.Tests.Emit.Interfaces.Observable;

/// <summary>
/// Tests for TypeBuilder IObserver interface implementation extensions.
/// </summary>
public class TypeBuilderIObserverExtensionsTests : RoslynTestBase
{
    [Test]
    public async Task IObserver_with_expression_bodies()
    {
        var result = TypeBuilder
            .Class("MessageObserver")
            .InNamespace("Test")
            .ImplementsIObserver("Message", "_handler(value)")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("IObserver<Message>");
        await Assert.That(result.Code).Contains("void OnNext(Message value)");
        await Assert.That(result.Code).Contains("_handler(value)");
        await Assert.That(result.Code).Contains("void OnError(global::System.Exception error)");
        await Assert.That(result.Code).Contains("throw error;");
        await Assert.That(result.Code).Contains("void OnCompleted()");
    }

    [Test]
    public async Task IObserver_with_custom_error_and_completed()
    {
        var result = TypeBuilder
            .Class("LoggingObserver")
            .InNamespace("Test")
            .ImplementsIObserver(
                "Event",
                "_logger.Log(value)",
                "_logger.LogError(error)",
                "_logger.LogCompleted()")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("_logger.Log(value)");
        await Assert.That(result.Code).Contains("_logger.LogError(error)");
        await Assert.That(result.Code).Contains("_logger.LogCompleted()");
    }

    [Test]
    public async Task IObserver_virtual_generates_overridable_methods()
    {
        var result = TypeBuilder
            .Class("BaseObserver")
            .InNamespace("Test")
            .ImplementsIObserverVirtual("Event")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public virtual void OnNext(Event value)");
        await Assert.That(result.Code).Contains("public virtual void OnError");
        await Assert.That(result.Code).Contains("public virtual void OnCompleted()");
    }

    [Test]
    public async Task IObserver_with_action_fields()
    {
        var result = TypeBuilder
            .Class("DelegatingObserver")
            .InNamespace("Test")
            .ImplementsIObserverWithActionFields("Item")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("Action<Item>? _onNext");
        await Assert.That(result.Code).Contains("Action<global::System.Exception>? _onError");
        await Assert.That(result.Code).Contains("Action? _onCompleted");
        await Assert.That(result.Code).Contains("_onNext?.Invoke(value)");
        await Assert.That(result.Code).Contains("_onError");
        await Assert.That(result.Code).Contains("_onCompleted?.Invoke()");
    }
}
