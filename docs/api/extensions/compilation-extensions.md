# Compilation Extensions

Extensions for Compilation and namespace types.

> **See also:** [Extensions Overview](index.md)

---

## INamespaceSymbol Extensions

Navigate namespace hierarchies.

```csharp
TypeQuery types = namespaceSymbol.QueryTypes();
```

---

## Compilation Extensions

Query types from a compilation.

```csharp
TypeQuery types = compilation.QueryTypes();
```
