// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Emit;

public class TypeRefTests
{
    #region Factory Methods

    [Test]
    public async Task From_creates_simple_type()
    {
        TypeRef typeRef = TypeRef.From("string");

        await Assert.That((string)typeRef).IsEqualTo("string");
    }

    [Test]
    public async Task From_creates_qualified_type()
    {
        TypeRef typeRef = TypeRef.From("System.Collections.Generic.List");

        await Assert.That((string)typeRef).IsEqualTo("System.Collections.Generic.List");
    }

    [Test]
    public async Task Global_adds_global_prefix()
    {
        TypeRef typeRef = TypeRef.Global("System.String");

        await Assert.That((string)typeRef).IsEqualTo("global::System.String");
    }

    #endregion

    #region Nullable

    [Test]
    public async Task Nullable_appends_question_mark()
    {
        TypeRef typeRef = TypeRef.From("string").Nullable();

        await Assert.That((string)typeRef).IsEqualTo("string?");
    }

    [Test]
    public async Task Nullable_on_value_type()
    {
        TypeRef typeRef = TypeRef.From("int").Nullable();

        await Assert.That((string)typeRef).IsEqualTo("int?");
    }

    #endregion

    #region Arrays

    [Test]
    public async Task Array_appends_brackets()
    {
        TypeRef typeRef = TypeRef.From("string").Array();

        await Assert.That((string)typeRef).IsEqualTo("string[]");
    }

    [Test]
    public async Task Array_with_rank_1()
    {
        TypeRef typeRef = TypeRef.From("int").Array(1);

        await Assert.That((string)typeRef).IsEqualTo("int[]");
    }

    [Test]
    public async Task Array_with_rank_2()
    {
        TypeRef typeRef = TypeRef.From("int").Array(2);

        await Assert.That((string)typeRef).IsEqualTo("int[,]");
    }

    [Test]
    public async Task Array_with_rank_3()
    {
        TypeRef typeRef = TypeRef.From("int").Array(3);

        await Assert.That((string)typeRef).IsEqualTo("int[,,]");
    }

    #endregion

    #region Generics

    [Test]
    public async Task Of_single_type_argument()
    {
        TypeRef typeRef = TypeRef.From("List").Of("string");

        await Assert.That((string)typeRef).IsEqualTo("List<string>");
    }

    [Test]
    public async Task Of_multiple_type_arguments()
    {
        TypeRef typeRef = TypeRef.From("Dictionary").Of("string", "int");

        await Assert.That((string)typeRef).IsEqualTo("Dictionary<string, int>");
    }

    [Test]
    public async Task Of_nested_generics()
    {
        TypeRef typeRef = TypeRef.From("Dictionary").Of(
            "string",
            TypeRef.From("List").Of("int"));

        await Assert.That((string)typeRef).IsEqualTo("Dictionary<string, List<int>>");
    }

    #endregion

    #region Exceptions

    [Test]
    public async Task Exceptions_ArgumentNull_creates_globally_qualified_type()
    {
        TypeRef typeRef = ExceptionRefs.ArgumentNull;

        await Assert.That((string)typeRef).IsEqualTo("global::System.ArgumentNullException");
    }

    [Test]
    public async Task Exceptions_Argument_creates_globally_qualified_type()
    {
        TypeRef typeRef = ExceptionRefs.Argument;

        await Assert.That((string)typeRef).IsEqualTo("global::System.ArgumentException");
    }

    [Test]
    public async Task Exceptions_ArgumentOutOfRange_creates_globally_qualified_type()
    {
        TypeRef typeRef = ExceptionRefs.ArgumentOutOfRange;

        await Assert.That((string)typeRef).IsEqualTo("global::System.ArgumentOutOfRangeException");
    }

    [Test]
    public async Task Exceptions_InvalidOperation_creates_globally_qualified_type()
    {
        TypeRef typeRef = ExceptionRefs.InvalidOperation;

        await Assert.That((string)typeRef).IsEqualTo("global::System.InvalidOperationException");
    }

