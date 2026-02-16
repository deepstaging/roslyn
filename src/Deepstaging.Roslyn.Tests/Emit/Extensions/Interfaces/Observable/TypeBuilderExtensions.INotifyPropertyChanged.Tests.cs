// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit.Interfaces.Observable;

namespace Deepstaging.Roslyn.Tests.Emit.Interfaces.Observable;

public class TypeBuilderNotifyPropertyChangedTests : RoslynTestBase
{
    [Test]
    public async Task ImplementsINotifyPropertyChanged_adds_event_and_method()
    {
        var result = TypeBuilder
            .Class("ViewModel")
            .InNamespace("Test")
            .ImplementsINotifyPropertyChanged()
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("INotifyPropertyChanged");
        await Assert.That(result.Code).Contains("event");
        await Assert.That(result.Code).Contains("PropertyChanged");
        await Assert.That(result.Code).Contains("OnPropertyChanged");
        await Assert.That(result.Code).Contains("CallerMemberName");
    }

    [Test]
    public async Task ImplementsINotifyPropertyChanged_with_custom_method_name()
    {
        var result = TypeBuilder
            .Class("ViewModel")
            .InNamespace("Test")
            .ImplementsINotifyPropertyChanged("RaisePropertyChanged")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("RaisePropertyChanged");
        await Assert.That(result.Code).DoesNotContain("OnPropertyChanged");
    }
}