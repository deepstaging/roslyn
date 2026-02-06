# XmlDocumentation

Parsed XML documentation from a symbol.

> **See also:** [Projections Overview](index.md) | [ValidSymbol](valid-symbol.md)

## Creating

```csharp
var doc = symbol.XmlDocumentation;      // from OptionalSymbol/ValidSymbol
var doc = XmlDocumentation.FromSymbol(symbol);
```

## Checking Presence

```csharp
doc.HasValue                // bool
doc.IsEmpty                 // bool
```

## Content

```csharp
doc.Summary                 // string?
doc.Remarks                 // string?
doc.Returns                 // string?
doc.Value                   // string?
doc.Example                 // string?
doc.RawXml                  // string?
```

## Parameters

```csharp
doc.Params                  // ImmutableDictionary<string, string>
doc.GetParam("name")        // string?
```

## Type Parameters

```csharp
doc.TypeParams              // ImmutableDictionary<string, string>
doc.GetTypeParam("T")       // string?
```

## Exceptions and References

```csharp
doc.Exceptions              // ImmutableArray<(string Type, string Description)>
doc.SeeAlso                 // ImmutableArray<string>
```
