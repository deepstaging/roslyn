# Testing Best Practices

All Roslyn tests inherit from `RoslynTestBase`.

## Inherit from RoslynTestBase

```csharp
public class MyGeneratorTests : RoslynTestBase
{
    [Test]
    public async Task Generates_Properties()
    {
        const string source = """
            [AutoNotify]
            public partial class Person
            {
                private string _name;
            }
            """;

        await GenerateWith<AutoNotifyGenerator>(source)
            .ShouldGenerate()
            .WithFileNamed("Person.g.cs")
            .VerifySnapshot();
    }
}
```

## Use ModuleInitializer for References

```csharp
[ModuleInitializer]
public static void Init() => 
    ReferenceConfiguration.AddReferencesFromTypes(
        typeof(AutoNotifyAttribute));
```

## Generator Tests

### Basic Generation Test

```csharp
[Test]
public async Task Generates_NotifyPropertyChanged_Implementation()
{
    const string source = """
        using MyLibrary;

        [AutoNotify]
        public partial class Person
        {
            private string _name;
            private int _age;
        }
        """;

    await GenerateWith<AutoNotifyGenerator>(source)
        .ShouldGenerate()
        .WithFileNamed("Person.g.cs")
        .VerifySnapshot();
}
```

### Test No Generation When Conditions Not Met

```csharp
[Test]
public async Task Does_Not_Generate_When_No_Fields()
{
    const string source = """
        [AutoNotify]
        public partial class Empty { }
        """;

    await GenerateWith<AutoNotifyGenerator>(source)
        .ShouldNotGenerate();
}
```

### Test Multiple Generated Files

```csharp
[Test]
public async Task Generates_Multiple_Types()
{
    const string source = """
        [StrongId] public partial struct CustomerId;
        [StrongId] public partial struct OrderId;
        """;

    await GenerateWith<StrongIdGenerator>(source)
        .ShouldGenerate()
        .WithFileCount(2);
}
```

### Test Generated Content

```csharp
[Test]
public async Task Generated_Code_Contains_Property()
{
    const string source = """
        [AutoNotify]
        public partial class Person
        {
            private string _name;
        }
        """;

    await GenerateWith<AutoNotifyGenerator>(source)
        .ShouldGenerate()
        .WithFileContaining("public string Name");
}
```

### Test Generated Code Compiles

```csharp
[Test]
public async Task Generated_Code_Compiles_Successfully()
{
    const string source = """
        [AutoNotify]
        public partial class Person
        {
            private string _name;
        }
        """;

    await GenerateWith<AutoNotifyGenerator>(source)
        .ShouldGenerate()
        .CompilesSuccessfully();
}
```

### Test No Diagnostics Emitted

```csharp
[Test]
public async Task Generator_Emits_No_Errors()
{
    const string source = """
        [AutoNotify]
        public partial class Person
        {
            private string _name;
        }
        """;

    await GenerateWith<AutoNotifyGenerator>(source)
        .ShouldGenerate()
        .WithNoErrors();
}
```

## Analyzer Tests

### Basic Diagnostic Test

```csharp
[Test]
public async Task Reports_NonPartial_Class()
{
    const string source = """
        [AutoNotify]
        public class Person  // Missing 'partial'
        {
            private string _name;
        }
        """;

    await AnalyzeWith<AutoNotifyMustBePartialAnalyzer>(source)
        .ShouldReportDiagnostic("RK1002")
        .WithSeverity(DiagnosticSeverity.Error);
}
```

### Test No Diagnostic When Valid

```csharp
[Test]
public async Task Does_Not_Report_When_Partial()
{
    const string source = """
        [AutoNotify]
        public partial class Person
        {
            private string _name;
        }
        """;

    await AnalyzeWith<AutoNotifyMustBePartialAnalyzer>(source)
        .ShouldNotReportDiagnostic("RK1002");
}
```

### Test Diagnostic Message

```csharp
[Test]
public async Task Reports_Correct_Message()
{
    const string source = """
        [AutoNotify]
        public class Person { }
        """;

    await AnalyzeWith<AutoNotifyMustBePartialAnalyzer>(source)
        .ShouldReportDiagnostic("RK1002")
        .WithMessage("*must be partial*");  // Wildcard matching
}
```

### Test No Diagnostics At All

```csharp
[Test]
public async Task Valid_Code_Has_No_Diagnostics()
{
    const string source = """
        [AutoNotify]
        public partial class Person
        {
            private string _name;
        }
        """;

    await AnalyzeWith<AutoNotifyMustBePartialAnalyzer>(source)
        .ShouldHaveNoDiagnostics();
}
```

### Test Has Any Diagnostics

```csharp
[Test]
public async Task Invalid_Code_Has_Diagnostics()
{
    const string source = """
        [AutoNotify]
        public class Person { }
        """;

    await AnalyzeWith<AutoNotifyMustBePartialAnalyzer>(source)
        .ShouldHaveDiagnostics()
        .WithErrorCode("RK1002")
        .WithSeverity(DiagnosticSeverity.Error);
}
```

## CodeFix Tests

### Basic CodeFix Test

