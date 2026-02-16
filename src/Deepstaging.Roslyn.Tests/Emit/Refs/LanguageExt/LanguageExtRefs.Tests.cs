// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Emit.Refs.LanguageExt;

using Roslyn.LanguageExt.Refs;

public class LanguageExtRefsTests
{
    [Test]
    public async Task Namespace_returns_LanguageExt()
    {
        var ns = LanguageExtRefs.Namespace;

        await Assert.That(ns.Value).IsEqualTo("LanguageExt");
    }

    [Test]
    public async Task EffectsNamespace_returns_LanguageExt_Effects()
    {
        var ns = LanguageExtRefs.EffectsNamespace;

        await Assert.That(ns.Value).IsEqualTo("LanguageExt.Effects");
    }

    [Test]
    public async Task PreludeStatic_returns_static_using()
    {
        var prelude = LanguageExtRefs.PreludeStatic;

        await Assert.That(prelude).IsEqualTo("static LanguageExt.Prelude");
    }

    [Test]
    public async Task Eff_creates_globally_qualified_type()
    {
        var typeRef = LanguageExtRefs.Eff("RT", "int");

        await Assert.That(typeRef).IsEqualTo("global::LanguageExt.Eff<RT, int>");
    }

    [Test]
    public async Task Option_creates_globally_qualified_type()
    {
        var typeRef = LanguageExtRefs.Option("string");

        await Assert.That(typeRef).IsEqualTo("global::LanguageExt.Option<string>");
    }

    [Test]
    public async Task Fin_creates_globally_qualified_type()
    {
        var typeRef = LanguageExtRefs.Fin("int");

        await Assert.That(typeRef).IsEqualTo("global::LanguageExt.Fin<int>");
    }

    [Test]
    public async Task Seq_creates_globally_qualified_type()
    {
        var typeRef = LanguageExtRefs.Seq("string");

        await Assert.That(typeRef).IsEqualTo("global::LanguageExt.Seq<string>");
    }

    [Test]
    public async Task Either_creates_globally_qualified_type()
    {
        var typeRef = LanguageExtRefs.Either("string", "int");

        await Assert.That(typeRef).IsEqualTo("global::LanguageExt.Either<string, int>");
    }

    [Test]
    public async Task HashMap_creates_globally_qualified_type()
    {
        var typeRef = LanguageExtRefs.HashMap("string", "int");

        await Assert.That(typeRef).IsEqualTo("global::LanguageExt.HashMap<string, int>");
    }

    [Test]
    public async Task Unit_creates_globally_qualified_type()
    {
        var typeRef = LanguageExtRefs.Unit;

        await Assert.That(typeRef).IsEqualTo("global::LanguageExt.Unit");
    }

    #region Composition

    [Test]
    public async Task Option_nullable()
    {
        var typeRef = LanguageExtRefs.Option("string").Nullable();

        await Assert.That(typeRef).IsEqualTo("global::LanguageExt.Option<string>?");
    }

    [Test]
    public async Task Eff_with_nested_option()
    {
        var typeRef = LanguageExtRefs.Eff("RT", LanguageExtRefs.Option("string"));

        await Assert.That(typeRef)
            .IsEqualTo("global::LanguageExt.Eff<RT, global::LanguageExt.Option<string>>");
    }

    [Test]
    public async Task Seq_with_nested_collection()
    {
        var typeRef = LanguageExtRefs.Seq(CollectionRefs.List("int"));

        await Assert.That(typeRef)
            .IsEqualTo("global::LanguageExt.Seq<global::System.Collections.Generic.List<int>>");
    }

    #endregion
}