    [Test]
    public async Task Exceptions_InvalidCast_creates_globally_qualified_type()
    {
        TypeRef typeRef = ExceptionRefs.InvalidCast;

        await Assert.That((string)typeRef).IsEqualTo("global::System.InvalidCastException");
    }

    [Test]
    public async Task Exceptions_Format_creates_globally_qualified_type()
    {
        TypeRef typeRef = ExceptionRefs.Format;

        await Assert.That((string)typeRef).IsEqualTo("global::System.FormatException");
    }

    [Test]
    public async Task Exceptions_NotSupported_creates_globally_qualified_type()
    {
        TypeRef typeRef = ExceptionRefs.NotSupported;

        await Assert.That((string)typeRef).IsEqualTo("global::System.NotSupportedException");
    }

    [Test]
    public async Task Exceptions_NotImplemented_creates_globally_qualified_type()
    {
        TypeRef typeRef = ExceptionRefs.NotImplemented;

        await Assert.That((string)typeRef).IsEqualTo("global::System.NotImplementedException");
    }

    #endregion

    #region Collections

    [Test]
    public async Task List_creates_globally_qualified_type()
    {
        TypeRef typeRef = CollectionRefs.List("string");

        await Assert.That((string)typeRef).IsEqualTo("global::System.Collections.Generic.List<string>");
    }

    [Test]
    public async Task Dictionary_creates_globally_qualified_type()
    {
        TypeRef typeRef = CollectionRefs.Dictionary("string", "int");

        await Assert.That((string)typeRef).IsEqualTo("global::System.Collections.Generic.Dictionary<string, int>");
    }

    [Test]
    public async Task HashSet_creates_globally_qualified_type()
    {
        TypeRef typeRef = CollectionRefs.HashSet("int");

        await Assert.That((string)typeRef).IsEqualTo("global::System.Collections.Generic.HashSet<int>");
    }

    [Test]
    public async Task KeyValuePair_creates_globally_qualified_type()
    {
        TypeRef typeRef = CollectionRefs.KeyValuePair("string", "int");

        await Assert.That((string)typeRef).IsEqualTo("global::System.Collections.Generic.KeyValuePair<string, int>");
    }

    [Test]
    public async Task IEnumerable_creates_globally_qualified_type()
    {
        TypeRef typeRef = CollectionRefs.IEnumerable("string");

        await Assert.That((string)typeRef).IsEqualTo("global::System.Collections.Generic.IEnumerable<string>");
    }

    [Test]
    public async Task ICollection_creates_globally_qualified_type()
    {
        TypeRef typeRef = CollectionRefs.ICollection("string");

        await Assert.That((string)typeRef).IsEqualTo("global::System.Collections.Generic.ICollection<string>");
    }

    [Test]
    public async Task IList_creates_globally_qualified_type()
    {
        TypeRef typeRef = CollectionRefs.IList("string");

        await Assert.That((string)typeRef).IsEqualTo("global::System.Collections.Generic.IList<string>");
    }

    [Test]
    public async Task IDictionary_creates_globally_qualified_type()
    {
        TypeRef typeRef = CollectionRefs.IDictionary("string", "int");

        await Assert.That((string)typeRef).IsEqualTo("global::System.Collections.Generic.IDictionary<string, int>");
    }

    [Test]
    public async Task ISet_creates_globally_qualified_type()
    {
        TypeRef typeRef = CollectionRefs.ISet("int");

        await Assert.That((string)typeRef).IsEqualTo("global::System.Collections.Generic.ISet<int>");
    }

    [Test]
    public async Task IReadOnlyList_creates_globally_qualified_type()
    {
        TypeRef typeRef = CollectionRefs.IReadOnlyList("string");

        await Assert.That((string)typeRef).IsEqualTo("global::System.Collections.Generic.IReadOnlyList<string>");
    }

