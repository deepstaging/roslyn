// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Interfaces.Observable;

namespace Deepstaging.Roslyn.Tests.Expressions;

public class PropertyBuilderNotifyingSetterTests : RoslynTestBase
{
    [Test]
    public async Task WithNotifyingSetter_generates_equality_guard_and_notification()
    {
        var result = TypeBuilder
            .Class("TestClass")
            .InNamespace("Test")
            .ImplementsINotifyPropertyChanged()
            .AddProperty("Name", "string", p => p
                .WithBackingField("_name")
                .WithNotifyingSetter("_name"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("EqualityComparer<string>.Default.Equals(_name, value)");
        await Assert.That(result.Code).Contains("_name = value");
        await Assert.That(result.Code).Contains("OnPropertyChanged(nameof(Name))");
    }

    [Test]
    public async Task WithNotifyingSetter_includes_also_notify()
    {
        var result = TypeBuilder
            .Class("TestClass")
            .InNamespace("Test")
            .ImplementsINotifyPropertyChanged()
            .AddProperty("FirstName", "string", p => p
                .WithBackingField("_firstName")
                .WithNotifyingSetter("_firstName", "OnPropertyChanged", "FullName", "DisplayName"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("OnPropertyChanged(nameof(FirstName))");
        await Assert.That(result.Code).Contains("OnPropertyChanged(nameof(FullName))");
        await Assert.That(result.Code).Contains("OnPropertyChanged(nameof(DisplayName))");
    }

    [Test]
    public async Task WithNotifyingSetter_custom_method_name()
    {
        var result = TypeBuilder
            .Class("TestClass")
            .InNamespace("Test")
            .ImplementsINotifyPropertyChanged("RaisePropertyChanged")
            .AddProperty("Age", "int", p => p
                .WithBackingField("_age")
                .WithNotifyingSetter("_age", "RaisePropertyChanged"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("RaisePropertyChanged(nameof(Age))");
    }
}