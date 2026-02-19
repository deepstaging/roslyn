// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Expressions;

public class TypeBuilderLazyFieldTests : RoslynTestBase
{
    [Test]
    public async Task WithLazyField_adds_field_and_property()
    {
        var result = TypeBuilder
            .Class("ServiceHost")
            .InNamespace("Test")
            .WithLazyField("ExpensiveService", "_service", "() => new ExpensiveService()", "Service")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("Lazy<ExpensiveService>");
        await Assert.That(result.Code).Contains("_service");
        await Assert.That(result.Code).Contains("_service.Value");
    }
}