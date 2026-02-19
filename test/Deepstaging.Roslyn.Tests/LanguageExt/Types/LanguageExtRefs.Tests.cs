// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.LanguageExt.Types;

using Roslyn.LanguageExt.Types;

public class LanguageExtRefsTests
{
    [Test]
    public async Task Namespace_returns_LanguageExt()
    {
        var ns = NamespaceRef.From("LanguageExt");

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

        await Assert.That((string)typeRef).IsEqualTo("global::LanguageExt.Eff<RT, int>");
        await Assert.That((string)typeRef.Rt).IsEqualTo("RT");
        await Assert.That((string)typeRef.Result).IsEqualTo("int");
    }

    [Test]
    public async Task Option_creates_globally_qualified_type()
    {
        var option = LanguageExtRefs.Option("string");

        await Assert.That((string)option).IsEqualTo("global::LanguageExt.Option<string>");
        await Assert.That((string)option.InnerType).IsEqualTo("string");
    }

    [Test]
    public async Task Fin_creates_globally_qualified_type()
    {
        var typeRef = LanguageExtRefs.Fin("int");

        await Assert.That((string)typeRef).IsEqualTo("global::LanguageExt.Fin<int>");
        await Assert.That((string)typeRef.InnerType).IsEqualTo("int");
    }

    [Test]
    public async Task Seq_creates_globally_qualified_type()
    {
        var typeRef = LanguageExtRefs.Seq("string");

        await Assert.That((string)typeRef).IsEqualTo("global::LanguageExt.Seq<string>");
        await Assert.That((string)typeRef.ElementType).IsEqualTo("string");
    }

    [Test]
    public async Task Either_creates_globally_qualified_type()
    {
        var typeRef = LanguageExtRefs.Either("string", "int");

        await Assert.That((string)typeRef).IsEqualTo("global::LanguageExt.Either<string, int>");
        await Assert.That((string)typeRef.Left).IsEqualTo("string");
        await Assert.That((string)typeRef.Right).IsEqualTo("int");
    }

    [Test]
    public async Task HashMap_creates_globally_qualified_type()
    {
        var typeRef = LanguageExtRefs.HashMap("string", "int");

        await Assert.That((string)typeRef).IsEqualTo("global::LanguageExt.HashMap<string, int>");
        await Assert.That((string)typeRef.KeyType).IsEqualTo("string");
        await Assert.That((string)typeRef.ValueType).IsEqualTo("int");
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
        TypeRef typeRef = LanguageExtRefs.Option("string");

        await Assert.That(typeRef.Nullable()).IsEqualTo("global::LanguageExt.Option<string>?");
    }

    [Test]
    public async Task Eff_with_nested_option()
    {
        var typeRef = LanguageExtRefs.Eff("RT", LanguageExtRefs.Option("string"));

        await Assert.That((string)typeRef)
            .IsEqualTo("global::LanguageExt.Eff<RT, global::LanguageExt.Option<string>>");
    }

    [Test]
    public async Task Seq_with_nested_collection()
    {
        var typeRef = LanguageExtRefs.Seq((TypeRef)new ListTypeRef("int"));

        await Assert.That((string)typeRef)
            .IsEqualTo("global::LanguageExt.Seq<global::System.Collections.Generic.List<int>>");
    }

    #endregion
}