    [Test]
    public async Task IReadOnlyCollection_creates_globally_qualified_type()
    {
        TypeRef typeRef = CollectionRefs.IReadOnlyCollection("string");

        await Assert.That((string)typeRef).IsEqualTo("global::System.Collections.Generic.IReadOnlyCollection<string>");
    }

    [Test]
    public async Task IReadOnlyDictionary_creates_globally_qualified_type()
    {
        TypeRef typeRef = CollectionRefs.IReadOnlyDictionary("string", "int");

        await Assert.That((string)typeRef).IsEqualTo("global::System.Collections.Generic.IReadOnlyDictionary<string, int>");
    }

    #endregion

    #region Immutable Collections

    [Test]
    public async Task ImmutableArray_creates_globally_qualified_type()
    {
        TypeRef typeRef = ImmutableCollectionRefs.ImmutableArray("string");

        await Assert.That((string)typeRef).IsEqualTo("global::System.Collections.Immutable.ImmutableArray<string>");
    }

    [Test]
    public async Task ImmutableList_creates_globally_qualified_type()
    {
        TypeRef typeRef = ImmutableCollectionRefs.ImmutableList("string");

        await Assert.That((string)typeRef).IsEqualTo("global::System.Collections.Immutable.ImmutableList<string>");
    }

    [Test]
    public async Task ImmutableDictionary_creates_globally_qualified_type()
    {
        TypeRef typeRef = ImmutableCollectionRefs.ImmutableDictionary("string", "int");

        await Assert.That((string)typeRef).IsEqualTo("global::System.Collections.Immutable.ImmutableDictionary<string, int>");
    }

    #endregion

    #region Tasks

    [Test]
    public async Task Task_non_generic()
    {
        TypeRef typeRef = TaskRefs.Task();

        await Assert.That((string)typeRef).IsEqualTo("global::System.Threading.Tasks.Task");
    }

    [Test]
    public async Task Task_generic()
    {
        TypeRef typeRef = TaskRefs.Task("string");

        await Assert.That((string)typeRef).IsEqualTo("global::System.Threading.Tasks.Task<string>");
    }

    [Test]
    public async Task ValueTask_non_generic()
    {
        TypeRef typeRef = TaskRefs.ValueTask();

        await Assert.That((string)typeRef).IsEqualTo("global::System.Threading.Tasks.ValueTask");
    }

    [Test]
    public async Task ValueTask_generic()
    {
        TypeRef typeRef = TaskRefs.ValueTask("int");

        await Assert.That((string)typeRef).IsEqualTo("global::System.Threading.Tasks.ValueTask<int>");
    }

    [Test]
    public async Task CompletedTask_expression()
    {
        TypeRef typeRef = TaskRefs.CompletedTask;

        await Assert.That((string)typeRef).IsEqualTo("global::System.Threading.Tasks.Task.CompletedTask");
    }

    [Test]
    public async Task CompletedValueTask_expression()
    {
        TypeRef typeRef = TaskRefs.CompletedValueTask;

        await Assert.That((string)typeRef).IsEqualTo("global::System.Threading.Tasks.ValueTask.CompletedTask");
    }

    #endregion

    #region Other Common Types

    [Test]
    public async Task CancellationToken_creates_globally_qualified_type()
    {
        TypeRef typeRef = TaskRefs.CancellationToken;

        await Assert.That((string)typeRef).IsEqualTo("global::System.Threading.CancellationToken");
    }

    #endregion

    #region JSON Types

    [Test]
    public async Task JsonSerializer_creates_globally_qualified_type()
    {
        TypeRef typeRef = JsonRefs.Serializer;

        await Assert.That((string)typeRef).IsEqualTo("global::System.Text.Json.JsonSerializer");
    }

