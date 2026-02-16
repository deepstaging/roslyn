// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Emit.Refs;

public class TypeRefTests
{
    #region Factory Methods

    [Test]
    public async Task From_creates_simple_type()
    {
        TypeRef typeRef = TypeRef.From("string");

        await Assert.That(typeRef).IsEqualTo("string");
    }

    [Test]
    public async Task From_creates_qualified_type()
    {
        TypeRef typeRef = TypeRef.From("System.Collections.Generic.List");

        await Assert.That(typeRef).IsEqualTo("System.Collections.Generic.List");
    }

    [Test]
    public async Task Global_adds_global_prefix()
    {
        TypeRef typeRef = TypeRef.Global("System.String");

        await Assert.That(typeRef).IsEqualTo("global::System.String");
    }

    #endregion

    #region Nullable

    [Test]
    public async Task Nullable_appends_question_mark()
    {
        TypeRef typeRef = TypeRef.From("string").Nullable();

        await Assert.That(typeRef).IsEqualTo("string?");
    }

    [Test]
    public async Task Nullable_on_value_type()
    {
        TypeRef typeRef = TypeRef.From("int").Nullable();

        await Assert.That(typeRef).IsEqualTo("int?");
    }

    #endregion

    #region Arrays

    [Test]
    public async Task Array_appends_brackets()
    {
        TypeRef typeRef = TypeRef.From("string").Array();

        await Assert.That(typeRef).IsEqualTo("string[]");
    }

    [Test]
    public async Task Array_with_rank_1()
    {
        TypeRef typeRef = TypeRef.From("int").Array(1);

        await Assert.That(typeRef).IsEqualTo("int[]");
    }

    [Test]
    public async Task Array_with_rank_2()
    {
        TypeRef typeRef = TypeRef.From("int").Array(2);

        await Assert.That(typeRef).IsEqualTo("int[,]");
    }

    [Test]
    public async Task Array_with_rank_3()
    {
        TypeRef typeRef = TypeRef.From("int").Array(3);

        await Assert.That(typeRef).IsEqualTo("int[,,]");
    }

    #endregion

    #region Generics

    [Test]
    public async Task Of_single_type_argument()
    {
        TypeRef typeRef = TypeRef.From("List").Of("string");

        await Assert.That(typeRef).IsEqualTo("List<string>");
    }

    [Test]
    public async Task Of_multiple_type_arguments()
    {
        TypeRef typeRef = TypeRef.From("Dictionary").Of("string", "int");

        await Assert.That(typeRef).IsEqualTo("Dictionary<string, int>");
    }

    [Test]
    public async Task Of_nested_generics()
    {
        TypeRef typeRef = TypeRef.From("Dictionary").Of(
            "string",
            TypeRef.From("List").Of("int"));

        await Assert.That(typeRef).IsEqualTo("Dictionary<string, List<int>>");
    }

    #endregion

    #region Tuples

    [Test]
    public async Task Tuple_with_two_elements()
    {
        TypeRef typeRef = TypeRef.Tuple(("string", "Name"), ("int", "Age"));

        await Assert.That(typeRef).IsEqualTo("(string Name, int Age)");
    }

    [Test]
    public async Task Tuple_with_three_elements()
    {
        TypeRef typeRef = TypeRef.Tuple(
            ("string", "First"),
            ("string", "Last"),
            ("int", "Age"));

        await Assert.That(typeRef).IsEqualTo("(string First, string Last, int Age)");
    }

    #endregion

    #region Delegate Invocation

    [Test]
    public async Task Invoke_with_no_arguments()
    {
        ExpressionRef expr = TypeRef.From("OnSave").Invoke();

        await Assert.That(expr).IsEqualTo("OnSave?.Invoke()");
    }

    [Test]
    public async Task Invoke_with_single_argument()
    {
        ExpressionRef expr = TypeRef.From("OnSave").Invoke("id");

        await Assert.That(expr).IsEqualTo("OnSave?.Invoke(id)");
    }

