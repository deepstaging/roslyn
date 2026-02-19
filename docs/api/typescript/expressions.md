# Expressions

Composable expression references and factory methods for TypeScript code generation.

> **See also:** [Overview](index.md) | [Emit](emit.md) | [Types](types.md) | [Core Expressions](../expressions.md)

---

## TsExpressionRef

The expression-domain counterpart to `TsTypeRef`. A composable, immutable `readonly record struct` representing anything valid on the right side of an assignment.

```csharp
using Deepstaging.Roslyn.TypeScript;
```

A `TsTypeRef` crosses into expression domain via gateway methods (`New`, `Call`, `Member`). Once in expression domain, chaining continues through `TsExpressionRef` methods. This is a one-way gate — same as the C# layer.

### Method Calls and Member Access

```csharp
TsExpressionRef.From("items")
    .Call("filter", "x => x > 0")
    .Call("map", "x => x * 2")
    // items.filter(x => x > 0).map(x => x * 2)

TsExpressionRef.From("fetch").Invoke("url")
    // fetch(url)

TsExpressionRef.From("user").Member("name")
    // user.name

TsExpressionRef.From("items").Index("0")
    // items[0]
```

| Method | Description | Output |
|--------|------------|--------|
| `.Call(method, args...)` | Method call | `expr.method(args)` |
| `.Invoke(args...)` | Function invocation | `expr(args)` |
| `.Member(name)` | Property access | `expr.name` |
| `.Index(key)` | Computed access | `expr[key]` |

### Optional Chaining and Nullish Coalescing

TypeScript's `?.` and `??` operators for safe navigation.

```csharp
TsExpressionRef.From("user")
    .OptionalChain("address")
    .OptionalChain("city")
    .NullishCoalesce("\"unknown\"")
    // user?.address?.city ?? "unknown"

TsExpressionRef.From("handler")
    .OptionalCall("process", "data")
    // handler?.process(data)

TsExpressionRef.From("items")
    .OptionalIndex("0")
    // items?.[0]
```

| Method | Description | Output |
|--------|------------|--------|
| `.OptionalChain(name)` | Optional member access | `expr?.name` |
| `.OptionalCall(method, args...)` | Optional method call | `expr?.method(args)` |
| `.OptionalIndex(index)` | Optional element access | `expr?.[index]` |
| `.NullishCoalesce(fallback)` | Nullish coalescing | `expr ?? fallback` |
| `.NonNullAssertion()` | Non-null assertion | `expr!` |

### Type Assertions and Checks

```csharp
TsExpressionRef.From("value").As("string")          // value as string
TsExpressionRef.From("config").Satisfies("Config")  // config satisfies Config
TsExpressionRef.From("value").TypeOf()               // typeof value
TsExpressionRef.From("err").InstanceOf("Error")      // err instanceof Error
```

| Method | Description | Output |
|--------|------------|--------|
| `.As(type)` | Type assertion | `expr as Type` |
| `.Satisfies(type)` | Satisfies check | `expr satisfies Type` |
| `.TypeOf()` | typeof operator | `typeof expr` |
| `.InstanceOf(type)` | instanceof check | `expr instanceof Type` |

### Async, Spread, and Template Literals

```csharp
TsExpressionRef.From("fetch").Invoke("url").Await()
    // await fetch(url)

TsExpressionRef.From("args").Spread()
    // ...args

TsExpressionRef.From("name").TemplateLiteral("Hello, ", "!")
    // `Hello, ${name}!`
```

### Logical Operators

```csharp
TsExpressionRef.From("a").StrictEquals("b")       // a === b
TsExpressionRef.From("a").StrictNotEquals("b")    // a !== b
TsExpressionRef.From("a").And("b")                // a && b
TsExpressionRef.From("a").Or("b")                 // a || b
TsExpressionRef.From("valid").Not()               // !valid
TsExpressionRef.From("expr").Parenthesize()       // (expr)
```

### Implicit Conversions

`TsExpressionRef` converts implicitly to and from `string`, and from `TsTypeRef` (a type name is a valid expression):