    [Test]
    public async Task JsonSerializerOptions_creates_globally_qualified_type()
    {
        TypeRef typeRef = JsonRefs.SerializerOptions;

        await Assert.That((string)typeRef).IsEqualTo("global::System.Text.Json.JsonSerializerOptions");
    }

    [Test]
    public async Task Utf8JsonReader_creates_globally_qualified_type()
    {
        TypeRef typeRef = JsonRefs.Reader;

        await Assert.That((string)typeRef).IsEqualTo("global::System.Text.Json.Utf8JsonReader");
    }

    [Test]
    public async Task Utf8JsonWriter_creates_globally_qualified_type()
    {
        TypeRef typeRef = JsonRefs.Writer;

        await Assert.That((string)typeRef).IsEqualTo("global::System.Text.Json.Utf8JsonWriter");
    }

    [Test]
    public async Task JsonConverter_creates_generic_globally_qualified_type()
    {
        TypeRef typeRef = JsonRefs.Converter("CustomerId");

        await Assert.That((string)typeRef).IsEqualTo("global::System.Text.Json.Serialization.JsonConverter<CustomerId>");
    }

    [Test]
    public async Task JsonConverterAttribute_creates_globally_qualified_type()
    {
        TypeRef typeRef = JsonRefs.ConverterAttribute;

        await Assert.That((string)typeRef).IsEqualTo("global::System.Text.Json.Serialization.JsonConverter");
    }

    #endregion

    #region Encoding Types

    [Test]
    public async Task Encoding_UTF8_creates_globally_qualified_expression()
    {
        TypeRef typeRef = EncodingRefs.UTF8;

        await Assert.That((string)typeRef).IsEqualTo("global::System.Text.Encoding.UTF8");
    }

    [Test]
    public async Task Encoding_ASCII_creates_globally_qualified_expression()
    {
        TypeRef typeRef = EncodingRefs.ASCII;

        await Assert.That((string)typeRef).IsEqualTo("global::System.Text.Encoding.ASCII");
    }

    [Test]
    public async Task Encoding_Unicode_creates_globally_qualified_expression()
    {
        TypeRef typeRef = EncodingRefs.Unicode;

        await Assert.That((string)typeRef).IsEqualTo("global::System.Text.Encoding.Unicode");
    }

    #endregion

    #region HTTP Types

    [Test]
    public async Task HttpClient_creates_globally_qualified_type()
    {
        TypeRef typeRef = HttpRefs.Client;

        await Assert.That((string)typeRef).IsEqualTo("global::System.Net.Http.HttpClient");
    }

    [Test]
    public async Task HttpRequestMessage_creates_globally_qualified_type()
    {
        TypeRef typeRef = HttpRefs.RequestMessage;

        await Assert.That((string)typeRef).IsEqualTo("global::System.Net.Http.HttpRequestMessage");
    }

    [Test]
    public async Task HttpResponseMessage_creates_globally_qualified_type()
    {
        TypeRef typeRef = HttpRefs.ResponseMessage;

        await Assert.That((string)typeRef).IsEqualTo("global::System.Net.Http.HttpResponseMessage");
    }

    [Test]
    public async Task HttpMethod_creates_globally_qualified_type()
    {
        TypeRef typeRef = HttpRefs.Method;

        await Assert.That((string)typeRef).IsEqualTo("global::System.Net.Http.HttpMethod");
    }

    [Test]
    public async Task HttpVerb_creates_method_expression()
    {
        TypeRef typeRef = HttpRefs.Verb("Get");

        await Assert.That((string)typeRef).IsEqualTo("global::System.Net.Http.HttpMethod.Get");
    }

    [Test]
    public void HttpVerb_throws_on_unknown_verb()
    {
        Assert.Throws<ArgumentException>(() => HttpRefs.Verb("Yeet"));
    }

    [Test]
    public async Task HttpGet_creates_method_expression()
    {
        TypeRef typeRef = HttpRefs.Get;

        await Assert.That((string)typeRef).IsEqualTo("global::System.Net.Http.HttpMethod.Get");
    }