    [Test]
    public async Task Invoke_with_multiple_arguments()
    {
        ExpressionRef expr = TypeRef.From("OnSendEmail").Invoke("to", "body");

        await Assert.That(expr).IsEqualTo("OnSendEmail?.Invoke(to, body)");
    }

    [Test]
    public async Task Invoke_with_OrDefault()
    {
        ExpressionRef expr = TypeRef.From("OnSendEmail")
            .Invoke("to", "body")
            .OrDefault(TaskRefs.CompletedTask);

        await Assert.That(expr).IsEqualTo("OnSendEmail?.Invoke(to, body) ?? global::System.Threading.Tasks.Task.CompletedTask");
    }

    [Test]
    public async Task OrDefault_with_string_fallback()
    {
        ExpressionRef expr = TypeRef.From("OnGetName")
            .Invoke()
            .OrDefault("default!");

        await Assert.That(expr).IsEqualTo("OnGetName?.Invoke() ?? default!");
    }

    #endregion

    #region Composition

    [Test]
    public async Task Nullable_generic_array()
    {
        TypeRef typeRef = TypeRef.From("List").Of("string").Nullable().Array();

        await Assert.That(typeRef).IsEqualTo("List<string>?[]");
    }

    [Test]
    public async Task Global_generic_nullable()
    {
        TypeRef typeRef = TypeRef.Global("System.Collections.Generic.List").Of("string").Nullable();

        await Assert.That(typeRef).IsEqualTo("global::System.Collections.Generic.List<string>?");
    }

    #endregion

    #region Implicit Conversions

    [Test]
    public async Task Implicit_conversion_to_string()
    {
        string result = TypeRef.From("int");

        await Assert.That(result).IsEqualTo("int");
    }

    [Test]
    public async Task Implicit_conversion_from_string()
    {
        TypeRef typeRef = "string";

        await Assert.That(typeRef).IsEqualTo("string");
    }

    [Test]
    public async Task Implicit_conversion_in_Of()
    {
        // strings implicitly convert to TypeRef in Of() params
        TypeRef typeRef = TypeRef.From("List").Of("string");

        await Assert.That(typeRef).IsEqualTo("List<string>");
    }

    #endregion

    #region Integration with Builders

    [Test]
    public async Task Works_with_PropertyBuilder()
    {
        var result = TypeBuilder
            .Class("Handler")
            .AddProperty("Callback", DelegateRefs.Func("string", "bool").Nullable(),
                p => p.WithAutoPropertyAccessors())
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("global::System.Func<string, bool>? Callback { get; set; }");
    }

    [Test]
    public async Task Works_with_FieldBuilder()
    {
        var result = TypeBuilder
            .Class("Handler")
            .AddField("_items", TypeRef.From("List").Of("string"),
                f => f.WithAccessibility(Accessibility.Private).AsReadonly())
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("private readonly List<string> _items");
    }

    #endregion

    #region Edge Cases

    [Test]
    public void From_throws_on_null()
    {
        Assert.Throws<ArgumentException>(() => TypeRef.From((string)null!));
    }

    [Test]
    public void From_throws_on_empty()
    {
        Assert.Throws<ArgumentException>(() => TypeRef.From(""));
    }

    [Test]
    public void From_throws_on_whitespace()
    {
        Assert.Throws<ArgumentException>(() => TypeRef.From("  "));
    }

    [Test]
    public void Tuple_throws_on_single_element()
    {
        Assert.Throws<ArgumentException>(() => TypeRef.Tuple(("string", "Name")));
    }

    [Test]
    public void Of_throws_on_no_arguments()
    {
        Assert.Throws<ArgumentException>(() => TypeRef.From("List").Of());
    }

    [Test]
    public void Array_throws_on_zero_rank()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => TypeRef.From("int").Array(0));
    }

    [Test]
    public async Task ToString_returns_value()
    {
        var typeRef = TypeRef.From("string");

        await Assert.That(typeRef.ToString()).IsEqualTo("string");
    }

    #endregion
}
