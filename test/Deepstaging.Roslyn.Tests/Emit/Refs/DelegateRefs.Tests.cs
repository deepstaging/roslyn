// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Emit.Refs;

public class DelegateRefsTests
{
    [Test]
    public async Task Func_with_return_type_only()
    {
        var typeRef = DelegateRefs.Func("int");

        await Assert.That(typeRef).IsEqualTo("global::System.Func<int>");
    }

    [Test]
    public async Task Func_with_parameter_and_return_type()
    {
        var typeRef = DelegateRefs.Func("int", "string");

        await Assert.That(typeRef).IsEqualTo("global::System.Func<int, string>");
    }

    [Test]
    public async Task Func_with_multiple_parameters()
    {
        var typeRef = DelegateRefs.Func("int", "string", "bool");

        await Assert.That(typeRef).IsEqualTo("global::System.Func<int, string, bool>");
    }

    [Test]
    public async Task Action_with_no_arguments()
    {
        var typeRef = DelegateRefs.Action();

        await Assert.That(typeRef).IsEqualTo("global::System.Action");
    }

    [Test]
    public async Task Action_with_single_argument()
    {
        var typeRef = DelegateRefs.Action("string");

        await Assert.That(typeRef).IsEqualTo("global::System.Action<string>");
    }

    [Test]
    public async Task Action_with_multiple_arguments()
    {
        var typeRef = DelegateRefs.Action("string", "int");

        await Assert.That(typeRef).IsEqualTo("global::System.Action<string, int>");
    }

    #region Composition

    [Test]
    public async Task Nullable_func_delegate()
    {
        var typeRef = DelegateRefs.Func("string", "bool").Nullable();

        await Assert.That(typeRef).IsEqualTo("global::System.Func<string, bool>?");
    }

    [Test]
    public async Task Func_with_generic_arguments()
    {
        var typeRef = DelegateRefs.Func(
            TypeRef.From("List").Of("string"),
            TypeRef.From("int"));

        await Assert.That(typeRef).IsEqualTo("global::System.Func<List<string>, int>");
    }

    #endregion

    #region Edge Cases

    [Test]
    public void Func_throws_on_no_arguments() => Assert.Throws<ArgumentException>(() => DelegateRefs.Func());

    #endregion
}