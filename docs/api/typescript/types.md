# Types

Composable type references for TypeScript, created through `TsTypeRef` and specialized type-safe wrappers.

> **See also:** [Overview](index.md) | [Emit](emit.md) | [Expressions](expressions.md) | [Core TypeRef](../emit/type-ref.md)

---

## TsTypeRef

The TypeScript counterpart to `TypeRef`. A composable, immutable `readonly record struct` with implicit `string` conversions. Every method returns a new instance.

```csharp
using Deepstaging.Roslyn.TypeScript;
```

### Basics

```csharp
TsTypeRef.From("string")                        // string
TsTypeRef.From("Map").Of("string", "number")    // Map<string, number>
```

### Type Operators

```csharp
// Union and intersection
TsTypeRef.Union("string", "number")             // string | number
TsTypeRef.Intersection("A", "B")                // A & B

// Tuples
TsTypeRef.Tuple("string", "number")             // [string, number]
TsTypeRef.NamedTuple(
    (TsTypeRef.From("string"), "name"),
    (TsTypeRef.From("number"), "age"))           // [name: string, age: number]

// Literals
TsTypeRef.Literal("success")                    // "success"
TsTypeRef.NumericLiteral("42")                   // 42
TsTypeRef.TemplateLiteral("`prefix-${string}`")  // `prefix-${string}`
```

### Modifiers

| Method | Output |
|--------|--------|
| `.Array()` | `string[]` |
| `.Nullable()` | `string \| null` |
| `.Optional()` | `string \| undefined` |
| `.NullableOptional()` | `string \| null \| undefined` |
| `.Readonly()` | `readonly string` |
| `.Parenthesize()` | `(string \| number)` |
| `.KeyOf()` | `keyof User` |
| `.TypeOf()` | `typeof config` |

```csharp
TsTypeRef.From("string").Array()                 // string[]
TsTypeRef.From("string").Nullable()              // string | null
TsTypeRef.From("string").Optional()              // string | undefined
TsTypeRef.From("string").NullableOptional()      // string | null | undefined
TsTypeRef.From("string").Readonly()              // readonly string
TsTypeRef.From("string | number").Parenthesize() // (string | number)
TsTypeRef.From("User").KeyOf()                   // keyof User
TsTypeRef.From("config").TypeOf()                // typeof config
```

### Expression Gateways

Cross from the type domain into expressions (one-way gate — same as C# layer):

```csharp
TsTypeRef.From("User").New("name", "email")      // new User(name, email)
TsTypeRef.From("Math").Member("PI")               // Math.PI
TsTypeRef.From("Array").Call("from", "iterable")   // Array.from(iterable)
```

These return `TsExpressionRef` — once you cross into the expression domain, you stay there.

### Implicit Conversions

`TsTypeRef` converts implicitly to and from `string`, so you can use raw strings anywhere a `TsTypeRef` is expected and vice versa:

```csharp
TsTypeRef t = "string";      // implicit from string
string s = t;                  // implicit to string
builder.AddField("x", t, f => f);
builder.AddField("y", "number", f => f);  // string works too
```

---

## Specialized Type Refs

Type-safe wrappers that carry constituent generic arguments, just like `TaskTypeRef` and `ListTypeRef` in the C# layer. All live in `Deepstaging.Roslyn.TypeScript.Types`.

```csharp
using Deepstaging.Roslyn.TypeScript.Types;
```

### Built-in Types

| Type Ref | TypeScript | Carried Properties |
|----------|-----------|-------------------|
| `TsPromiseTypeRef("string")` | `Promise<string>` | `ResultType` |
| `TsArrayTypeRef("User")` | `User[]` | `ElementType` |
| `TsMapTypeRef("string", "User")` | `Map<string, User>` | `KeyType`, `ValueType` |
| `TsSetTypeRef("string")` | `Set<string>` | `ElementType` |
| `TsRecordTypeRef("string", "number")` | `Record<string, number>` | `KeyType`, `ValueType` |
| `TsFunctionTypeRef(params, returnType)` | `(a: A) => R` | `Parameters`, `ReturnType` |
| `TsReadonlyArrayTypeRef("User")` | `readonly User[]` | `ElementType` |
| `TsNullableTypeRef("string")` | `string \| null` | `InnerType` |

#### Usage

```csharp
var promise = new TsPromiseTypeRef("User");

// Use directly in builders — implicit conversion to string
builder.AddMethod("fetch", m => m
    .Async()
    .WithReturnType(promise)       // Promise<User>
    .AddParameter("id", "string"));

// Access constituent types for downstream use
TsTypeRef resultType = promise.ResultType;  // "User"
```

#### TsFunctionTypeRef

The function type ref requires explicit parameters:

```csharp
var fn = new TsFunctionTypeRef(
    ImmutableArray.Create(
        (TsTypeRef.From("string"), "name"),
        (TsTypeRef.From("number"), "age")),
    TsTypeRef.From("boolean"));
// (name: string, age: number) => boolean

builder.AddField("validator", fn, f => f);
```

---

### Utility Types

TypeScript's built-in utility types, each as a type-safe wrapper.

| Type Ref | TypeScript | Purpose |
|----------|-----------|---------|
| `TsPartialTypeRef("User")` | `Partial<User>` | All properties optional |
| `TsRequiredTypeRef("User")` | `Required<User>` | All properties required |
| `TsReadonlyTypeRef("User")` | `Readonly<User>` | All properties readonly |
| `TsPickTypeRef("User", "'name' \| 'email'")` | `Pick<User, 'name' \| 'email'>` | Pick specific properties |
| `TsOmitTypeRef("User", "'password'")` | `Omit<User, 'password'>` | Omit specific properties |
| `TsExcludeTypeRef("Status", "'deleted'")` | `Exclude<Status, 'deleted'>` | Exclude from union |
| `TsExtractTypeRef("Status", "'active'")` | `Extract<Status, 'active'>` | Extract from union |
| `TsNonNullableTypeRef("string \| null")` | `NonNullable<string \| null>` | Remove null/undefined |
| `TsReturnTypeRef("typeof fn")` | `ReturnType<typeof fn>` | Function return type |
| `TsParametersTypeRef("typeof fn")` | `Parameters<typeof fn>` | Function parameter types |
| `TsInstanceTypeRef("typeof MyClass")` | `InstanceType<typeof MyClass>` | Constructor instance type |
| `TsAwaitedTypeRef("Promise<string>")` | `Awaited<Promise<string>>` | Unwrap Promise type |

#### Usage

```csharp
var partial = new TsPartialTypeRef("User");
var pick = new TsPickTypeRef("User", "'name' | 'email'");

// Use in type aliases
TsTypeBuilder.TypeAlias("UserUpdate", partial).Exported().Emit();
// export type UserUpdate = Partial<User>;

TsTypeBuilder.TypeAlias("UserSummary", pick).Exported().Emit();
// export type UserSummary = Pick<User, 'name' | 'email'>;

// Use in method parameters
builder.AddMethod("update", m => m
    .AddParameter("data", partial)
    .WithReturnType("void"));
```

### Implicit Conversions

All specialized type refs convert implicitly to both `TsTypeRef` and `string`:

```csharp
TsTypeRef t = new TsPromiseTypeRef("User");   // implicit to TsTypeRef
string s = new TsArrayTypeRef("number");        // implicit to string
```