    [Test]
    public async Task HttpPost_creates_method_expression()
    {
        TypeRef typeRef = HttpRefs.Post;

        await Assert.That((string)typeRef).IsEqualTo("global::System.Net.Http.HttpMethod.Post");
    }

    [Test]
    public async Task HttpContent_creates_globally_qualified_type()
    {
        TypeRef typeRef = HttpRefs.Content;

        await Assert.That((string)typeRef).IsEqualTo("global::System.Net.Http.HttpContent");
    }

    [Test]
    public async Task StringContent_creates_globally_qualified_type()
    {
        TypeRef typeRef = HttpRefs.StringContent;

        await Assert.That((string)typeRef).IsEqualTo("global::System.Net.Http.StringContent");
    }

    [Test]
    public async Task ByteArrayContent_creates_globally_qualified_type()
    {
        TypeRef typeRef = HttpRefs.ByteArrayContent;

        await Assert.That((string)typeRef).IsEqualTo("global::System.Net.Http.ByteArrayContent");
    }

    [Test]
    public async Task StreamContent_creates_globally_qualified_type()
    {
        TypeRef typeRef = HttpRefs.StreamContent;

        await Assert.That((string)typeRef).IsEqualTo("global::System.Net.Http.StreamContent");
    }

    #endregion

    #region Common Types Composition

    [Test]
    public async Task List_nullable()
    {
        TypeRef typeRef = CollectionRefs.List("string").Nullable();

        await Assert.That((string)typeRef).IsEqualTo("global::System.Collections.Generic.List<string>?");
    }

    [Test]
    public async Task Dictionary_with_nested_list()
    {
        TypeRef typeRef = CollectionRefs.Dictionary("string", CollectionRefs.List("int"));

        await Assert.That((string)typeRef).IsEqualTo("global::System.Collections.Generic.Dictionary<string, global::System.Collections.Generic.List<int>>");
    }

    [Test]
    public async Task Task_of_list()
    {
        TypeRef typeRef = TaskRefs.Task(CollectionRefs.List("string"));

        await Assert.That((string)typeRef).IsEqualTo("global::System.Threading.Tasks.Task<global::System.Collections.Generic.List<string>>");
    }

    [Test]
    public async Task IReadOnlyList_array()
    {
        TypeRef typeRef = CollectionRefs.IReadOnlyList("int").Array();

        await Assert.That((string)typeRef).IsEqualTo("global::System.Collections.Generic.IReadOnlyList<int>[]");
    }

    #endregion

    #region Delegate Invocation

    [Test]
    public async Task Invoke_with_no_arguments()
    {
        TypeRef typeRef = TypeRef.From("OnSave").Invoke();

        await Assert.That((string)typeRef).IsEqualTo("OnSave?.Invoke()");
    }

    [Test]
    public async Task Invoke_with_single_argument()
    {
        TypeRef typeRef = TypeRef.From("OnSave").Invoke("id");

        await Assert.That((string)typeRef).IsEqualTo("OnSave?.Invoke(id)");
    }

    [Test]
    public async Task Invoke_with_multiple_arguments()
    {
        TypeRef typeRef = TypeRef.From("OnSendEmail").Invoke("to", "body");

        await Assert.That((string)typeRef).IsEqualTo("OnSendEmail?.Invoke(to, body)");
    }

    [Test]
    public async Task Invoke_with_OrDefault()
    {
        TypeRef typeRef = TypeRef.From("OnSendEmail")
            .Invoke("to", "body")
            .OrDefault(TaskRefs.CompletedTask);

        await Assert.That((string)typeRef).IsEqualTo("OnSendEmail?.Invoke(to, body) ?? global::System.Threading.Tasks.Task.CompletedTask");
    }

