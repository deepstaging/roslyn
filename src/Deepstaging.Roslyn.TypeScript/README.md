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
TsTypeRef.String, TsTypeRef.Number, TsTypeRef.Boolean
var custom = TsTypeRef.From("MyType");

// Generics, unions, intersections
var generic = TsTypeRef.From("Map").Generic("string", "number");
var union = TsTypeRef.String.Union(TsTypeRef.Number);
var intersection = typeA.Intersection(typeB);

// .NET Type → TypeScript mapping
TsTypeRef.From(typeof(string));           // string
TsTypeRef.From(typeof(Dictionary<,>));    // Record<string, unknown>
TsTypeRef.From(typeof(Task<int>));        // Promise<number>

// Specialized types
TsPromiseType.Of("string");              // Promise<string>
TsArrayType.Of("number");                // number[]
TsRecordType.Of("string", "unknown");    // Record<string, unknown>
```

## Expressions

```csharp
// Compose TypeScript expressions
TsExpressionRef.From("fetch")
    .Call("'/api/users'")
    .Await()
    .Member("json")
    .Call();
// → (await fetch('/api/users')).json()

// Factory methods
TsConsoleExpression.Log("'hello'");       // console.log('hello')
TsFetchExpression.Get("'/api/data'");     // await fetch('/api/data')
TsJsonExpression.Parse("raw");            // JSON.parse(raw)
```

## Emit Builders

```csharp
// Classes, interfaces, type aliases, enums
TsTypeBuilder.Class("UserService").Exported()
    .AddConstructor(c => c.AddParameter("http", "HttpClient", p => p.AsPrivate()))
    .AddMethod("getUsers", m => m
        .Async()
        .WithReturnType(TsPromiseType.Of(TsArrayType.Of("User")))
        .WithBody(b => b
            .Return(TsExpressionRef.From("this.http.get").Call("'/api/users'").Await())));
```

## Package Structure

- **Types/** — `TsTypeRef`, specialized types (Promise, Array, Map, Record, etc.), utility types
- **Expressions/** — Expression factories (Console, Fetch, JSON, Array, Object, Error, Promise)
- **Emit/** — Fluent builders (Type, Method, Property, Field, Constructor, Parameter, Body)
