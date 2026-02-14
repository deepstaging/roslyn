// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Scriban;

namespace Deepstaging.Roslyn.Tests.Scriban;

public class TemplateMapTests
{
    #region Bind - Simple Properties

    [Test]
    public async Task Bind_returns_value_unchanged()
    {
        var map = new TemplateMap<TestModel>();
        var result = map.Bind("Hello", m => m.Name);

        await Assert.That(result).IsEqualTo("Hello");
    }

    [Test]
    public async Task Bind_records_property_path()
    {
        var map = new TemplateMap<TestModel>();
        map.Bind("Hello", m => m.Name);

        await Assert.That(map.Bindings).Count().IsEqualTo(1);
        await Assert.That(map.Bindings[0].PropertyPath).IsEqualTo("Name");
        await Assert.That(map.Bindings[0].Value).IsEqualTo("Hello");
    }

    [Test]
    public async Task Bind_records_multiple_bindings_in_order()
    {
        var map = new TemplateMap<TestModel>();
        map.Bind("MyType", m => m.Name);
        map.Bind("TestApp", m => m.Namespace);

        await Assert.That(map.Bindings).Count().IsEqualTo(2);
        await Assert.That(map.Bindings[0].PropertyPath).IsEqualTo("Name");
        await Assert.That(map.Bindings[1].PropertyPath).IsEqualTo("Namespace");
    }

    #endregion

    #region Bind - Nested Properties

    [Test]
    public async Task Bind_extracts_nested_property_path()
    {
        var map = new TemplateMap<TestModel>();
        map.Bind("int", m => m.Nested.CodeName);

        await Assert.That(map.Bindings).Count().IsEqualTo(1);
        await Assert.That(map.Bindings[0].PropertyPath).IsEqualTo("Nested.CodeName");
        await Assert.That(map.Bindings[0].Value).IsEqualTo("int");
    }

    #endregion

    #region Bind - Null/Empty Skipping

    [Test]
    public async Task Bind_skips_null_value()
    {
        var map = new TemplateMap<TestModel>();
        map.Bind<string>(null!, m => m.Name);

        await Assert.That(map.Bindings).Count().IsEqualTo(0);
    }

    [Test]
    public async Task Bind_skips_empty_string()
    {
        var map = new TemplateMap<TestModel>();
        map.Bind("", m => m.Name);

        await Assert.That(map.Bindings).Count().IsEqualTo(0);
    }

    [Test]
    public async Task Bind_skips_whitespace_only_string()
    {
        var map = new TemplateMap<TestModel>();
        map.Bind("   ", m => m.Name);

        await Assert.That(map.Bindings).Count().IsEqualTo(0);
    }

    #endregion

    #region Bind - Non-String Types

    [Test]
    public async Task Bind_converts_int_to_string()
    {
        var map = new TemplateMap<TestModel>();
        map.Bind(42, m => m.Count);

        await Assert.That(map.Bindings).Count().IsEqualTo(1);
        await Assert.That(map.Bindings[0].Value).IsEqualTo("42");
    }

    #endregion

    #region Test Models

    private sealed class TestModel
    {
        public string Name { get; init; } = "";
        public string Namespace { get; init; } = "";
        public int Count { get; init; }
        public NestedModel Nested { get; init; } = new();
    }

    private sealed class NestedModel
    {
        public string CodeName { get; init; } = "";
        public string GloballyQualifiedName { get; init; } = "";
    }

    #endregion
}
