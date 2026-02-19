# Deepstaging.Roslyn.TypeScript

Fluent TypeScript code emitter for .NET source generators and code-gen pipelines
built with [Deepstaging.Roslyn](https://github.com/deepstaging/roslyn).

📚 **[Full Documentation](https://deepstaging.github.io/roslyn/api/typescript/)**

## Usage

```csharp
using Deepstaging.Roslyn.TypeScript;
using Deepstaging.Roslyn.TypeScript.Emit;
using Deepstaging.Roslyn.TypeScript.Types;

// Build a TypeScript interface from C# source generators
var result = TsTypeBuilder.Interface("UserDto")
    .Exported()
    .AddProperty("name", "string", p => p)
    .AddProperty("age", "number", p => p)
    .AddProperty("email", "string", p => p.AsOptional())
    .Emit();

// result.Code:
// export interface UserDto {
//   name: string;
//   age: number;
//   email?: string;
// }
```

## Types

Type references model the TypeScript type system:

```csharp
// Primitives and custom types
var str = TsTypeRef.From("string");
var custom = TsTypeRef.From("MyType");

// Generics, unions, intersections
var generic = TsTypeRef.From("Map").Of("string", "number");
var union = TsTypeRef.Union("string", "number");
var intersection = TsTypeRef.Intersection("A", "B");

// .NET Type → TypeScript mapping
TsTypeRef.From(typeof(string));           // string
TsTypeRef.From(typeof(Dictionary<,>));    // Record<string, unknown>
TsTypeRef.From(typeof(Task<int>));        // Promise<number>

// Specialized types
new TsPromiseTypeRef("string");           // Promise<string>
new TsArrayTypeRef("number");             // number[]
new TsRecordTypeRef("string", "unknown"); // Record<string, unknown>
```

## Expressions

```csharp
// Compose TypeScript expressions
TsExpressionRef.From("fetch")
    .Invoke("'/api/users'")
    .Await()
    .Call("json")
// → (await fetch('/api/users')).json()

// Factory methods
TsConsoleExpression.Log("'hello'");       // console.log('hello')
TsFetchExpression.Get("'/api/data'");     // fetch('/api/data')
TsJsonExpression.Parse("raw");            // JSON.parse(raw)
```

## Emit Builders

```csharp
// Classes, interfaces, type aliases, enums
TsTypeBuilder.Class("UserService").Exported()
    .AddConstructor(c => c
        .AddParameter("http", "HttpClient", p => p.AsParameterProperty(TsAccessibility.Private)))
    .AddMethod("getUsers", m => m
        .Async()
        .WithReturnType(new TsPromiseTypeRef(new TsArrayTypeRef("User")))
        .WithBody(b => b
            .AddReturn("await this.http.get('/api/users')")));
```

## Package Structure

- **Types/** — `TsTypeRef`, specialized types (Promise, Array, Map, Record, etc.), utility types
- **Expressions/** — Expression factories (Console, Fetch, JSON, Array, Object, Error, Promise)
- **Emit/** — Fluent builders (Type, Method, Property, Field, Constructor, Parameter, Body)