```csharp
[Test]
public async Task Fixes_NonPartial_Class()
{
    const string source = """
        [AutoNotify]
        public class Person { }
        """;

    const string expected = """
        [AutoNotify]
        public partial class Person { }
        """;

    await AnalyzeAndFixWith<AutoNotifyMustBePartialAnalyzer, MakePartialCodeFix>(source)
        .ForDiagnostic("RK1002")
        .ShouldProduce(expected);
}
```

### CodeFix Without Analyzer (Compiler Diagnostics)

```csharp
[Test]
public async Task Fixes_Compiler_Error()
{
    const string source = """
        public class Person
        {
            publc string Name { get; set; }  // Typo
        }
        """;

    await FixWith<TypoCodeFix>(source)
        .ForDiagnostic("CS0116")
        .ShouldProduce(expectedSource);
}
```

## Symbol Tests

Test projections and queries:

```csharp
[Test]
public async Task Can_Find_Public_Types()
{
    const string source = """
        public class PublicClass { }
        internal class InternalClass { }
        """;

    var types = SymbolsFor(source)
        .Types()
        .ThatArePublic()
        .GetAll();

    await Assert.That(types.Any(t => t.Value.Name == "PublicClass")).IsTrue();
    await Assert.That(types.Any(t => t.Value.Name == "InternalClass")).IsFalse();
}
```

### Query Specific Type

```csharp
[Test]
public async Task Can_Get_Named_Type()
{
    const string source = """
        public class Person
        {
            public string Name { get; set; }
        }
        """;

    var symbol = SymbolsFor(source).RequireNamedType("Person");

    await Assert.That(symbol.Value.Name).IsEqualTo("Person");
}
```

### Query Type Members

```csharp
[Test]
public async Task Can_Query_Properties()
{
    const string source = """
        public class Person
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }
        """;

    var type = SymbolsFor(source).RequireNamedType("Person");
    var properties = type.Properties().GetAll();

    await Assert.That(properties).HasCount(2);
}
```

### Query With Projections

```csharp
[Test]
public async Task Can_Use_Projections()
{
    const string source = """
        using System;

        [Obsolete("Use NewClass instead")]
        public class OldClass { }
        """;

    var type = SymbolsFor(source).RequireNamedType("OldClass");
    var attr = type.GetAttribute<ObsoleteAttribute>();
    var message = attr.ConstructorArg<string>(0).OrDefault("");

    await Assert.That(message).IsEqualTo("Use NewClass instead");
}
```

## Template Tests (Scriban)

Test template rendering with the `RenderTemplateFrom<TGenerator>` method:

```csharp
[Test]
public async Task Template_Renders_With_Symbols()
{
    const string source = """
        [AutoNotify]
        public partial class Person
        {
            private string _name;
        }
        """;

    await RenderTemplateFrom<AutoNotifyGenerator>(source)
        .Render("AutoNotify.scriban-cs", ctx =>
        {
            var type = ctx.RequireNamedType("Person");
            return new { TypeName = type.Value.Name };
        })
        .ShouldRender()
        .VerifySnapshot();
}
```

### Template With Direct Model

```csharp
[Test]
public async Task Template_Renders_With_Model()
{
    const string source = "class Dummy { }";

    var model = new
    {
        Namespace = "MyApp",
        TypeName = "Person",
        Properties = new[] { new { Name = "Name", Type = "string" } }
    };

    await RenderTemplateFrom<AutoNotifyGenerator>(source)
        .Render("AutoNotify.scriban-cs", model)
        .ShouldRender()
        .WithContent("public string Name");
}
```

### Template Should Fail

```csharp
[Test]
public async Task Template_Fails_With_Missing_Property()
{
    const string source = "class Dummy { }";
    var model = new { }; // Missing required properties

    await RenderTemplateFrom<AutoNotifyGenerator>(source)
        .Render("AutoNotify.scriban-cs", model)
        .ShouldFail();
}
```

## Common Test Patterns

### Testing Edge Cases

```csharp
[Test]
public async Task Handles_Nested_Types()
{
    const string source = """
        public class Outer
        {
            [AutoNotify]
            public partial class Inner
            {
                private string _value;
            }
        }
        """;

    await GenerateWith<AutoNotifyGenerator>(source)
        .ShouldGenerate()
        .WithFileNamed("Outer.Inner.g.cs");
}

[Test]
public async Task Handles_Generic_Types()
{
    const string source = """
        [AutoNotify]
        public partial class Container<T>
        {
            private T _value;
        }
        """;

    await GenerateWith<AutoNotifyGenerator>(source)
        .ShouldGenerate()
        .VerifySnapshot();
}
```

### Testing With Multiple Partial Declarations

```csharp
[Test]
public async Task Works_With_Multiple_Partial_Declarations()
{
    // Combine source files into a single compilation unit
    const string source = """
        public partial class Person
        {
            public string Name { get; set; }
        }

        [AutoNotify]
        public partial class Person
        {
            private int _age;
        }
        """;

    await GenerateWith<AutoNotifyGenerator>(source)
        .ShouldGenerate()
        .VerifySnapshot();
}
```

For more detailed testing documentation, see the [Testing section](../testing/roslyn-test-base.md).
