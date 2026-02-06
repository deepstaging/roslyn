# Attribute Extensions

Extensions for extracting attribute arguments fluently.

> **See also:** [Extensions Overview](index.md) | [Projections](../projections/index.md)

---

## AttributeData Extensions

### Wrap in Projection

```csharp
OptionalAttribute optional = attributeData.Query();
```

### Type Checking

```csharp
bool isObsolete = attributeData.Is<ObsoleteAttribute>();
T? typed = attributeData.As<ObsoleteAttribute>();
```

### Get Arguments

```csharp
// Named argument
OptionalArgument<int> retries = attributeData.GetNamedArgument<int>("MaxRetries");
int value = retries.OrDefault(3);

// Constructor argument by index
OptionalArgument<string> name = attributeData.GetConstructorArgument<string>(0);
```
