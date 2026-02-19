// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System.Text.Json.Nodes;

namespace Deepstaging.Roslyn.Tests.Extensions.Json;

public class JsonMergeTests
{
    #region Apply Tests

    [Test]
    public async Task Apply_adds_missing_keys()
    {
        var existing = """{ "a": 1 }""";
        var template = """{ "a": 0, "b": 2 }""";

        var result = JsonMerge.Apply(existing, template);
        var obj = JsonNode.Parse(result)!.AsObject();

        await Assert.That((int)obj["a"]!).IsEqualTo(1);
        await Assert.That((int)obj["b"]!).IsEqualTo(2);
    }

    [Test]
    public async Task Apply_preserves_existing_values()
    {
        var existing = """{ "a": "user-value" }""";
        var template = """{ "a": "default" }""";

        var result = JsonMerge.Apply(existing, template);
        var obj = JsonNode.Parse(result)!.AsObject();

        await Assert.That((string)obj["a"]!).IsEqualTo("user-value");
    }

    [Test]
    public async Task Apply_does_not_remove_extra_keys()
    {
        var existing = """{ "a": 1, "old": "stale" }""";
        var template = """{ "a": 0 }""";

        var result = JsonMerge.Apply(existing, template);
        var obj = JsonNode.Parse(result)!.AsObject();

        await Assert.That(obj.ContainsKey("old")).IsTrue();
    }

    #endregion

    #region Sync Tests

    [Test]
    public async Task Sync_adds_missing_keys()
    {
        var existing = """{ "a": 1 }""";
        var template = """{ "a": 0, "b": 2 }""";

        var result = JsonMerge.Sync(existing, template);
        var obj = JsonNode.Parse(result)!.AsObject();

        await Assert.That((int)obj["a"]!).IsEqualTo(1);
        await Assert.That((int)obj["b"]!).IsEqualTo(2);
    }

    [Test]
    public async Task Sync_preserves_existing_values()
    {
        var existing = """{ "a": "user-value" }""";
        var template = """{ "a": "default" }""";

        var result = JsonMerge.Sync(existing, template);
        var obj = JsonNode.Parse(result)!.AsObject();

        await Assert.That((string)obj["a"]!).IsEqualTo("user-value");
    }

    [Test]
    public async Task Sync_removes_keys_not_in_template()
    {
        var existing = """{ "a": 1, "old": "stale" }""";
        var template = """{ "a": 0 }""";

        var result = JsonMerge.Sync(existing, template);
        var obj = JsonNode.Parse(result)!.AsObject();

        await Assert.That(obj.ContainsKey("a")).IsTrue();
        await Assert.That(obj.ContainsKey("old")).IsFalse();
    }

    [Test]
    public async Task Sync_preserves_dollar_prefixed_keys()
    {
        var existing = """{ "$schema": "./my.schema.json", "a": 1 }""";
        var template = """{ "a": 0 }""";

        var result = JsonMerge.Sync(existing, template);
        var obj = JsonNode.Parse(result)!.AsObject();

        await Assert.That((string)obj["$schema"]!).IsEqualTo("./my.schema.json");
        await Assert.That((int)obj["a"]!).IsEqualTo(1);
    }

    [Test]
    public async Task Sync_removes_nested_keys_not_in_template()
    {
        var existing = """
                       {
                           "Section": {
                               "Config": {
                                   "OldProp": "old",
                                   "KeptProp": "user"
                               }
                           }
                       }
                       """;

        var template = """
                       {
                           "Section": {
                               "Config": {
                                   "KeptProp": "default",
                                   "NewProp": "new"
                               }
                           }
                       }
                       """;

        var result = JsonMerge.Sync(existing, template);
        var config = JsonNode.Parse(result)!["Section"]!["Config"]!.AsObject();

        await Assert.That(config.ContainsKey("OldProp")).IsFalse();
        await Assert.That((string)config["KeptProp"]!).IsEqualTo("user");
        await Assert.That((string)config["NewProp"]!).IsEqualTo("new");
    }

    [Test]
    public async Task Sync_removes_nested_section_not_in_template()
    {
        var existing = """
                       {
                           "Section": {
                               "RemovedType": { "Prop": "val" },
                               "KeptType": { "Prop": "user" }
                           }
                       }
                       """;

        var template = """
                       {
                           "Section": {
                               "KeptType": { "Prop": "default" }
                           }
                       }
                       """;

        var result = JsonMerge.Sync(existing, template);
        var section = JsonNode.Parse(result)!["Section"]!.AsObject();

        await Assert.That(section.ContainsKey("RemovedType")).IsFalse();
        await Assert.That((string)section["KeptType"]!["Prop"]!).IsEqualTo("user");
    }

    [Test]
    public async Task Sync_writes_template_when_existing_is_not_object()
    {
        var existing = """[ 1, 2, 3 ]""";
        var template = """{ "a": 1 }""";

        var result = JsonMerge.Sync(existing, template);

        await Assert.That(result).IsEqualTo(template);
    }

    #endregion
}