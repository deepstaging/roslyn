// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Tests.Emit;

public class SignatureParserMethodTests : RoslynTestBase
{
    [Test]
    public async Task Parse_SimpleMethod_ExtractsNameAndReturnType()
    {
        var builder = MethodBuilder.Parse("public string GetName()");

        await Assert.That(builder.Name).IsEqualTo("GetName");
        await Assert.That(builder.ReturnType).IsEqualTo("string");
    }

    [Test]
    public async Task Parse_VoidMethod_SetsVoidReturnType()
    {
        var builder = MethodBuilder.Parse("public void Execute()");

        await Assert.That(builder.ReturnType).IsEqualTo("void");
    }

    [Test]
    public async Task Parse_MethodWithParameters_AddsParameters()
    {
        var result = TypeBuilder.Class("Test")
            .AddMethod(MethodBuilder.Parse("public string Format(string name, int age)"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public string Format(string name, int age)");
    }

    [Test]
    public async Task Parse_AsyncMethod_SetsAsyncModifier()
    {
        var result = TypeBuilder.Class("Test")
            .AddMethod(MethodBuilder.Parse("public async Task<bool> ProcessAsync()"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public async Task<bool> ProcessAsync()");
    }

    [Test]
    public async Task Parse_StaticMethod_SetsStaticModifier()
    {
        var result = TypeBuilder.Class("Test")
            .AddMethod(MethodBuilder.Parse("public static int Create()"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public static int Create()");
    }

    [Test]
    public async Task Parse_VirtualMethod_SetsVirtualModifier()
    {
        var result = TypeBuilder.Class("Test")
            .AddMethod(MethodBuilder.Parse("protected virtual void OnChanged()"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("protected virtual void OnChanged()");
    }

    [Test]
    public async Task Parse_OverrideMethod_SetsOverrideModifier()
    {
        var result = TypeBuilder.Class("Test")
            .AddMethod(MethodBuilder.Parse("public override string ToString()"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public override string ToString()");
    }

    [Test]
    public async Task Parse_AbstractMethod_SetsAbstractModifier()
    {
        var result = TypeBuilder.Class("Test").AsAbstract()
            .AddMethod(MethodBuilder.Parse("public abstract void Execute()"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public abstract void Execute();");
    }

    [Test]
    public async Task Parse_PrivateMethod_SetsPrivateAccessibility()
    {
        var result = TypeBuilder.Class("Test")
            .AddMethod(MethodBuilder.Parse("private void Helper()"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("private void Helper()");
    }

    [Test]
    public async Task Parse_InternalMethod_SetsInternalAccessibility()
    {
        var result = TypeBuilder.Class("Test")
            .AddMethod(MethodBuilder.Parse("internal void Process()"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("internal void Process()");
    }

    [Test]
    public async Task Parse_RefParameter_SetsRefModifier()
    {
        var result = TypeBuilder.Class("Test")
            .AddMethod(MethodBuilder.Parse("public void Update(ref int value)"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("ref int value");
    }

    [Test]
    public async Task Parse_OutParameter_SetsOutModifier()
    {
        var result = TypeBuilder.Class("Test")
            .AddMethod(MethodBuilder.Parse("public bool TryGet(out string result)"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("out string result");
    }

    [Test]
    public async Task Parse_InParameter_SetsInModifier()
    {
        var result = TypeBuilder.Class("Test")
            .AddMethod(MethodBuilder.Parse("public void Process(in int data)"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("in int data");
    }

    [Test]
    public async Task Parse_ParamsParameter_SetsParamsModifier()
    {
        var result = TypeBuilder.Class("Test")
            .AddMethod(MethodBuilder.Parse("public void Log(params string[] messages)"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("params string[] messages");
    }

    [Test]
    public async Task Parse_DefaultParameterValue_SetsDefaultValue()
    {
        var result = TypeBuilder.Class("Test")
            .AddMethod(MethodBuilder.Parse("public void Process(int count = 10)"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("int count = 10");
    }

    [Test]
    public async Task Parse_DefaultCancellationToken_SetsDefaultValue()
    {
        var result = TypeBuilder.Class("Test")
            .AddMethod(MethodBuilder.Parse("public async Task RunAsync(CancellationToken ct = default)"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("CancellationToken ct = default");
    }

    [Test]
    public async Task Parse_GenericMethod_AddsTypeParameter()
    {
        var result = TypeBuilder.Class("Test")
            .AddMethod(MethodBuilder.Parse("public T Convert<T>(object value)"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public T Convert<T>(object value)");
    }

    [Test]
    public async Task Parse_GenericMethodWithMultipleTypeParams_AddsAllTypeParameters()
    {
        var result = TypeBuilder.Class("Test")
            .AddMethod(MethodBuilder.Parse("public TResult Map<TSource, TResult>(TSource source)"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public TResult Map<TSource, TResult>(TSource source)");
    }

    [Test]
    public async Task Parse_GenericMethodWithClassConstraint_AddsConstraint()
    {
        var result = TypeBuilder.Class("Test")
            .AddMethod(MethodBuilder.Parse("public T Create<T>() where T : class"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("where T : class");
    }

    [Test]
    public async Task Parse_GenericMethodWithStructConstraint_AddsConstraint()
    {
        var result = TypeBuilder.Class("Test")
            .AddMethod(MethodBuilder.Parse("public T Parse<T>(string value) where T : struct"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("where T : struct");
    }

    [Test]
    public async Task Parse_GenericMethodWithNewConstraint_AddsConstraint()
    {
        var result = TypeBuilder.Class("Test")
            .AddMethod(MethodBuilder.Parse("public T Create<T>() where T : new()"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("where T : new()");
    }

    [Test]
    public async Task Parse_GenericMethodWithTypeConstraint_AddsConstraint()
    {
        var result = TypeBuilder.Class("Test")
            .AddMethod(MethodBuilder.Parse("public T Create<T>() where T : IDisposable"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("where T : IDisposable");
    }

    [Test]
    public async Task Parse_ComplexSignature_ParsesAllComponents()
    {
        var result = TypeBuilder.Class("Test")
            .AddMethod(MethodBuilder.Parse(
                "public static async Task<TResult> ExecuteAsync<TInput, TResult>(TInput input, CancellationToken ct = default) where TResult : class"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public static async Task<TResult> ExecuteAsync<TInput, TResult>");
        await Assert.That(result.Code).Contains("TInput input");
        await Assert.That(result.Code).Contains("CancellationToken ct = default");
        await Assert.That(result.Code).Contains("where TResult : class");
    }

    [Test]
    public async Task Parse_CanAddBodyAfterParsing()
    {
        var result = TypeBuilder.Class("Test")
            .AddMethod(MethodBuilder.Parse("public string GetMessage()")
                .WithExpressionBody("\"Hello, World!\""))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("=> \"Hello, World!\"");
    }

    [Test]
    public async Task Parse_CanAddAttributesAfterParsing()
    {
        var result = TypeBuilder.Class("Test")
            .AddMethod(MethodBuilder.Parse("public void Process()")
                .WithAttribute("Obsolete"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("[Obsolete]");
    }

    [Test]
    public async Task Parse_InvalidSignature_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => 
            Task.FromResult(MethodBuilder.Parse("this is not valid c#")));
    }

    [Test]
    public async Task Parse_EmptySignature_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => 
            Task.FromResult(MethodBuilder.Parse("")));
    }

    [Test]
    public async Task Parse_NullSignature_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => 
            Task.FromResult(MethodBuilder.Parse(null!)));
    }

    [Test]
    public async Task Parse_NullableReturnType_PreservesNullability()
    {
        var builder = MethodBuilder.Parse("public string? GetOptionalName()");

        await Assert.That(builder.ReturnType).IsEqualTo("string?");
    }

    [Test]
    public async Task Parse_NullableParameter_PreservesNullability()
    {
        var result = TypeBuilder.Class("Test")
            .AddMethod(MethodBuilder.Parse("public void Process(string? input)"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("string? input");
    }

    [Test]
    public async Task Parse_GenericReturnType_PreservesFullType()
    {
        var builder = MethodBuilder.Parse("public Dictionary<string, List<int>> GetData()");

        await Assert.That(builder.ReturnType).IsEqualTo("Dictionary<string, List<int>>");
    }

    [Test]
    public async Task Parse_TrailingSemicolon_HandlesGracefully()
    {
        var builder = MethodBuilder.Parse("public void Execute();");

        await Assert.That(builder.Name).IsEqualTo("Execute");
    }

    [Test]
    public async Task Parse_ExtraWhitespace_HandlesGracefully()
    {
        var builder = MethodBuilder.Parse("  public   string   GetName  (  string   input  )  ");

        await Assert.That(builder.Name).IsEqualTo("GetName");
        await Assert.That(builder.ReturnType).IsEqualTo("string");
    }

    [Test]
    public async Task Parse_ExtensionMethod_SetsThisModifier()
    {
        var result = TypeBuilder.Class("Extensions").AsStatic()
            .AddMethod(MethodBuilder.Parse("public static string ToUpper(this string value)"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("this string value");
    }

    [Test]
    public async Task Parse_ExtensionMethod_ExposesExtensionTargetType()
    {
        var builder = MethodBuilder.Parse("public static IServiceCollection AddMyService(this IServiceCollection services)");

        await Assert.That(builder.ExtensionTargetType).IsEqualTo("IServiceCollection");
    }

    [Test]
    public async Task Parse_NonExtensionMethod_ReturnsNullExtensionTargetType()
    {
        var builder = MethodBuilder.Parse("public static void Execute(string input)");

        await Assert.That(builder.ExtensionTargetType).IsNull();
    }

    [Test]
    public async Task Parse_ExtensionMethodWithMultipleParams_OnlyFirstIsExtensionTarget()
    {
        var builder = MethodBuilder.Parse("public static string Format(this string template, params object[] args)");

        await Assert.That(builder.ExtensionTargetType).IsEqualTo("string");
    }
}
