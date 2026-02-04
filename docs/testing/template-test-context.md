# TemplateTestContext

Test Scriban template rendering with symbol queries and snapshot verification.

> **See also:** [RoslynTestBase](RoslynTestBase.md) | [Scriban Templates](../../Deepstaging.Roslyn.Scriban/README.md)

## Overview

`TemplateTestContext` combines symbol querying with template rendering for testing individual Scriban templates used in source generators.

```csharp
await RenderTemplateFrom<MyGenerator>(source)
    .Render("MyTemplate.scriban-cs", ctx => new 
    { 
        Name = ctx.RequireNamedType("Customer").Value.Name 
    })
    .ShouldRender()
    .VerifySnapshot();
```

---

## Entry Point

From `RoslynTestBase`:

```csharp
TemplateTestContext RenderTemplateFrom<TGenerator>(string source)
```

The `TGenerator` type is used to locate templates via `TemplateName.ForGenerator<TGenerator>()`.

---

## Rendering Templates

### With Symbol Query Builder

Build the template context from symbol queries:

```csharp
await RenderTemplateFrom<MyGenerator>(source)
    .Render("Customer.scriban-cs", ctx => 
    {
        var type = ctx.RequireNamedType("Customer");
        var props = ctx.Type("Customer").Properties().GetAll();
        
        return new CustomerModel
        {
            Name = type.Value.Name,
            Properties = props.Select(p => new PropModel(p.Name, p.Type.Name))
        };
    })
    .ShouldRender();
```

### With Direct Context Object

Pass a context object directly without symbol queries:

```csharp
var model = new CustomerModel 
{ 
    Name = "Customer", 
    Namespace = "MyApp" 
};

await RenderTemplateFrom<MyGenerator>(source)
    .Render("Customer.scriban-cs", model)
    .ShouldRender();
```

---

## Assertions

### Success Assertions

```csharp
// Assert render succeeds
await RenderTemplateFrom<MyGenerator>(source)
    .Render("Template.scriban-cs", model)
    .ShouldRender();

// Assert output contains specific content
await RenderTemplateFrom<MyGenerator>(source)
    .Render("Template.scriban-cs", model)
    .ShouldRender()
    .WithContent("public class Customer");
```

### Failure Assertions

```csharp
// Assert template fails to render
await RenderTemplateFrom<MyGenerator>(source)
    .Render("InvalidTemplate.scriban-cs", model)
    .ShouldFail();
```

### Snapshot Verification

```csharp
await RenderTemplateFrom<MyGenerator>(source)
    .Render("Template.scriban-cs", model)
    .ShouldRender()
    .VerifySnapshot();
```

Snapshots are stored next to your test file:
- Test: `MyTemplateTests.cs`
- Snapshot: `MyTemplateTests.RendersCustomerTemplate.verified.txt`

---

## Accessing Symbols

The context provides full access to `SymbolTestContext`:

```csharp
var templateContext = RenderTemplateFrom<MyGenerator>(source);

// Access symbols directly
var type = templateContext.Symbols.RequireNamedType("Customer");
var methods = templateContext.Symbols.Type("Customer").Methods().GetAll();

// Use in render
await templateContext
    .Render("Template.scriban-cs", _ => new { Type = type.Value })
    .ShouldRender();
```

---

## Common Patterns

### Testing Template Output Structure

```csharp
[Test]
public async Task CustomerTemplateGeneratesClass()
{
    var source = """
        namespace MyApp;
        public class Customer 
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }
        """;

    await RenderTemplateFrom<MyGenerator>(source)
        .Render("CustomerProxy.scriban-cs", ctx =>
        {
            var type = ctx.RequireNamedType("Customer");
            return new 
            {
                Namespace = type.Value.ContainingNamespace.ToDisplayString(),
                Name = type.Value.Name,
                Properties = ctx.Type("Customer").Properties().GetAll()
                    .Select(p => new { p.Name, Type = p.Type.ToDisplayString() })
            };
        })
        .ShouldRender()
        .WithContent("namespace MyApp")
        .WithContent("public class CustomerProxy")
        .VerifySnapshot();
}
```

### Testing Template Handles Edge Cases

```csharp
[Test]
public async Task HandlesEmptyProperties()
{
    var source = "public class Empty { }";

    await RenderTemplateFrom<MyGenerator>(source)
        .Render("PropertyList.scriban-cs", ctx => new 
        {
            Properties = ctx.Type("Empty").Properties().GetAll()
        })
        .ShouldRender()
        .WithContent("// No properties");
}
```

### Testing Multiple Templates

```csharp
[Test]
public async Task AllTemplatesRenderSuccessfully()
{
    var source = "public class Foo { public int Id { get; set; } }";
    var model = new { Name = "Foo", Id = 42 };

    var context = RenderTemplateFrom<MyGenerator>(source);

    await context.Render("Header.scriban-cs", model).ShouldRender();
    await context.Render("Body.scriban-cs", model).ShouldRender();
    await context.Render("Footer.scriban-cs", model).ShouldRender();
}
```

---

## Template Resolution

Templates are resolved using `TemplateName.ForGenerator<TGenerator>()`:

- Generator namespace: `MyApp.Generators`
- Template file: `Customer.scriban-cs`
- Full resource name: `MyApp.Generators.Templates.Customer.scriban-cs`

Templates must be embedded resources in the generator assembly.

---

## Why Use Template Testing?

| Scenario | Use |
|----------|-----|
| Test full generator output | `GenerateWith<T>` |
| Test individual template rendering | `RenderTemplateFrom<T>` |
| Test template with mock data | `RenderTemplateFrom<T>` with direct context |
| Test template edge cases | `RenderTemplateFrom<T>` |

Template testing is useful when:
- You want to test templates in isolation
- You need to verify specific template behavior
- You want faster tests (no full generator run)
- You're debugging template issues

---

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

You can use this code, modify it, and share it freely. But when you deploy it — internally or externally, as a service or within your company — you share your improvements back under the same license.

Why? We believe if you benefit from this code, the community should benefit from your improvements. That's the deal we think is fair.

**Personal research and experimentation? No obligations.** Go learn, explore, and build.

See [LICENSE](../../../LICENSE) for the full legal text.
