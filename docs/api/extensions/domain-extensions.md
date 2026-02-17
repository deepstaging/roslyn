# Domain Extensions

Domain-specific extensions and utilities for popular frameworks.

> **See also:** [Extensions Overview](index.md)

---

## Entity Framework Core

```csharp
// Check if property is a DbSet<T>
bool isDbSet = propertySymbol.IsEfDbSet();

// Check if type is a DbContext
bool isContext = validSymbol.IsEfDbContext();
bool isNotContext = validSymbol.IsNotEfDbContext();
```

---

## LanguageExt

```csharp
// Filter methods returning LanguageExt Eff
var effMethods = typeSymbol.QueryMethods()
    .ReturningLanguageExtEff()
    .GetAll();

// Check if symbol is LanguageExt Eff type
bool isEff = symbol.IsLanguageExtEff();
```

---

## JSON Utilities

`JsonMerge` provides deep merge operations for JSON documents — useful for code fixes that need to update user-edited JSON files without losing existing values.

### Merge (Additive)

Adds missing keys from a template while preserving all existing values:

```csharp
string merged = JsonMerge.Apply(existingJson, templateJson);
```

- Existing keys are kept as-is
- Missing keys are added from the template
- Nested objects are recursively merged

### Sync (Authoritative)

Synchronizes a file with a template — adds missing keys, preserves existing values for matching keys, and removes keys not present in the template:

```csharp
string synced = JsonMerge.Sync(existingJson, templateJson);
```

- Keys starting with `$` (e.g., `$schema`) are always preserved
- Extra keys not in the template are removed
- Nested objects are recursively synced
