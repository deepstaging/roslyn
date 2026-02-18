# TemplateTestContext

Test Scriban template rendering with symbol queries and snapshot verification.

## Creating

```csharp
await RenderTemplateFrom<MyGenerator>(source)
    .Render("MyTemplate.scriban-cs", ctx => new { Name = ctx.RequireNamedType("Foo").Value.Name })
    .ShouldRender()
    .VerifySnapshot();
```

## Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `Symbols` | `SymbolTestContext` | Access the underlying symbol test context |
| `Render(fileName, contextBuilder)` | `TemplateRenderContext` | Render template using a builder function |
| `Render(fileName, context)` | `TemplateRenderContext` | Render template with a pre-built context object |

The `contextBuilder` receives a `SymbolTestContext`, letting you extract symbols from the compiled source to build the template model.

## TemplateRenderContext

| Method | Returns | Description |
|--------|---------|-------------|
| `ShouldRender()` | `TemplateRenderAssertions` | Assert rendering succeeded |
| `ShouldFail()` | `Task` | Assert rendering failed |

## TemplateRenderAssertions

| Method | Returns | Description |
|--------|---------|-------------|
| `WithContent(text)` | `TemplateRenderAssertions` | Assert output contains text |
| `VerifySnapshot(sourceFile)` | `Task` | Snapshot test with Verify |

## Example

```csharp
[Test]
public async Task Renders_class_template()
{
    const string source = """
        namespace MyApp;
        public partial class Customer { }
        """;

    await RenderTemplateFrom<MyGenerator>(source)
        .Render("Class.scriban-cs", ctx =>
        {
            var type = ctx.RequireNamedType("Customer");
            return new
            {
                Name = type.Value.Name,
                Namespace = type.Value.ContainingNamespace.ToDisplayString()
            };
        })
        .ShouldRender()
        .WithContent("partial class Customer")
        .VerifySnapshot();
}

[Test]
public async Task Reports_error_for_invalid_template()
{
    const string source = "public class Foo { }";

    await RenderTemplateFrom<BrokenGenerator>(source)
        .Render("Bad.scriban-cs", new { })
        .ShouldFail();
}
```
