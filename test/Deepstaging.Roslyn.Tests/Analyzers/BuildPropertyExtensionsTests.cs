// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Analyzers;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Deepstaging.Roslyn.Tests.Analyzers;

public class BuildPropertyExtensionsTests
{
    // ── GetBuildProperty (string) ────────────────────────────────────────

    [Test]
    public async Task GetBuildProperty_ReturnsValue_WhenPropertyExists()
    {
        var options = new TestOptions(new Dictionary<string, string>
        {
            ["build_property.DeepstagingDataDirectory"] = ".config"
        });

        var result = options.GetBuildProperty("DeepstagingDataDirectory", "default");

        await Assert.That(result).IsEqualTo(".config");
    }

    [Test]
    public async Task GetBuildProperty_ReturnsFallback_WhenPropertyMissing()
    {
        var options = new TestOptions(new Dictionary<string, string>());

        var result = options.GetBuildProperty("DeepstagingDataDirectory", ".config");

        await Assert.That(result).IsEqualTo(".config");
    }

    [Test]
    public async Task GetBuildProperty_ReturnsFallback_WhenPropertyEmpty()
    {
        var options = new TestOptions(new Dictionary<string, string>
        {
            ["build_property.DeepstagingDataDirectory"] = ""
        });

        var result = options.GetBuildProperty("DeepstagingDataDirectory", ".config");

        await Assert.That(result).IsEqualTo(".config");
    }

    // ── GetBuildProperty<T> (typed) ──────────────────────────────────────

    [Test]
    public async Task GetBuildProperty_Bool_ParsesTrue()
    {
        var options = new TestOptions(new Dictionary<string, string>
        {
            ["build_property.IsDirty"] = "true"
        });

        var result = options.GetBuildProperty("IsDirty", false);

        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task GetBuildProperty_Bool_ParsesFalse()
    {
        var options = new TestOptions(new Dictionary<string, string>
        {
            ["build_property.IsDirty"] = "false"
        });

        var result = options.GetBuildProperty("IsDirty", true);

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task GetBuildProperty_Bool_ReturnsFallback_WhenUnparseable()
    {
        var options = new TestOptions(new Dictionary<string, string>
        {
            ["build_property.IsDirty"] = "not-a-bool"
        });

        var result = options.GetBuildProperty("IsDirty", true);

        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task GetBuildProperty_Int_ParsesValue()
    {
        var options = new TestOptions(new Dictionary<string, string>
        {
            ["build_property.DirtyCount"] = "42"
        });

        var result = options.GetBuildProperty("DirtyCount", 0);

        await Assert.That(result).IsEqualTo(42);
    }

    [Test]
    public async Task GetBuildProperty_Int_ReturnsFallback_WhenMissing()
    {
        var options = new TestOptions(new Dictionary<string, string>());

        var result = options.GetBuildProperty("DirtyCount", -1);

        await Assert.That(result).IsEqualTo(-1);
    }

    [Test]
    public async Task GetBuildProperty_Long_ParsesValue()
    {
        var options = new TestOptions(new Dictionary<string, string>
        {
            ["build_property.BigNumber"] = "9999999999"
        });

        var result = options.GetBuildProperty("BigNumber", 0L);

        await Assert.That(result).IsEqualTo(9999999999L);
    }

    [Test]
    public async Task GetBuildProperty_Double_ParsesValue()
    {
        var options = new TestOptions(new Dictionary<string, string>
        {
            ["build_property.Ratio"] = "3.14"
        });

        var result = options.GetBuildProperty("Ratio", 0.0);

        await Assert.That(result).IsEqualTo(3.14);
    }

    // ── TryGetBuildProperty ──────────────────────────────────────────────

    [Test]
    public async Task TryGetBuildProperty_ReturnsTrue_WhenPropertyExists()
    {
        var options = new TestOptions(new Dictionary<string, string>
        {
            ["build_property.MyProp"] = "hello"
        });

        var found = options.TryGetBuildProperty("MyProp", out var value);

        await Assert.That(found).IsTrue();
        await Assert.That(value).IsEqualTo("hello");
    }

    [Test]
    public async Task TryGetBuildProperty_ReturnsFalse_WhenPropertyMissing()
    {
        var options = new TestOptions(new Dictionary<string, string>());

        var found = options.TryGetBuildProperty("MyProp", out _);

        await Assert.That(found).IsFalse();
    }

    [Test]
    public async Task TryGetBuildProperty_ReturnsFalse_WhenPropertyEmpty()
    {
        var options = new TestOptions(new Dictionary<string, string>
        {
            ["build_property.MyProp"] = ""
        });

        var found = options.TryGetBuildProperty("MyProp", out _);

        await Assert.That(found).IsFalse();
    }

    // ── DiscoverBuildProperties ──────────────────────────────────────────

    [Test]
    public async Task DiscoverBuildProperties_DefaultPrefix_ReturnsDeepstagingProps()
    {
        var options = new TestOptions(new Dictionary<string, string>
        {
            ["build_property.DeepstagingDataDirectory"] = ".config",
            ["build_property.DeepstagingHasReadme"] = "true",
            ["build_property.UnrelatedProp"] = "ignored"
        });

        var result = options.DiscoverBuildProperties();

        await Assert.That(result.Count).IsEqualTo(2);
        await Assert.That(result["DeepstagingDataDirectory"]).IsEqualTo(".config");
        await Assert.That(result["DeepstagingHasReadme"]).IsEqualTo("true");
    }

    [Test]
    public async Task DiscoverBuildProperties_CustomPrefix_FiltersCorrectly()
    {
        var options = new TestOptions(new Dictionary<string, string>
        {
            ["build_property._DeepstagingGitBranch"] = "main",
            ["build_property._DeepstagingGitSha"] = "abc123",
            ["build_property._DeepstagingEnvDB_URL"] = "postgres://",
            ["build_property.DeepstagingDataDirectory"] = ".config"
        });

        var result = options.DiscoverBuildProperties("_DeepstagingGit");

        await Assert.That(result.Count).IsEqualTo(2);
        await Assert.That(result["_DeepstagingGitBranch"]).IsEqualTo("main");
        await Assert.That(result["_DeepstagingGitSha"]).IsEqualTo("abc123");
    }

    [Test]
    public async Task DiscoverBuildProperties_IgnoresNonBuildPropertyKeys()
    {
        var options = new TestOptions(new Dictionary<string, string>
        {
            ["build_property.DeepstagingFoo"] = "bar",
            ["not_build_property.DeepstagingBaz"] = "ignored"
        });

        var result = options.DiscoverBuildProperties();

        await Assert.That(result.Count).IsEqualTo(1);
        await Assert.That(result["DeepstagingFoo"]).IsEqualTo("bar");
    }

    [Test]
    public async Task DiscoverBuildProperties_ReturnsEmpty_WhenNoMatch()
    {
        var options = new TestOptions(new Dictionary<string, string>
        {
            ["build_property.SomethingElse"] = "value"
        });

        var result = options.DiscoverBuildProperties();

        await Assert.That(result.Count).IsEqualTo(0);
    }

    // ── Test infrastructure ──────────────────────────────────────────────

    private sealed class TestOptions(Dictionary<string, string> properties) : AnalyzerConfigOptions
    {
        public override IEnumerable<string> Keys => properties.Keys;

        public override bool TryGetValue(string key, out string value)
        {
            if (properties.TryGetValue(key, out var v))
            {
                value = v;
                return true;
            }

            value = "";
            return false;
        }
    }
}