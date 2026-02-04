// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn;

/// <summary>
/// An optional Roslyn attribute that may or may not contain a value.
/// Provides fluent argument extraction with null-safety.
/// Use Validate() or IsNotValid() to get an Attribute with guaranteed non-null access.
/// </summary>
public readonly struct OptionalAttribute(AttributeData? attribute)
    : IValidatableProjection<AttributeData, ValidAttribute>
{
    /// <summary>
    /// Creates a projection with an attribute.
    /// </summary>
    public static OptionalAttribute WithValue(AttributeData attribute)
    {
        return new OptionalAttribute(attribute);
    }

    /// <summary>
    /// Creates a projection without an attribute.
    /// </summary>
    public static OptionalAttribute Empty()
    {
        return new OptionalAttribute(null);
    }

    /// <summary>
    /// Creates a projection from a nullable attribute.
    /// </summary>
    public static OptionalAttribute FromNullable(AttributeData? attribute)
    {
        return attribute != null ? WithValue(attribute) : Empty();
    }

    /// <summary>
    /// Gets a constructor argument at the specified index as a projected argument.
    /// Constructor arguments are positional parameters passed when the attribute is applied.
    /// Example: [MyAttribute("value1", 123)] has constructor args at index 0 and 1.
    /// </summary>
    /// <typeparam name="T">The type of the argument.</typeparam>
    /// <param name="index">The zero-based index of the constructor argument.</param>
    /// <returns>A projected argument for fluent transformation and materialization.</returns>
    public OptionalArgument<T> ConstructorArg<T>(int index)
    {
        return attribute?.GetConstructorArgument<T>(index)
               ?? OptionalArgument<T>.Empty();
    }

    /// <summary>
    /// Gets a named argument as a projected argument.
    /// Named arguments are properties set when the attribute is applied.
    /// Example: [MyAttribute(MaxRetries = 5)] has a named arg "MaxRetries".
    /// </summary>
    /// <typeparam name="T">The type of the argument.</typeparam>
    /// <param name="name">The name of the named argument (property name).</param>
    /// <returns>A projected argument for fluent transformation and materialization.</returns>
    public OptionalArgument<T> NamedArg<T>(string name)
    {
        return attribute?.GetNamedArgument<T>(name)
               ?? OptionalArgument<T>.Empty();
    }

    /// <summary>
    /// Maps the attribute to a different type using the provided function.
    /// The mapper receives the ValidAttribute with guaranteed non-null access.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="mapper">The mapping function.</param>
    /// <returns>A new projected argument with the result type.</returns>
    public OptionalValue<TResult> Map<TResult>(Func<ValidAttribute, TResult> mapper)
    {
        return attribute != null
            ? OptionalValue<TResult>.WithValue(mapper(ValidAttribute.From(attribute)))
            : OptionalValue<TResult>.Empty();
    }

    /// <summary>
    /// Extracts multiple arguments at once using a custom extraction function.
    /// Useful for building configuration objects from attribute data.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="extractor">Function that extracts data from the validated attribute.</param>
    /// <returns>A projected argument with the extracted result.</returns>
    public OptionalValue<TResult> WithArgs<TResult>(Func<ValidAttribute, TResult> extractor)
    {
        return Map(extractor);
    }

    /// <summary>
    /// Gets the type arguments for generic attributes as TypeModels.
    /// Example: [MyAttribute&lt;TRuntime, TEvent&gt;] has two type arguments.
    /// Returns empty array if attribute class is not generic or has no type arguments.
    /// </summary>
    /// <returns>Array of TypeModels representing the type arguments.</returns>
    public ImmutableArray<OptionalSymbol<INamedTypeSymbol>> GetTypeArguments()
    {
        return attribute?.AttributeClass is { IsGenericType: true } attrType
            ? [..attrType.TypeArguments.Select(t => t.AsNamedType())]
            : [];
    }

    /// <summary>
    /// Gets the attribute class type symbol.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when attribute is not present.</exception>
    public INamedTypeSymbol AttributeClass =>
        attribute?.AttributeClass ?? throw new InvalidOperationException("Attribute is not present");

    /// <summary>
    /// Gets a specific type argument by index as a TypeModel.
    /// Example: [MyAttribute&lt;TRuntime, TEvent&gt;] - index 0 is TRuntime, index 1 is TEvent.
    /// Returns Empty projection if attribute is not generic or index is out of range.
    /// </summary>
    /// <param name="index">Zero-based index of the type argument.</param>
    /// <returns>OptionalArgument containing the TypeModel, or Empty if not found.</returns>
    public OptionalArgument<INamedTypeSymbol> GetTypeArgument(int index)
    {
        if (attribute?.AttributeClass is not { IsGenericType: true } attrType)
            return OptionalArgument<INamedTypeSymbol>.Empty();

        if (index < 0 || index >= attrType.TypeArguments.Length)
            return OptionalArgument<INamedTypeSymbol>.Empty();

        var type = attrType.TypeArguments[index].AsValidNamedType();
        return OptionalArgument<INamedTypeSymbol>.WithValue(type.Value);
    }

    /// <summary>
    /// Gets a specific type argument by index as a projected symbol.
    /// Returns Empty projection if attribute is not generic or index is out of range.
    /// </summary>
    /// <param name="index">Zero-based index of the type argument.</param>
    /// <returns>OptionalSymbol containing the type symbol, or Empty if not found.</returns>
    public OptionalSymbol<ITypeSymbol> GetTypeArgumentSymbol(int index)
    {
        if (attribute?.AttributeClass is not { IsGenericType: true } attrType)
            return OptionalSymbol<ITypeSymbol>.Empty();

        if (index < 0 || index >= attrType.TypeArguments.Length)
            return OptionalSymbol<ITypeSymbol>.Empty();

        return OptionalSymbol<ITypeSymbol>.WithValue(attrType.TypeArguments[index]);
    }

    /// <summary>
    /// Executes an action if the attribute is present.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns>This projected attribute for chaining.</returns>
    public OptionalAttribute Do(Action<AttributeData> action)
    {
        if (attribute != null)
            action(attribute);

        return this;
    }

    /// <summary>
    /// Executes an action if the attribute is NOT present.
    /// </summary>
    /// <param name="action">The action to execute when attribute is missing.</param>
    /// <returns>This projected attribute for chaining.</returns>
    public OptionalAttribute OrElse(Action action)
    {
        if (attribute == null)
            action();

        return this;
    }

    /// <summary>
    /// Returns the attribute or null if not present.
    /// </summary>
    public AttributeData? OrNull()
    {
        return attribute;
    }

    /// <summary>
    /// Returns the attribute or throws if not present.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when attribute is not found.</exception>
    public AttributeData OrThrow(string? message = null)
    {
        return attribute ?? throw new InvalidOperationException(message ?? "Attribute not found");
    }

    /// <summary>
    /// Returns the attribute or throws an exception with a lazily-computed message if not present.
    /// </summary>
    /// <param name="messageFactory">Factory function to create the error message.</param>
    /// <exception cref="InvalidOperationException">Thrown when attribute is not found.</exception>
    public AttributeData OrThrow(Func<string> messageFactory)
    {
        return attribute ?? throw new InvalidOperationException(messageFactory());
    }

    /// <summary>
    /// Returns the attribute or throws a custom exception if not present.
    /// </summary>
    /// <param name="exceptionFactory">Factory function to create the exception.</param>
    public AttributeData OrThrow(Func<Exception> exceptionFactory)
    {
        return attribute ?? throw exceptionFactory();
    }

    /// <summary>
    /// Validates this optional attribute to an Attribute with guaranteed non-null access.
    /// Returns null if the attribute is not present.
    /// </summary>
    public ValidAttribute? Validate()
    {
        return attribute != null ? ValidAttribute.From(attribute) : null;
    }

    /// <summary>
    /// Validates this optional attribute, throwing if not present.
    /// </summary>
    public ValidAttribute ValidateOrThrow(string? message = null)
    {
        return attribute != null
            ? ValidAttribute.From(attribute)
            : throw new InvalidOperationException(message ?? "Cannot validate empty OptionalAttribute");
    }

    /// <summary>
    /// Attempts to validate the optional attribute. Returns true if valid.
    /// </summary>
    public bool TryValidate(out ValidAttribute validated)
    {
        if (attribute != null)
        {
            validated = ValidAttribute.From(attribute);
            return true;
        }

        validated = default;
        return false;
    }

    /// <summary>
    /// Checks if the optional attribute is NOT valid (empty). Returns true if INVALID.
    /// Enables fast-exit pattern: if (attr.IsNotValid(out var valid)) return;
    /// </summary>
    public bool IsNotValid(out ValidAttribute validated)
    {
        return !TryValidate(out validated);
    }

    /// <summary>
    /// Checks if the attribute is present.
    /// </summary>
    public bool HasValue => attribute != null;

    /// <summary>
    /// Checks if the attribute is NOT present (negation of HasValue).
    /// </summary>
    public bool IsEmpty => attribute == null;

    /// <summary>
    /// Gets the underlying attribute. Throws if attribute is not present.
    /// Use this when you've already verified HasValue or within a Match/Map callback.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when attribute is not present.</exception>
    public AttributeData Value => attribute ?? throw new InvalidOperationException("Attribute is empty");

    /// <summary>
    /// Pattern matching with discriminated union semantics.
    /// Executes the appropriate function based on whether the attribute is present.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="whenPresent">Function to execute when attribute is present (receives non-null attribute).</param>
    /// <param name="whenEmpty">Function to execute when attribute is empty.</param>
    /// <returns>The result from the executed function.</returns>
    /// <example>
    /// <code>
    /// var name = attr.Match(
    ///     whenPresent: a => a.AttributeClass?.Name ?? "Unknown",
    ///     whenEmpty: () => "NoAttribute");
    /// </code>
    /// </example>
    public TResult Match<TResult>(
        Func<AttributeData, TResult> whenPresent,
        Func<TResult> whenEmpty)
    {
        return attribute != null ? whenPresent(attribute) : whenEmpty();
    }

    /// <summary>
    /// Pattern matching with void return (for side effects).
    /// Executes the appropriate action based on whether the attribute is present.
    /// </summary>
    /// <param name="whenPresent">Action to execute when attribute is present (receives non-null attribute).</param>
    /// <param name="whenEmpty">Action to execute when attribute is empty.</param>
    public void Match(
        Action<AttributeData> whenPresent,
        Action whenEmpty)
    {
        if (attribute != null)
            whenPresent(attribute);
        else
            whenEmpty();
    }

    /// <summary>
    /// Gets the underlying AttributeData if present, otherwise returns the provided default.
    /// </summary>
    public AttributeData OrDefault(AttributeData defaultValue)
    {
        return attribute ?? defaultValue;
    }

    /// <summary>
    /// Gets the attribute class name without the "Attribute" suffix.
    /// Returns null if attribute is not present.
    /// </summary>
    /// <example>
    /// RuntimeDependencyAttribute → "RuntimeDependency"
    /// EffectsAttribute → "Effects"
    /// MyAttribute → "My"
    /// </example>
    public string? PropertyName
    {
        get
        {
            if (attribute?.AttributeClass == null)
                return null;

            var name = attribute.AttributeClass.Name;

            // Strip "Attribute" suffix if present
            return (name.EndsWith("Attribute") && name.Length > "Attribute".Length
                ? name.Substring(0, name.Length - "Attribute".Length)
                : name).ToPascalCase();
        }
    }

    /// <summary>
    /// Gets a suggested parameter name (camelCase) derived from the attribute class name.
    /// Strips "Attribute" suffix and converts to camelCase.
    /// Returns null if attribute is not present.
    /// </summary>
    /// <example>
    /// RuntimeDependencyAttribute → "runtimeDependency"
    /// EffectsAttribute → "effects"
    /// MyAttribute → "my"
    /// </example>
    public string? ParameterName => PropertyName?.ToCamelCase();
}