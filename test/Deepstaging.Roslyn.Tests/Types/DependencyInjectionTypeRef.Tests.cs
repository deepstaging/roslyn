// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Types;

public class DependencyInjectionTypesTests
{
    [Test]
    public async Task IServiceCollection_produces_correct_type()
    {
        await Assert.That(DependencyInjectionTypes.IServiceCollection.Value)
            .IsEqualTo("global::Microsoft.Extensions.DependencyInjection.IServiceCollection");
    }

    [Test]
    public async Task IServiceProvider_produces_correct_type()
    {
        await Assert.That(DependencyInjectionTypes.IServiceProvider.Value)
            .IsEqualTo("global::System.IServiceProvider");
    }

    [Test]
    public async Task IServiceScopeFactory_produces_correct_type()
    {
        await Assert.That(DependencyInjectionTypes.IServiceScopeFactory.Value)
            .IsEqualTo("global::Microsoft.Extensions.DependencyInjection.IServiceScopeFactory");
    }
}
