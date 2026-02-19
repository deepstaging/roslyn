# Emit

Fluent builders for generating TypeScript type declarations, members, and bodies.

> **See also:** [Overview](index.md) | [Types](types.md) | [Expressions](expressions.md)

---

## TsTypeBuilder

The main entry point. Create TypeScript type declarations.

### Factory Methods

| Method | Description |
|--------|-------------|
| `Class(string name)` | Create a class |
| `Interface(string name)` | Create an interface |
| `TypeAlias(string name, string definition)` | Create a type alias (`type X = ...`) |
| `Enum(string name)` | Create an enum |
| `ConstEnum(string name)` | Create a const enum |

### Modifiers

```csharp
TsTypeBuilder
    .Class("MyClass")
    .Exported()          // export class MyClass
    .DefaultExported()   // export default class MyClass
    .AsAbstract()        // abstract class MyClass
    .AsDeclare()         // declare class MyClass
    .WithJsDoc("Service for managing users.")
    .WithDecorator("@Injectable()")
```

### Type Parameters

```csharp
// Simple type parameter
TsTypeBuilder.Class("Repository")
    .AddTypeParameter("T")

// Constrained type parameter
TsTypeBuilder.Class("Repository")
    .AddTypeParameter("T", "BaseEntity")  // T extends BaseEntity
```

### Inheritance

```csharp
TsTypeBuilder.Class("UserService")
    .Extends("BaseService")       // extends BaseService
    .Implements("IUserService")    // implements IUserService
    .Implements("IDisposable")     // implements IUserService, IDisposable
```

### Adding Members

```csharp
builder
    // Fields (class only)
    .AddField("count", "number", f => f
        .WithAccessibility(TsAccessibility.Private)
        .AsReadonly()
        .WithInitializer("0"))

    // Properties (interface or class)
    .AddProperty("name", "string", p => p
        .AsReadonly()
        .AsOptional())

    // Methods
    .AddMethod("greet", m => m
        .WithReturnType("string")
        .WithExpressionBody("`Hello, ${this.name}`"))

    // Constructors
    .AddConstructor(c => c
        .AddParameter("name", "string")
        .WithBody(b => b.AddStatement("this.name = name")))

    // Index signatures
    .AddIndexSignature("key", "string", "unknown")

    // Enum members (enum/const enum only)
    .AddEnumMember("Active", "'active'")
    .AddEnumMember("Inactive")  // auto-assigned value

    // Imports
    .AddImport("import { User } from './models'")
```

### Emit

```csharp
TsOptionalEmit result = builder.Emit();
TsOptionalEmit result = builder.Emit(new TsEmitOptions { FormatOutput = true });
```

---

## Member Builders

### TsFieldBuilder

Class fields with all TypeScript modifiers.

```csharp
TsFieldBuilder.For("name", "string")                              // name: string;
TsFieldBuilder.For("id", "number").AsReadonly()                    // readonly id: number;
TsFieldBuilder.For("_count", "number")
    .WithAccessibility(TsAccessibility.Private)
    .WithInitializer("0")                                          // private _count: number = 0;
TsFieldBuilder.For("secret", "string").AsEsPrivate()               // #secret: string;
TsFieldBuilder.For("cache", "Map<string, unknown>").AsDeclare()    // declare cache: Map<string, unknown>;
```

| Method | Description |
|--------|-------------|
| `WithAccessibility(TsAccessibility)` | `public`, `private`, `protected` |
| `AsReadonly()` | `readonly` modifier |
| `AsStatic()` | `static` modifier |
| `AsOptional()` | `name?: Type` |
| `AsEsPrivate()` | `#name` (ES private field) |
| `AsDeclare()` | `declare` modifier |
| `AsAbstract()` | `abstract` modifier |
| `AsOverride()` | `override` modifier |
| `WithInitializer(string)` | `= expression` |

### TsPropertyBuilder

Interface and class properties with optional getter/setter bodies.

```csharp
TsPropertyBuilder.For("name", "string")                           // name: string;
TsPropertyBuilder.For("name", "string").AsReadonly()               // readonly name: string;
TsPropertyBuilder.For("email", "string").AsOptional()              // email?: string;

// Getter
TsPropertyBuilder.For("fullName", "string")
    .WithGetter("this.first + ' ' + this.last")
    // get fullName(): string { return this.first + ' ' + this.last; }

// Getter + Setter
TsPropertyBuilder.For("name", "string")
    .WithGetter(b => b.AddReturn("this._name"))
    .WithSetter(b => b.AddStatement("this._name = value"))
```

### TsMethodBuilder

Methods with async, generators, overloads, and decorators.

