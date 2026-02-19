// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Expressions;

public class DependencyInjectionExpressionTests
{
    [Test]
    public async Task AddSingleton_two_type_args_produces_expression()
    {
        var expr = DependencyInjectionExpression.AddSingleton("services", "IService", "ServiceImpl");

        await Assert.That(expr.Value).Contains("AddSingleton<IService, ServiceImpl>()");
    }

    [Test]
    public async Task GetRequiredService_produces_expression()
    {
        var expr = DependencyInjectionExpression.GetRequiredService("provider", "IService");

        await Assert.That(expr.Value).Contains("GetRequiredService<IService>()");
    }
}
