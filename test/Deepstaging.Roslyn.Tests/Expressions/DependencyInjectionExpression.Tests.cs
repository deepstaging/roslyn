// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Expressions;

public class DependencyInjectionExpressionTests
{
    [Test]
    public async Task AddSingleton_with_impl_produces_correct_expression()
    {
        var expr = DependencyInjectionExpression.AddSingleton("services", "IService", "ServiceImpl");

        await Assert.That(expr.Value).IsEqualTo("services.AddSingleton<IService, ServiceImpl>()");
    }

    [Test]
    public async Task AddSingleton_produces_correct_expression()
    {
        var expr = DependencyInjectionExpression.AddSingleton("services", "IService");

        await Assert.That(expr.Value).IsEqualTo("services.AddSingleton<IService>()");
    }

    [Test]
    public async Task AddScoped_with_impl_produces_correct_expression()
    {
        var expr = DependencyInjectionExpression.AddScoped("services", "IService", "ServiceImpl");

        await Assert.That(expr.Value).IsEqualTo("services.AddScoped<IService, ServiceImpl>()");
    }

    [Test]
    public async Task AddScoped_produces_correct_expression()
    {
        var expr = DependencyInjectionExpression.AddScoped("services", "IService");

        await Assert.That(expr.Value).IsEqualTo("services.AddScoped<IService>()");
    }

    [Test]
    public async Task AddTransient_with_impl_produces_correct_expression()
    {
        var expr = DependencyInjectionExpression.AddTransient("services", "IService", "ServiceImpl");

        await Assert.That(expr.Value).IsEqualTo("services.AddTransient<IService, ServiceImpl>()");
    }

    [Test]
    public async Task AddTransient_produces_correct_expression()
    {
        var expr = DependencyInjectionExpression.AddTransient("services", "IService");

        await Assert.That(expr.Value).IsEqualTo("services.AddTransient<IService>()");
    }

    [Test]
    public async Task GetRequiredService_produces_correct_expression()
    {
        var expr = DependencyInjectionExpression.GetRequiredService("provider", "IService");

        await Assert.That(expr.Value).IsEqualTo("provider.GetRequiredService<IService>()");
    }

    [Test]
    public async Task GetService_produces_correct_expression()
    {
        var expr = DependencyInjectionExpression.GetService("provider", "IService");

        await Assert.That(expr.Value).IsEqualTo("provider.GetService<IService>()");
    }
}
