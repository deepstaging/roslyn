// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn;

/// <summary>
/// Pipeline-safe snapshot of common symbol data from <see cref="ValidSymbol{TSymbol}"/>.
/// Contains all properties that are shared across all symbol types.
/// Use specialized snapshots (<see cref="TypeSnapshot"/>, <see cref="MethodSnapshot"/>, etc.)
/// for symbol-type-specific data.
/// </summary>
public record SymbolSnapshot : IEquatable<SymbolSnapshot>
{
    #region Name & Display

    /// <summary>
    /// Gets the name of the symbol.
    /// </summary>
    public string Name { get; init; } = "";

    /// <summary>
    /// Gets the containing namespace, or null if in global namespace.
    /// </summary>
    public string? Namespace { get; init; }

    /// <summary>
    /// Gets the fully qualified name without global namespace prefix.
    /// </summary>
    public string FullyQualifiedName { get; init; } = "";

    /// <summary>
    /// Gets the fully qualified name with global namespace prefix.
    /// Uses canonical form without C# keyword substitution (e.g., <c>global::System.Int32</c>).
    /// For code generation that should emit C# keywords (e.g., <c>int</c>), use <see cref="CodeName"/>.
    /// </summary>
    public string GloballyQualifiedName { get; init; } = "";

    /// <summary>
    /// Gets the type name suitable for emitting in generated C# code.
    /// Uses C# keyword aliases where applicable (e.g., <c>int</c> instead of <c>global::System.Int32</c>).
    /// Falls back to <see cref="GloballyQualifiedName"/> for non-keyword types.
    /// </summary>
    public string CodeName { get; init; } = "";

    /// <summary>
    /// Gets the display name (namespace.name) of the symbol.
    /// </summary>
    public string DisplayName =>
        string.IsNullOrEmpty(Namespace) ? Name : $"{Namespace}.{Name}";

    /// <summary>
    /// Gets a suggested property name derived from the symbol name.
    /// </summary>
    public string PropertyName { get; init; } = "";

    /// <summary>
    /// Gets a suggested parameter name (camelCase) derived from the property name.
    /// </summary>
    public string ParameterName { get; init; } = "";

    #endregion

    #region Accessibility

    /// <summary>
    /// Gets the accessibility as a string keyword.
    /// </summary>
    public string AccessibilityString { get; init; } = "";

    /// <summary>
    /// Gets a value indicating whether the symbol is public.
    /// </summary>
    public bool IsPublic { get; init; }

    /// <summary>
    /// Gets a value indicating whether the symbol is internal.
    /// </summary>
    public bool IsInternal { get; init; }

    #endregion

    #region Modifiers

    /// <summary>
    /// Gets a value indicating whether the symbol is static.
    /// </summary>
    public bool IsStatic { get; init; }

    /// <summary>
    /// Gets a value indicating whether the symbol is abstract.
    /// </summary>
    public bool IsAbstract { get; init; }

    /// <summary>
    /// Gets a value indicating whether the symbol is sealed.
    /// </summary>
    public bool IsSealed { get; init; }

    /// <summary>
    /// Gets a value indicating whether the symbol is virtual.
    /// </summary>
    public bool IsVirtual { get; init; }

    /// <summary>
    /// Gets a value indicating whether the symbol is an override.
    /// </summary>
    public bool IsOverride { get; init; }

    /// <summary>
    /// Gets a value indicating whether the symbol is readonly.
    /// </summary>
    public bool IsReadOnly { get; init; }

    /// <summary>
    /// Gets a value indicating whether the symbol is partial.
    /// </summary>
    public bool IsPartial { get; init; }

    #endregion

    #region Type Classification

    /// <summary>
    /// Gets the kind of the symbol as a string keyword (class, struct, interface, etc.).
    /// </summary>
    public string? Kind { get; init; }

    /// <summary>
    /// Gets a value indicating whether the symbol is a value type.
    /// </summary>
    public bool IsValueType { get; init; }

    /// <summary>
    /// Gets a value indicating whether the symbol is a reference type.
    /// </summary>
    public bool IsReferenceType { get; init; }

    /// <summary>
    /// Gets a value indicating whether the symbol has nullable annotation.
    /// </summary>
    public bool IsNullable { get; init; }

    #endregion

    #region Documentation

    /// <summary>
    /// Gets the documentation snapshot for this symbol.
    /// </summary>
    public DocumentationSnapshot Documentation { get; init; } = DocumentationSnapshot.Empty;

    #endregion

    /// <inheritdoc />
    public override string ToString() => GloballyQualifiedName;
}
