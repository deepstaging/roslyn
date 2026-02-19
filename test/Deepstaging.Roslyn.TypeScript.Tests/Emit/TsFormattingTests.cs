// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.TypeScript.Emit;
using Deepstaging.Roslyn.TypeScript.Testing;

namespace Deepstaging.Roslyn.TypeScript.Tests.Emit;

public class TsFormattingTests : TsTestBase
{
    [Test]
    public async Task FormattedOutput_IsClean()
    {
        var result = TsTypeBuilder.Interface("User")
            .Exported()
            .AddProperty("id", "number", p => p.AsReadonly())
            .AddProperty("name", "string", p => p)
            .AddProperty("email", "string", p => p.AsOptional())
            .Emit(new TsEmitOptions { FormatOutput = true, HeaderComment = null });

        var code = result.ValidateOrThrow().Code;

        // Verify the formatter produced clean output (consistent indentation, semicolons)
        await Assert.That(code).Contains("export interface User");
        await Assert.That(code).Contains("readonly id: number;");
        await Assert.That(code).Contains("name: string;");
        await Assert.That(code).Contains("email?: string;");
    }

    [Test]
    public async Task FormattedClass_IsIdiomatic()
    {
        var result = TsTypeBuilder.Class("Greeter")
            .Exported()
            .AddConstructor(c => c
                .AddParameter("name", "string", p => p.AsParameterProperty(TsAccessibility.Private).AsReadonlyParameterProperty()))
            .AddMethod("greet", m => m
                .WithReturnType("string")
                .WithExpressionBody("`Hello, ${this.name}!`"))
            .Emit(new TsEmitOptions { FormatOutput = true, HeaderComment = null });

        var code = result.ValidateOrThrow().Code;
        await Assert.That(code).Contains("export class Greeter");
        await Assert.That(code).Contains("private readonly name: string");
        await Assert.That(code).Contains("greet(): string");
    }

    [Test]
    public async Task FormattedAndValidated_WorksTogether()
    {
        var result = TsTypeBuilder.Class("Stack")
            .Exported()
            .AddTypeParameter("T")
            .AddField("items", "T[]", f => f
                .WithAccessibility(TsAccessibility.Private)
                .WithInitializer("[]"))
            .AddMethod("push", m => m
                .AddParameter("item", "T")
                .WithReturnType("void")
                .WithBody(b => b
                    .AddStatement("this.items.push(item)")))
            .AddMethod("pop", m => m
                .WithReturnType("T | undefined")
                .WithExpressionBody("this.items.pop()"))
            .AddMethod("peek", m => m
                .WithReturnType("T | undefined")
                .WithExpressionBody("this.items[this.items.length - 1]"))
            .Emit(new TsEmitOptions
            {
                FormatOutput = true,
                ValidationLevel = TsValidationLevel.Syntax,
                TscPath = TscPath ?? throw new InvalidOperationException("tsc not found"),
                HeaderComment = null,
            });

        // Both formatting and validation should succeed
        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Diagnostics).IsEmpty();

        var code = result.ValidateOrThrow().Code;
        await Assert.That(code).Contains("export class Stack<T>");
        await Assert.That(code).Contains("private items: T[] = [];");
    }
}
