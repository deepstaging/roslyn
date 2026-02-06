# Domain Extensions

Domain-specific extensions for popular frameworks.

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
