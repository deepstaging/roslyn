// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn;

/// <summary>
/// A validated Roslyn attribute where the AttributeData is guaranteed non-null.
/// Provides non-nullable access to attribute properties and arguments.
/// Create via OptionalAttribute.Validate() or ValidAttribute.From().
/// </summary>
public readonly struct ValidAttribute : IProjection<AttributeData>
{
    private readonly AttributeData _attribute;

    private ValidAttribute(AttributeData attribute)
    {
        _attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));
    }

    /// <summary>
    /// Creates a validated attribute from non-null AttributeData.
    /// </summary>
    public static ValidAttribute From(AttributeData attribute)
    {
        return new ValidAttribute(attribute);
    }

    /// <summary>
    /// Attempts to create a validated attribute from nullable AttributeData.
    /// Returns null if the attribute is null.
    /// </summary>
    public static ValidAttribute? TryFrom(AttributeData? attribute)
    {
        return attribute != null ? new ValidAttribute(attribute) : null;
    }

    /// <summary>
    /// Gets the underlying non-null AttributeData.
    /// </summary>
    public AttributeData Value => _attribute;

    // IProjection<AttributeData> implementation

    /// <summary>
    /// Always returns true for validated attributes.
    /// </summary>
    public bool HasValue => true;

    /// <summary>
    /// Always returns false for validated attributes.
    /// </summary>
    public bool IsEmpty => false;

    /// <summary>
    /// Returns the guaranteed non-null attribute.
    /// </summary>
    public AttributeData OrThrow(string? message = null)
    {
        return _attribute;
    }

    /// <summary>
    /// Returns the guaranteed non-null attribute.
    /// </summary>
    public AttributeData OrNull()
    {
        return _attribute;
    }

    /// <summary>
    /// Gets the attribute class (the type of the attribute).
    /// </summary>
    public INamedTypeSymbol AttributeClass =>
        _attribute.AttributeClass ?? throw new InvalidOperationException("AttributeClass is null");

    /// <summary>
    /// Gets a constructor argument at the specified index.
    /// </summary>
    public OptionalArgument<T> ConstructorArg<T>(int index)
    {
        var result = _attribute.GetConstructorArgument<T>(index);
        return result.HasValue ? result : OptionalArgument<T>.Empty();
    }

    /// <summary>
    /// Gets a named argument (property value) from the attribute.
    /// </summary>
    public OptionalArgument<T> NamedArg<T>(string name)
    {
        var result = _attribute.GetNamedArgument<T>(name);
        return result.HasValue ? result : OptionalArgument<T>.Empty();
    }

    /// <summary>
    /// Gets a named argument (property value) from the attribute (alias for NamedArg).
    /// </summary>
    public OptionalArgument<T> GetNamedArgument<T>(string name)
    {
        return NamedArg<T>(name);
    }

    /// <summary>
    /// Maps the attribute to a different type.
    /// </summary>
    public TResult Map<TResult>(Func<AttributeData, TResult> mapper)
    {
        return mapper(_attribute);
    }

    /// <summary>
    /// Gets the type arguments for generic attributes as TypeModels.
    /// </summary>
    public ImmutableArray<ValidSymbol<INamedTypeSymbol>> GetTypeArguments()
    {
        return _attribute.AttributeClass is { IsGenericType: true } attrType
            ? [..attrType.TypeArguments.Select(t => t.AsValidNamedType())]
            : ImmutableArray<ValidSymbol<INamedTypeSymbol>>.Empty;
    }

    /// <summary>
    /// Gets a specific type argument by index as a TypeModel.
    /// </summary>
    public OptionalSymbol<INamedTypeSymbol> GetTypeArgument(int index)
    {
        if (_attribute.AttributeClass is not { IsGenericType: true } attrType)
            return OptionalSymbol<INamedTypeSymbol>.Empty();

        if (index < 0 || index >= attrType.TypeArguments.Length)
            return OptionalSymbol<INamedTypeSymbol>.Empty();

        return attrType.TypeArguments[index].AsNamedType();
    }

    /// <summary>
    /// Gets a specific type argument by index as an optional symbol.
    /// </summary>
    public OptionalSymbol<INamedTypeSymbol> GetTypeArgumentSymbol(int index)
    {
        if (_attribute.AttributeClass is not { IsGenericType: true } attrType)
            return OptionalSymbol<INamedTypeSymbol>.Empty();

        if (index < 0 || index >= attrType.TypeArguments.Length)
            return OptionalSymbol<INamedTypeSymbol>.Empty();

        return attrType.TypeArguments[index].AsNamedType();
    }

    /// <summary>
    /// Gets the attribute class name without the "Attribute" suffix.
    /// </summary>
    public string PropertyName
    {
        get
        {
            var name = AttributeClass.Name;
            return (name.EndsWith("Attribute") && name.Length > "Attribute".Length
                ? name.Substring(0, name.Length - "Attribute".Length)
                : name).ToPascalCase();
        }
    }

    /// <summary>
    /// Gets a suggested parameter name (camelCase) derived from the attribute class name.
    /// </summary>
    public string ParameterName => PropertyName.ToCamelCase();

    /// <summary>
    /// Executes an action with the attribute.
    /// </summary>
    public ValidAttribute Do(Action<AttributeData> action)
    {
        action(_attribute);
        return this;
    }
}