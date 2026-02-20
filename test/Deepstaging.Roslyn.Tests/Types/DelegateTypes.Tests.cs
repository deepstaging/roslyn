// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Types;

public class DelegateTypesTests
{
    [Test]
    public async Task Func_with_no_params()
    {
        await Assert.That((string)DelegateTypes.Func("int"))
            .IsEqualTo("global::System.Func<int>");
    }

    [Test]
    public async Task Func_with_TypeRef_array_params()
    {
        await Assert.That((string)DelegateTypes.Func(new TypeRef[] { "string" }, "int"))
            .IsEqualTo("global::System.Func<string, int>");
    }

    [Test]
    public async Task Func_with_string_array_params()
    {
        await Assert.That((string)DelegateTypes.Func(new string[] { "string", "int" }, "bool"))
            .IsEqualTo("global::System.Func<string, int, bool>");
    }

    [Test]
    public async Task Action_with_no_params()
    {
        await Assert.That((string)DelegateTypes.Action(Array.Empty<TypeRef>()))
            .IsEqualTo("global::System.Action");
    }

    [Test]
    public async Task Action_with_single_param()
    {
        await Assert.That((string)DelegateTypes.Action(new TypeRef[] { "string" }))
            .IsEqualTo("global::System.Action<string>");
    }

    [Test]
    public async Task Action_with_multiple_params()
    {
        await Assert.That((string)DelegateTypes.Action(new TypeRef[] { "string", "int" }))
            .IsEqualTo("global::System.Action<string, int>");
    }

    [Test]
    public async Task Action_with_string_array_overload()
    {
        await Assert.That((string)DelegateTypes.Action(new string[] { "string" }))
            .IsEqualTo("global::System.Action<string>");
    }

    [Test]
    public async Task Predicate_returns_globally_qualified_type()
    {
        await Assert.That(DelegateTypes.Predicate("Customer").Value)
            .IsEqualTo("global::System.Predicate<Customer>");
    }

    [Test]
    public async Task Predicate_with_string_overload()
    {
        await Assert.That(DelegateTypes.Predicate("int").Value)
            .IsEqualTo("global::System.Predicate<int>");
    }

    [Test]
    public async Task Action_nullable_appends_question_mark()
    {
        await Assert.That(DelegateTypes.Action("string").Nullable().Value)
            .IsEqualTo("global::System.Action<string>?");
    }

    [Test]
    public async Task Func_nullable_appends_question_mark()
    {
        await Assert.That(DelegateTypes.Func("int").Nullable().Value)
            .IsEqualTo("global::System.Func<int>?");
    }
}