```csharp
TsExpressionRef e = "myVar";                // implicit from string
string s = e.Call("toString");               // implicit to string
TsExpressionRef t = TsTypeRef.From("Math"); // implicit from TsTypeRef
```

---

## Expression Factories

Static classes that produce `TsExpressionRef` for common TypeScript patterns. These mirror `TaskExpression`, `CollectionExpression`, etc. from the C# layer.

```csharp
using Deepstaging.Roslyn.TypeScript.Expressions;
```

### TsPromiseExpression

```csharp
TsPromiseExpression.Resolve("value")            // Promise.resolve(value)
TsPromiseExpression.Reject("reason")            // Promise.reject(reason)
TsPromiseExpression.All(a, b, c)                // Promise.all([a, b, c])
TsPromiseExpression.AllSettled(a, b)             // Promise.allSettled([a, b])
TsPromiseExpression.Race(a, b)                   // Promise.race([a, b])
TsPromiseExpression.Any(a, b)                    // Promise.any([a, b])
TsPromiseExpression.New("(resolve, reject) => { ... }")
                                                  // new Promise((resolve, reject) => { ... })
```

### TsJsonExpression

```csharp
TsJsonExpression.Stringify("data")               // JSON.stringify(data)
TsJsonExpression.Parse("text")                    // JSON.parse(text)
```

### TsFetchExpression

```csharp
TsFetchExpression.Get("url")                     // fetch(url)
TsFetchExpression.Post("url", "body")            // fetch(url, { method: 'POST', body: ... })
```

### TsConsoleExpression

```csharp
TsConsoleExpression.Log("msg")                   // console.log(msg)
TsConsoleExpression.Error("msg")                 // console.error(msg)
TsConsoleExpression.Warn("msg")                  // console.warn(msg)
```

### TsArrayExpression

```csharp
TsArrayExpression.From("iterable")              // Array.from(iterable)
TsArrayExpression.Of(a, b, c)                   // [a, b, c]
TsArrayExpression.IsArray("value")              // Array.isArray(value)
```

### TsObjectExpression

```csharp
TsObjectExpression.Keys("obj")                  // Object.keys(obj)
TsObjectExpression.Values("obj")                // Object.values(obj)
TsObjectExpression.Entries("obj")               // Object.entries(obj)
TsObjectExpression.Assign("target", "source")   // Object.assign(target, source)
TsObjectExpression.Freeze("obj")                // Object.freeze(obj)
```

### TsErrorExpression

```csharp
TsErrorExpression.New("message")                 // new Error(message)
TsErrorExpression.NewType("TypeError", "msg")    // new TypeError(msg)
```

---

## Composing Types and Expressions

A typical source generator pattern — analyze a Roslyn symbol, then build TypeScript output using both type refs and expressions:

```csharp
using Deepstaging.Roslyn.TypeScript;
using Deepstaging.Roslyn.TypeScript.Emit;
using Deepstaging.Roslyn.TypeScript.Types;
using Deepstaging.Roslyn.TypeScript.Expressions;

// Build type refs for the service
var promiseType = new TsPromiseTypeRef("User[]");
var mapType = new TsMapTypeRef("string", "User");

// Build the service class
var result = TsTypeBuilder.Class("UserService")
    .Exported()
    .AddField("cache", mapType, f => f
        .WithAccessibility(TsAccessibility.Private)
        .AsReadonly()
        .WithInitializer("new Map()"))
    .AddMethod("fetchUsers", m => m
        .Async()
        .WithReturnType(promiseType)
        .WithBody(b => b
            .AddConst("response", TsExpressionRef.From("fetch").Invoke("'/api/users'").Await())
            .AddConst("data", TsExpressionRef.From("response").Call("json").Await())
            .AddReturn("data")))
    .AddMethod("logCache", m => m
        .WithReturnType("void")
        .WithBody(b => b
            .AddStatement(TsConsoleExpression.Log("this.cache"))))
    .Emit();
```
