# Deepstaging.Roslyn.LanguageExt

LanguageExt type references and emit patterns for Roslyn source generators built with [Deepstaging.Roslyn](https://github.com/deepstaging/roslyn).

## Usage

```csharp
using Deepstaging.Roslyn.LanguageExt.Refs;

// Create type references for LanguageExt types
var effType = LanguageExtRefs.Eff("RT", "int");        // global::LanguageExt.Eff<RT, int>
var optionType = LanguageExtRefs.Option("string");      // global::LanguageExt.Option<string>
var unitType = LanguageExtRefs.Unit;                     // global::LanguageExt.Unit

// Use namespaces in generated code
var ns = LanguageExtRefs.Namespace;                      // LanguageExt
var prelude = LanguageExtRefs.PreludeStatic;             // static LanguageExt.Prelude
```

## Package Structure

This is a satellite package following the `Deepstaging.Roslyn.{Library}` convention:

- **Refs/** — Static factory classes for type references
- **Patterns/** — Reusable emit patterns *(future)*
- **Extensions/** — TypeBuilder/MethodBuilder extensions *(future)*
