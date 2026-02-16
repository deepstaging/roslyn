// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Emit.Refs;

public class ExpressionRefTests
{
    #region Factory

    [Test]
    public async Task From_creates_expression()
    {
        ExpressionRef expr = ExpressionRef.From("value");

        await Assert.That(expr).IsEqualTo("value");
    }

    [Test]
    public void From_throws_on_null()
    {
        Assert.Throws<ArgumentException>(() => ExpressionRef.From(null!));
    }

    [Test]
    public void From_throws_on_empty()
    {
        Assert.Throws<ArgumentException>(() => ExpressionRef.From(""));
    }

    #endregion

    #region Implicit Conversions

    [Test]
    public async Task Implicit_from_string()
    {
        ExpressionRef expr = "value + 1";

        await Assert.That(expr).IsEqualTo("value + 1");
    }

    [Test]
    public async Task Implicit_to_string()
    {
        string result = ExpressionRef.From("value");

        await Assert.That(result).IsEqualTo("value");
    }

    [Test]
    public async Task Implicit_from_TypeRef()
    {
        ExpressionRef expr = TypeRef.From("string");

        await Assert.That(expr).IsEqualTo("string");
    }

    [Test]
    public async Task Implicit_from_TypeRef_preserves_value()
    {
        ExpressionRef expr = CollectionRefs.List("int");

        await Assert.That(expr).IsEqualTo("global::System.Collections.Generic.List<int>");
    }

    #endregion

    #region New (via TypeRef gateway)

    [Test]
    public async Task New_with_no_arguments()
    {
        ExpressionRef expr = CollectionRefs.List("string").New();

        await Assert.That(expr).IsEqualTo("new global::System.Collections.Generic.List<string>()");
    }

    [Test]
    public async Task New_with_single_argument()
    {
        ExpressionRef expr = ExceptionRefs.ArgumentNull.New("nameof(value)");

        await Assert.That(expr).IsEqualTo("new global::System.ArgumentNullException(nameof(value))");
    }

    [Test]
    public async Task New_with_multiple_arguments()
    {
        ExpressionRef expr = TypeRef.From("KeyValuePair").New("key", "value");

        await Assert.That(expr).IsEqualTo("new KeyValuePair(key, value)");
    }

    #endregion

    #region Call (via TypeRef gateway)

    [Test]
    public async Task Call_static_method_no_args()
    {
        ExpressionRef expr = TaskRefs.Task().Call("CompletedTask");

        await Assert.That(expr).IsEqualTo("global::System.Threading.Tasks.Task.CompletedTask()");
    }

    [Test]
    public async Task Call_static_method_with_args()
    {
        ExpressionRef expr = TypeRef.From("Guid").Call("Parse", "input");

        await Assert.That(expr).IsEqualTo("Guid.Parse(input)");
    }

    [Test]
    public async Task Call_static_method_with_multiple_args()
    {
        ExpressionRef expr = TypeRef.From("int").Call("Parse", "input", "provider");

        await Assert.That(expr).IsEqualTo("int.Parse(input, provider)");
    }

    [Test]
    public async Task Call_on_exception_ref()
    {
        ExpressionRef expr = ExceptionRefs.ArgumentNull.Call("ThrowIfNull", "param");

        await Assert.That(expr).IsEqualTo("global::System.ArgumentNullException.ThrowIfNull(param)");
    }

    #endregion

    #region Member (via TypeRef gateway)

    [Test]
    public async Task Member_access()
    {
        ExpressionRef expr = TypeRef.From("string").Member("Empty");

        await Assert.That(expr).IsEqualTo("string.Empty");
    }

    [Test]
    public async Task Member_on_global_type()
    {
        ExpressionRef expr = TypeRef.Global("System.Text.Encoding").Member("UTF8");

        await Assert.That(expr).IsEqualTo("global::System.Text.Encoding.UTF8");
    }

    #endregion

    #region TypeOf / Default / NameOf (via TypeRef gateway)

    [Test]
    public async Task TypeOf_simple()
    {
        ExpressionRef expr = TypeRef.From("string").TypeOf();

        await Assert.That(expr).IsEqualTo("typeof(string)");
    }

    [Test]
    public async Task TypeOf_generic()
    {
        ExpressionRef expr = JsonRefs.Converter("OrderId").TypeOf();

        await Assert.That(expr).IsEqualTo("typeof(global::System.Text.Json.Serialization.JsonConverter<OrderId>)");
    }

    [Test]
    public async Task Default_expression()
    {
        ExpressionRef expr = TaskRefs.CancellationToken.Default();

        await Assert.That(expr).IsEqualTo("default(global::System.Threading.CancellationToken)");
    }

    [Test]
    public async Task NameOf_expression()
    {
        ExpressionRef expr = TypeRef.From("CustomerId").NameOf();

        await Assert.That(expr).IsEqualTo("nameof(CustomerId)");
    }

    #endregion

    #region ExpressionRef Chaining — Call

    [Test]
    public async Task Chained_call()
    {
        ExpressionRef expr = ExpressionRef.From("value")
            .Call("ToString")
            .Call("Trim");

        await Assert.That(expr).IsEqualTo("value.ToString().Trim()");
    }

    [Test]
    public async Task Call_with_arguments()
    {
        ExpressionRef expr = ExpressionRef.From("list").Call("Add", "item");

        await Assert.That(expr).IsEqualTo("list.Add(item)");
    }

    #endregion

    #region ExpressionRef Chaining — Member

    [Test]
    public async Task Chained_member_access()
    {
        ExpressionRef expr = ExpressionRef.From("value")
            .Member("Name")
            .Member("Length");

        await Assert.That(expr).IsEqualTo("value.Name.Length");
    }

