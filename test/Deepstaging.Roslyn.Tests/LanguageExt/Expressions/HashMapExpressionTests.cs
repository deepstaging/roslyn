// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.LanguageExt.Expressions;

using Roslyn.LanguageExt;
using Roslyn.LanguageExt.Expressions;
using Roslyn.LanguageExt.Types;

public class HashMapExpressionTests
{
    [Test]
    public async Task HashMap_single_pair()
    {
        var result = HashMapExpression.HashMap("(\"key\", value)");

        await Assert.That(result.Value)
            .IsEqualTo("HashMap((\"key\", value))");
    }

    [Test]
    public async Task HashMap_multiple_pairs()
    {
        var result = HashMapExpression.HashMap("(\"a\", 1)", "(\"b\", 2)");

        await Assert.That(result.Value)
            .IsEqualTo("HashMap((\"a\", 1), (\"b\", 2))");
    }

    [Test]
    public async Task toHashMap_converts_enumerable()
    {
        var result = HashMapExpression.toHashMap("items.Select(x => (x.Key, x.Value))");

        await Assert.That(result.Value)
            .IsEqualTo("toHashMap(items.Select(x => (x.Key, x.Value)))");
    }

    [Test]
    public async Task Empty_returns_typed_empty()
    {
        var result = HashMapExpression.Empty(LanguageExtTypes.HashMap("string", "int"));

        await Assert.That(result.Value)
            .IsEqualTo("global::LanguageExt.HashMap<string, int>.Empty");
    }

    [Test]
    public async Task toHashMap_is_chainable()
    {
        var result = HashMapExpression.toHashMap("pairs").Call("Add", "\"key\"", "42");

        await Assert.That(result.Value)
            .IsEqualTo("toHashMap(pairs).Add(\"key\", 42)");
    }
}