    [Test]
    public async Task OrDefault_with_string_fallback()
    {
        TypeRef typeRef = TypeRef.From("OnGetName")
            .Invoke()
            .OrDefault("default!");

        await Assert.That((string)typeRef).IsEqualTo("OnGetName?.Invoke() ?? default!");
    }

    #endregion

    #region Delegates

    [Test]
    public async Task Func_with_return_type_only()
    {
        TypeRef typeRef = DelegateRefs.Func("int");

        await Assert.That((string)typeRef).IsEqualTo("global::System.Func<int>");
    }

    [Test]
    public async Task Func_with_parameter_and_return_type()
    {
        TypeRef typeRef = DelegateRefs.Func("int", "string");

        await Assert.That((string)typeRef).IsEqualTo("global::System.Func<int, string>");
    }

    [Test]
    public async Task Func_with_multiple_parameters()
    {
        TypeRef typeRef = DelegateRefs.Func("int", "string", "bool");

        await Assert.That((string)typeRef).IsEqualTo("global::System.Func<int, string, bool>");
    }

    [Test]
    public async Task Action_with_no_arguments()
    {
        TypeRef typeRef = DelegateRefs.Action();

        await Assert.That((string)typeRef).IsEqualTo("global::System.Action");
    }

    [Test]
    public async Task Action_with_single_argument()
    {
        TypeRef typeRef = DelegateRefs.Action("string");

        await Assert.That((string)typeRef).IsEqualTo("global::System.Action<string>");
    }

    [Test]
    public async Task Action_with_multiple_arguments()
    {
        TypeRef typeRef = DelegateRefs.Action("string", "int");

        await Assert.That((string)typeRef).IsEqualTo("global::System.Action<string, int>");
    }

    #endregion

    #region Tuples

    [Test]
    public async Task Tuple_with_two_elements()
    {
        TypeRef typeRef = TypeRef.Tuple(("string", "Name"), ("int", "Age"));

        await Assert.That((string)typeRef).IsEqualTo("(string Name, int Age)");
    }

    [Test]
    public async Task Tuple_with_three_elements()
    {
        TypeRef typeRef = TypeRef.Tuple(
            ("string", "First"),
            ("string", "Last"),
            ("int", "Age"));

        await Assert.That((string)typeRef).IsEqualTo("(string First, string Last, int Age)");
    }

    #endregion

    #region Composition

    [Test]
    public async Task Nullable_func_delegate()
    {
        TypeRef typeRef = DelegateRefs.Func("string", "bool").Nullable();

        await Assert.That((string)typeRef).IsEqualTo("global::System.Func<string, bool>?");
    }

    [Test]
    public async Task Func_with_generic_arguments()
    {
        TypeRef typeRef = DelegateRefs.Func(
            TypeRef.From("List").Of("string"),
            TypeRef.From("int"));

        await Assert.That((string)typeRef).IsEqualTo("global::System.Func<List<string>, int>");
    }

    [Test]
    public async Task Nullable_generic_array()
    {
        TypeRef typeRef = TypeRef.From("List").Of("string").Nullable().Array();

        await Assert.That((string)typeRef).IsEqualTo("List<string>?[]");
    }

    [Test]
    public async Task Global_generic_nullable()
    {
        TypeRef typeRef = TypeRef.Global("System.Collections.Generic.List").Of("string").Nullable();

        await Assert.That((string)typeRef).IsEqualTo("global::System.Collections.Generic.List<string>?");
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

        await Assert.That((string)typeRef).IsEqualTo("string");
    }

    [Test]
    public async Task Implicit_conversion_in_Of()
    {
        // strings implicitly convert to TypeRef in Of() params
        TypeRef typeRef = TypeRef.From("List").Of("string");

        await Assert.That((string)typeRef).IsEqualTo("List<string>");
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
    public void Func_throws_on_no_arguments()
    {
        Assert.Throws<ArgumentException>(() => DelegateRefs.Func());
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