    [Test]
    public async Task Member_then_call()
    {
        ExpressionRef expr = ExpressionRef.From("value")
            .Member("Name")
            .Call("ToUpper");

        await Assert.That(expr).IsEqualTo("value.Name.ToUpper()");
    }

    #endregion

    #region Type Operations

    [Test]
    public async Task As_expression()
    {
        ExpressionRef expr = ExpressionRef.From("obj").As("string");

        await Assert.That(expr).IsEqualTo("obj as string");
    }

    [Test]
    public async Task Cast_expression()
    {
        ExpressionRef expr = ExpressionRef.From("obj").Cast("int");

        await Assert.That(expr).IsEqualTo("(int)obj");
    }

    [Test]
    public async Task Is_expression()
    {
        ExpressionRef expr = ExpressionRef.From("obj").Is("string");

        await Assert.That(expr).IsEqualTo("obj is string");
    }

    [Test]
    public async Task Is_with_pattern_variable()
    {
        ExpressionRef expr = ExpressionRef.From("obj").Is(TypeRef.From("string"), "text");

        await Assert.That(expr).IsEqualTo("obj is string text");
    }

    #endregion

    #region Null Handling

    [Test]
    public async Task OrDefault_expression()
    {
        ExpressionRef expr = ExpressionRef.From("value").OrDefault("fallback");

        await Assert.That(expr).IsEqualTo("value ?? fallback");
    }

    [Test]
    public async Task NullForgiving_expression()
    {
        ExpressionRef expr = ExpressionRef.From("value").NullForgiving();

        await Assert.That(expr).IsEqualTo("value!");
    }

    [Test]
    public async Task NullConditionalMember_expression()
    {
        ExpressionRef expr = ExpressionRef.From("handler").NullConditionalMember("Name");

        await Assert.That(expr).IsEqualTo("handler?.Name");
    }

    [Test]
    public async Task NullConditionalCall_expression()
    {
        ExpressionRef expr = ExpressionRef.From("handler").NullConditionalCall("Dispose");

        await Assert.That(expr).IsEqualTo("handler?.Dispose()");
    }

    #endregion

    #region Async

    [Test]
    public async Task Await_expression()
    {
        ExpressionRef expr = ExpressionRef.From("task").Await();

        await Assert.That(expr).IsEqualTo("await task");
    }

    [Test]
    public async Task ConfigureAwait_false()
    {
        ExpressionRef expr = ExpressionRef.From("stream")
            .Call("FlushAsync")
            .ConfigureAwait(false);

        await Assert.That(expr).IsEqualTo("stream.FlushAsync().ConfigureAwait(false)");
    }

    [Test]
    public async Task ConfigureAwait_true()
    {
        ExpressionRef expr = ExpressionRef.From("stream")
            .Call("FlushAsync")
            .ConfigureAwait(true);

        await Assert.That(expr).IsEqualTo("stream.FlushAsync().ConfigureAwait(true)");
    }

    [Test]
    public async Task Await_with_ConfigureAwait()
    {
        ExpressionRef expr = ExpressionRef.From("disposable")
            .Call("DisposeAsync")
            .ConfigureAwait(false)
            .Await();

        await Assert.That(expr).IsEqualTo("await disposable.DisposeAsync().ConfigureAwait(false)");
    }

    #endregion

    #region Parenthesization

    [Test]
    public async Task Parenthesize_wraps()
    {
        ExpressionRef expr = ExpressionRef.From("a + b").Parenthesize();

        await Assert.That(expr).IsEqualTo("(a + b)");
    }

    [Test]
    public async Task As_then_NullForgiving_with_parentheses()
    {
        ExpressionRef expr = ExpressionRef.From("value")
            .As("string")
            .Parenthesize()
            .NullForgiving();

        await Assert.That(expr).IsEqualTo("(value as string)!");
    }

    #endregion

    #region Complex Composition

    [Test]
    public async Task New_exception_with_nameof()
    {
        ExpressionRef expr = ExceptionRefs.ArgumentNull
            .New(TypeRef.From("value").NameOf());

        await Assert.That(expr).IsEqualTo("new global::System.ArgumentNullException(nameof(value))");
    }

    [Test]
    public async Task Delegate_invoke_with_await_and_fallback()
    {
        ExpressionRef expr = TypeRef.From("OnSaveAsync")
            .Invoke("entity")
            .OrDefault(TaskRefs.CompletedTask)
            .Await();

        await Assert.That(expr).IsEqualTo("await OnSaveAsync?.Invoke(entity) ?? global::System.Threading.Tasks.Task.CompletedTask");
    }

    [Test]
    public async Task Static_method_parse_pattern()
    {
        ExpressionRef expr = TypeRef.From("Guid").Call("Parse", "input", "provider");

        await Assert.That(expr).IsEqualTo("Guid.Parse(input, provider)");
    }

    [Test]
    public async Task Throw_if_null_pattern()
    {
        ExpressionRef expr = ExceptionRefs.ArgumentNull.Call("ThrowIfNull", "value");

        await Assert.That(expr).IsEqualTo("global::System.ArgumentNullException.ThrowIfNull(value)");
    }

    [Test]
    public async Task Typeof_for_json_converter()
    {
        ExpressionRef expr = JsonRefs.Converter("CustomerId").TypeOf();

        await Assert.That(expr).IsEqualTo("typeof(global::System.Text.Json.Serialization.JsonConverter<CustomerId>)");
    }

    [Test]
    public async Task ToString_returns_value()
    {
        var expr = ExpressionRef.From("x + y");

        await Assert.That(expr.ToString()).IsEqualTo("x + y");
    }

    #endregion
}