```csharp
TsMethodBuilder.For("fetchData")
    .Async()
    .AddParameter("url", "string")
    .WithReturnType("Promise<Response>")
    .WithBody(b => b.AddReturn("fetch(url)"))
    // async fetchData(url: string): Promise<Response> { return fetch(url); }

TsMethodBuilder.For("process")
    .AsAbstract()
    .AddParameter("data", "Buffer")
    .WithReturnType("void")
    // abstract process(data: Buffer): void;

TsMethodBuilder.For("generate")
    .AsGenerator()
    .WithReturnType("Generator<number>")
    .WithBody(b => b
        .AddStatement("yield 1")
        .AddStatement("yield 2"))
    // *generate(): Generator<number> { yield 1; yield 2; }
```

| Method | Description |
|--------|-------------|
| `WithReturnType(string)` | Return type annotation |
| `WithAccessibility(TsAccessibility)` | `public`, `private`, `protected` |
| `AsStatic()` | `static` modifier |
| `Async()` | `async` modifier |
| `AsAbstract()` | `abstract` — no body emitted |
| `AsOverride()` | `override` modifier |
| `AsGenerator()` | `*name()` generator syntax |
| `AsOptional()` | `name?()` (interface members) |
| `AddTypeParameter(string)` | Generic type parameter |
| `AddParameter(name, type)` | Add parameter |
| `AddParameter(name, type, configure)` | Add configured parameter |
| `WithBody(configure)` | Block body via `TsBodyBuilder` |
| `WithExpressionBody(string)` | Single expression (wraps in `return`) |
| `AddOverloadSignature(string)` | Raw overload signature |
| `WithDecorator(string)` | Decorator (e.g., `@Get('/')`) |

### TsConstructorBuilder

Constructors with parameter properties and `super()` calls.

```csharp
// Simple constructor
TsConstructorBuilder.Create()
    .AddParameter("name", "string")
    .WithBody(b => b.AddStatement("this.name = name"))
    // constructor(name: string) { this.name = name; }

// Parameter properties (TypeScript shorthand)
TsConstructorBuilder.Create()
    .AddParameter("x", "number", p => p
        .AsParameterProperty(TsAccessibility.Public)
        .AsReadonlyParameterProperty())
    .AddParameter("y", "number", p => p
        .AsParameterProperty(TsAccessibility.Public)
        .AsReadonlyParameterProperty())
    // constructor(public readonly x: number, public readonly y: number) { }

// Super call
TsConstructorBuilder.Create()
    .AddParameter("config", "Config")
    .CallsSuper("config")
    .WithBody(b => b.AddStatement("this.init()"))
    // constructor(config: Config) { super(config); this.init(); }
```

### TsParameterBuilder

```csharp
TsParameterBuilder.For("name", "string")                               // name: string
TsParameterBuilder.For("name", "string").AsOptional()                   // name?: string
TsParameterBuilder.For("name", "string").WithDefaultValue("\"world\"")  // name: string = "world"
TsParameterBuilder.For("args", "string[]").AsRest()                     // ...args: string[]
TsParameterBuilder.For("name", "string")
    .AsParameterProperty(TsAccessibility.Public)                        // public name: string
TsParameterBuilder.For("name", "string")
    .AsParameterProperty(TsAccessibility.Private)
    .AsReadonlyParameterProperty()                                      // private readonly name: string
```

---

## TsBodyBuilder

Statement accumulation for method and property bodies.

### Statements

```csharp
TsBodyBuilder.Empty()
    .AddStatement("console.log('hello')")        // console.log('hello');
    .AddStatements("const a = 1", "const b = 2") // multiple at once
    .AddReturn("a + b")                           // return a + b;
    .AddReturn()                                  // return;
    .AddThrow("new Error('fail')")                // throw new Error('fail');
```

### Variable Declarations

```csharp
body
    .AddConst("name", "user.name")                           // const name = user.name;
    .AddConst("items", TsTypeRef.From("string[]"), "[]")     // const items: string[] = [];
    .AddLet("count", "0")                                    // let count = 0;
    .AddLet("total", TsTypeRef.From("number"), "0")          // let total: number = 0;
```

### Control Flow

```csharp
body
    .AddIf("user != null", b => b
        .AddStatement("process(user)"))

    .AddIfElse("isValid",
        then => then.AddReturn("data"),
        @else => @else.AddThrow("new Error('invalid')"))

    .AddForOf("item", "items", b => b
        .AddStatement("process(item)"))

    .AddForIn("key", "config", b => b
        .AddStatement("console.log(key)"))

    .AddTryCatch(
        try_ => try_.AddStatement("await fetch(url)"),
        "err",
        catch_ => catch_.AddStatement("console.error(err)"))

    .AddTryCatchFinally(
        try_ => try_.AddStatement("await fetch(url)"),
        "err",
        catch_ => catch_.AddStatement("console.error(err)"),
        finally_ => finally_.AddStatement("cleanup()"))
```

---

## TsOptionalEmit

The result of a TypeScript emit operation. Same pattern as the C# layer's `OptionalEmit` → `ValidEmit` projection.

```csharp
var result = builder.Emit();

// Option 1: Try/validate pattern
if (result.TryValidate(out var valid))
{
    string code = valid.Code;
}

// Option 2: Throw on failure
var valid = result.ValidateOrThrow();
string code = valid.Code;

// Option 3: Check success and diagnostics
if (result.Success)
{
    string code = result.Code!;
}
else
{
    foreach (var diagnostic in result.Diagnostics)
        Console.WriteLine(diagnostic);
}
```

---

## TsEmitOptions

Controls formatting, validation, and output style.

```csharp
var options = new TsEmitOptions
{
    Indentation = "  ",              // default: 2 spaces
    EndOfLine = "\n",                // default: LF
    HeaderComment = "// <auto-generated/>",  // default; set null to omit
    UseSemicolons = true,            // default: true
    UseTrailingCommas = true,        // default: true
    SingleQuotes = true,             // default: true

    // Validation (test-time only — requires tsc)
    ValidationLevel = TsValidationLevel.Syntax,
    TscPath = "node_modules/.bin/tsc",  // or null to auto-resolve

    // Formatting (test-time only — requires dprint or prettier)
    FormatOutput = true,
};
```

!!! tip "Validation and formatting are test-time tools"
    Set `ValidationLevel` and `FormatOutput` in your test code, not in your source generator. The defaults (`None` / `false`) ensure zero overhead in production.

---

## Complete Examples

### Interface

```csharp
var result = TsTypeBuilder.Interface("User")
    .Exported()
    .AddProperty("id", "number", p => p.AsReadonly())
    .AddProperty("name", "string", p => p)
    .AddProperty("email", "string", p => p.AsOptional())
    .Emit();
```

Produces:

```typescript
// <auto-generated/>

export interface User {
  readonly id: number;
  name: string;
  email?: string;
}
```

### Class with Inheritance

```csharp
var result = TsTypeBuilder.Class("UserService")
    .Exported()
    .Extends("BaseService")
    .Implements("IUserService")
    .AddField("users", "Map<string, User>", f => f
        .WithAccessibility(TsAccessibility.Private)
        .AsReadonly()
        .WithInitializer("new Map()"))
    .AddConstructor(c => c
        .AddParameter("config", "ServiceConfig")
        .CallsSuper("config")
        .WithBody(b => b.AddStatement("this.users = new Map()")))
    .AddMethod("getUser", m => m
        .Async()
        .AddParameter("id", "string")
        .WithReturnType("Promise<User | undefined>")
        .WithBody(b => b.AddReturn("this.users.get(id)")))
    .Emit();
```

### Type Alias

```csharp
var result = TsTypeBuilder.TypeAlias("UserId", "string | number")
    .Exported()
    .Emit();
```

### Enum

```csharp
var result = TsTypeBuilder.Enum("Status")
    .Exported()
    .AddEnumMember("Active", "'active'")
    .AddEnumMember("Inactive", "'inactive'")
    .AddEnumMember("Pending", "'pending'")
    .Emit();
```

### Abstract Generic Class

```csharp
var result = TsTypeBuilder.Class("Repository")
    .Exported()
    .AsAbstract()
    .AddTypeParameter("T")
    .AddMethod("findById", m => m
        .AsAbstract()
        .AddParameter("id", "string")
        .WithReturnType("Promise<T | null>"))
    .AddMethod("save", m => m
        .AsAbstract()
        .AddParameter("entity", "T")
        .WithReturnType("Promise<void>"))
    .Emit();
```

### Decorated Controller (NestJS-style)

```csharp
var result = TsTypeBuilder.Class("UserController")
    .Exported()
    .WithDecorator("@Controller('/users')")
    .AddMethod("getAll", m => m
        .WithDecorator("@Get('/')")
        .Async()
        .WithReturnType("Promise<User[]>")
        .WithBody(b => b.AddReturn("this.userService.findAll()")))
    .Emit();
```

### Constructor Parameter Properties

```csharp
var result = TsTypeBuilder.Class("Point")
    .Exported()
    .AddConstructor(c => c
        .AddParameter("x", "number", p => p
            .AsParameterProperty(TsAccessibility.Public)
            .AsReadonlyParameterProperty())
        .AddParameter("y", "number", p => p
            .AsParameterProperty(TsAccessibility.Public)
            .AsReadonlyParameterProperty()))
    .AddMethod("distanceTo", m => m
        .AddParameter("other", "Point")
        .WithReturnType("number")
        .WithBody(b => b
            .AddConst("dx", "this.x - other.x")
            .AddConst("dy", "this.y - other.y")
            .AddReturn("Math.sqrt(dx * dx + dy * dy)")))
    .Emit();
```

Produces:

```typescript
export class Point {
  constructor(public readonly x: number, public readonly y: number) {}

  distanceTo(other: Point): number {
    const dx = this.x - other.x;
    const dy = this.y - other.y;
    return Math.sqrt(dx * dx + dy * dy);
  }
}
```
