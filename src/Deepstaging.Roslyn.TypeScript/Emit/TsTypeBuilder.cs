// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System;
using System.Collections.Immutable;

namespace Deepstaging.Roslyn.TypeScript.Emit;

/// <summary>
/// Fluent builder for composing TypeScript type declarations: classes, interfaces, type aliases, enums, and const enums.
/// Immutable — each method returns a new instance via <c>with</c> expressions.
/// </summary>
/// <example>
/// <code>
/// var ts = TsTypeBuilder.Class("UserService")
///     .Exported()
///     .AddTypeParameter("T")
///     .Extends("BaseService")
///     .Implements("IDisposable")
///     .AddField("name", "string", f => f.WithAccessibility(TsAccessibility.Private))
///     .AddConstructor(c => c
///         .AddParameter("name", "string", p => p.AsParameterProperty(TsAccessibility.Public))
///         .WithBody(b => b.AddStatement("this.name = name")))
///     .AddProperty("displayName", "string", p => p.AsReadonly())
///     .AddMethod("greet", m => m
///         .WithReturnType("string")
///         .WithExpressionBody("`Hello, ${this.name}`"))
///     .Emit()
///     .ValidateOrThrow();
/// </code>
/// </example>
public readonly record struct TsTypeBuilder
{
    /// <summary>Gets the type name.</summary>
    public string Name { get; init; }

    /// <summary>Gets the kind of TypeScript type declaration.</summary>
    public TsTypeKind Kind { get; init; }

    /// <summary>Gets the properties defined on this type.</summary>
    public ImmutableArray<TsPropertyBuilder> Properties { get; init; }

    /// <summary>Gets the fields defined on this type.</summary>
    public ImmutableArray<TsFieldBuilder> Fields { get; init; }

    /// <summary>Gets the methods defined on this type.</summary>
    public ImmutableArray<TsMethodBuilder> Methods { get; init; }

    /// <summary>Gets the constructors defined on this type (usually 0–1 for TypeScript).</summary>
    public ImmutableArray<TsConstructorBuilder> Constructors { get; init; }

    /// <summary>Gets the generic type parameter names.</summary>
    public ImmutableArray<string> TypeParameters { get; init; }

    /// <summary>Gets the base types in the <c>extends</c> clause.</summary>
    public ImmutableArray<string> ExtendsClause { get; init; }

    /// <summary>Gets the interfaces in the <c>implements</c> clause.</summary>
    public ImmutableArray<string> ImplementsClause { get; init; }

    /// <summary>Gets the import statements to emit at the top of the file.</summary>
    public ImmutableArray<string> Imports { get; init; }

    /// <summary>Gets the decorator names applied to this type.</summary>
    public ImmutableArray<string> Decorators { get; init; }

    /// <summary>Gets the enum members (for <see cref="TsTypeKind.Enum"/> and <see cref="TsTypeKind.ConstEnum"/>).</summary>
    public ImmutableArray<(string Name, string? Value)> EnumMembers { get; init; }

    /// <summary>Gets the type alias definition body (for <see cref="TsTypeKind.TypeAlias"/>).</summary>
    public string? TypeAliasDefinition { get; init; }

    /// <summary>Gets the index signatures (e.g., <c>[key: string]: T</c>).</summary>
    public ImmutableArray<string> IndexSignatures { get; init; }

    /// <summary>Gets a value indicating whether this type is exported.</summary>
    public bool IsExported { get; init; }

    /// <summary>Gets a value indicating whether this type is the default export.</summary>
    public bool IsDefaultExport { get; init; }

    /// <summary>Gets a value indicating whether this type is abstract.</summary>
    public bool IsAbstract { get; init; }

    /// <summary>Gets a value indicating whether this type has the <c>declare</c> modifier.</summary>
    public bool IsDeclare { get; init; }

    /// <summary>Gets the JSDoc comment (<c>/** ... */</c>) for this type.</summary>
    public string? JsDocComment { get; init; }

    /// <summary>Gets the nested types declared within this type (for namespace-scoped types).</summary>
    public ImmutableArray<TsTypeBuilder> NestedTypes { get; init; }

    /// <summary>Initializes a new <see cref="TsTypeBuilder"/> with empty collections.</summary>
    public TsTypeBuilder()
    {
        Name = string.Empty;
        Properties = ImmutableArray<TsPropertyBuilder>.Empty;
        Fields = ImmutableArray<TsFieldBuilder>.Empty;
        Methods = ImmutableArray<TsMethodBuilder>.Empty;
        Constructors = ImmutableArray<TsConstructorBuilder>.Empty;
        TypeParameters = ImmutableArray<string>.Empty;
        ExtendsClause = ImmutableArray<string>.Empty;
        ImplementsClause = ImmutableArray<string>.Empty;
        Imports = ImmutableArray<string>.Empty;
        Decorators = ImmutableArray<string>.Empty;
        EnumMembers = ImmutableArray<(string, string?)>.Empty;
        IndexSignatures = ImmutableArray<string>.Empty;
        NestedTypes = ImmutableArray<TsTypeBuilder>.Empty;
    }

    // ── Factories ───────────────────────────────────────────────────────

    /// <summary>Creates a new <see cref="TsTypeBuilder"/> for a TypeScript class.</summary>
    /// <param name="name">The class name.</param>
    public static TsTypeBuilder Class(string name) =>
        new() { Name = name, Kind = TsTypeKind.Class };

    /// <summary>Creates a new <see cref="TsTypeBuilder"/> for a TypeScript interface.</summary>
    /// <param name="name">The interface name.</param>
    public static TsTypeBuilder Interface(string name) =>
        new() { Name = name, Kind = TsTypeKind.Interface };

    /// <summary>Creates a new <see cref="TsTypeBuilder"/> for a TypeScript type alias.</summary>
    /// <param name="name">The type alias name.</param>
    /// <param name="definition">The type alias body (the right-hand side of <c>=</c>).</param>
    public static TsTypeBuilder TypeAlias(string name, string definition) =>
        new() { Name = name, Kind = TsTypeKind.TypeAlias, TypeAliasDefinition = definition };

    /// <summary>Creates a new <see cref="TsTypeBuilder"/> for a TypeScript enum.</summary>
    /// <param name="name">The enum name.</param>
    public static TsTypeBuilder Enum(string name) =>
        new() { Name = name, Kind = TsTypeKind.Enum };

    /// <summary>Creates a new <see cref="TsTypeBuilder"/> for a TypeScript const enum.</summary>
    /// <param name="name">The const enum name.</param>
    public static TsTypeBuilder ConstEnum(string name) =>
        new() { Name = name, Kind = TsTypeKind.ConstEnum };

    // ── Modifiers ───────────────────────────────────────────────────────

    /// <summary>Marks this type as exported (<c>export</c>).</summary>
    public TsTypeBuilder Exported() =>
        this with { IsExported = true };

    /// <summary>Marks this type as the default export (<c>export default</c>).</summary>
    public TsTypeBuilder DefaultExported() =>
        this with { IsDefaultExport = true };

    /// <summary>Marks this type as abstract.</summary>
    public TsTypeBuilder AsAbstract() =>
        this with { IsAbstract = true };

    /// <summary>Marks this type with the <c>declare</c> modifier.</summary>
    public TsTypeBuilder AsDeclare() =>
        this with { IsDeclare = true };

    /// <summary>Adds a decorator to this type.</summary>
    /// <param name="decorator">The decorator name or expression (e.g., <c>"@Component"</c>).</param>
    public TsTypeBuilder WithDecorator(string decorator) =>
        this with { Decorators = Decorators.Add(decorator) };

    /// <summary>Sets the JSDoc comment for this type.</summary>
    /// <param name="comment">The JSDoc comment text (without <c>/** */</c> delimiters).</param>
    public TsTypeBuilder WithJsDoc(string comment) =>
        this with { JsDocComment = comment };

    // ── Generics ────────────────────────────────────────────────────────

    /// <summary>Adds a generic type parameter to this type.</summary>
    /// <param name="name">The type parameter name (e.g., <c>"T"</c>).</param>
    public TsTypeBuilder AddTypeParameter(string name) =>
        this with { TypeParameters = TypeParameters.Add(name) };

    /// <summary>Adds a constrained generic type parameter to this type.</summary>
    /// <param name="name">The type parameter name.</param>
    /// <param name="constraint">The constraint expression (e.g., <c>"Foo"</c> for <c>T extends Foo</c>).</param>
    public TsTypeBuilder AddTypeParameter(string name, string constraint) =>
        this with { TypeParameters = TypeParameters.Add($"{name} extends {constraint}") };

    // ── Inheritance ─────────────────────────────────────────────────────

    /// <summary>Adds a base type to the <c>extends</c> clause.</summary>
    /// <param name="baseType">The base type name.</param>
    public TsTypeBuilder Extends(string baseType) =>
        this with { ExtendsClause = ExtendsClause.Add(baseType) };

    /// <summary>Adds an interface to the <c>implements</c> clause.</summary>
    /// <param name="interfaceName">The interface name.</param>
    public TsTypeBuilder Implements(string interfaceName) =>
        this with { ImplementsClause = ImplementsClause.Add(interfaceName) };

    /// <summary>Adds multiple interfaces to the <c>implements</c> clause.</summary>
    /// <param name="interfaceNames">The interface names.</param>
    public TsTypeBuilder Implements(params string[] interfaceNames) =>
        this with { ImplementsClause = ImplementsClause.AddRange(interfaceNames) };

    // ── Members ─────────────────────────────────────────────────────────

    /// <summary>Adds a property configured via a builder function.</summary>
    /// <param name="name">The property name.</param>
    /// <param name="type">The TypeScript type annotation.</param>
    /// <param name="configure">A function that further configures the property builder.</param>
    public TsTypeBuilder AddProperty(string name, string type, Func<TsPropertyBuilder, TsPropertyBuilder> configure) =>
        this with { Properties = Properties.Add(configure(TsPropertyBuilder.For(name, type))) };

    /// <summary>Adds a pre-built property to this type.</summary>
    /// <param name="property">The property builder to add.</param>
    public TsTypeBuilder AddProperty(TsPropertyBuilder property) =>
        this with { Properties = Properties.Add(property) };

    /// <summary>Adds a field configured via a builder function.</summary>
    /// <param name="name">The field name.</param>
    /// <param name="type">The TypeScript type annotation.</param>
    /// <param name="configure">A function that further configures the field builder.</param>
    public TsTypeBuilder AddField(string name, string type, Func<TsFieldBuilder, TsFieldBuilder> configure) =>
        this with { Fields = Fields.Add(configure(TsFieldBuilder.For(name, type))) };

    /// <summary>Adds a pre-built field to this type.</summary>
    /// <param name="field">The field builder to add.</param>
    public TsTypeBuilder AddField(TsFieldBuilder field) =>
        this with { Fields = Fields.Add(field) };

    /// <summary>Adds a method configured via a builder function.</summary>
    /// <param name="name">The method name.</param>
    /// <param name="configure">A function that further configures the method builder.</param>
    public TsTypeBuilder AddMethod(string name, Func<TsMethodBuilder, TsMethodBuilder> configure) =>
        this with { Methods = Methods.Add(configure(TsMethodBuilder.For(name))) };

    /// <summary>Adds a pre-built method to this type.</summary>
    /// <param name="method">The method builder to add.</param>
    public TsTypeBuilder AddMethod(TsMethodBuilder method) =>
        this with { Methods = Methods.Add(method) };

    /// <summary>Adds a constructor configured via a builder function.</summary>
    /// <param name="configure">A function that configures the constructor builder.</param>
    public TsTypeBuilder AddConstructor(Func<TsConstructorBuilder, TsConstructorBuilder> configure) =>
        this with { Constructors = Constructors.Add(configure(TsConstructorBuilder.Create())) };

    /// <summary>Adds a pre-built constructor to this type.</summary>
    /// <param name="constructor">The constructor builder to add.</param>
    public TsTypeBuilder AddConstructor(TsConstructorBuilder constructor) =>
        this with { Constructors = Constructors.Add(constructor) };

    /// <summary>Adds an index signature to this type.</summary>
    /// <param name="keyName">The key parameter name (e.g., <c>"key"</c>).</param>
    /// <param name="keyType">The key type (e.g., <c>"string"</c>).</param>
    /// <param name="valueType">The value type (e.g., <c>"unknown"</c>).</param>
    public TsTypeBuilder AddIndexSignature(string keyName, string keyType, string valueType) =>
        this with { IndexSignatures = IndexSignatures.Add($"[{keyName}: {keyType}]: {valueType}") };

    /// <summary>Adds a nested type declaration within this type.</summary>
    /// <param name="nestedType">The nested type builder.</param>
    public TsTypeBuilder AddNestedType(TsTypeBuilder nestedType) =>
        this with { NestedTypes = NestedTypes.Add(nestedType) };

    // ── Enum members ────────────────────────────────────────────────────

    /// <summary>Adds an enum member with an auto-assigned value.</summary>
    /// <param name="name">The enum member name.</param>
    public TsTypeBuilder AddEnumMember(string name) =>
        this with { EnumMembers = EnumMembers.Add((name, null)) };

    /// <summary>Adds an enum member with an explicit value.</summary>
    /// <param name="name">The enum member name.</param>
    /// <param name="value">The explicit value expression.</param>
    public TsTypeBuilder AddEnumMember(string name, string value) =>
        this with { EnumMembers = EnumMembers.Add((name, value)) };

    // ── Imports ─────────────────────────────────────────────────────────

    /// <summary>Adds a raw import statement to emit at the top of the file.</summary>
    /// <param name="importStatement">The full import statement (e.g., <c>"import { Foo } from './foo'"</c>).</param>
    public TsTypeBuilder AddImport(string importStatement) =>
        this with { Imports = Imports.Add(importStatement) };

    // ── Emit ────────────────────────────────────────────────────────────

    /// <summary>Emits the TypeScript source code for this type using default options.</summary>
    public TsOptionalEmit Emit() =>
        TsEmitter.Emit(this, TsEmitOptions.Default);

    /// <summary>Emits the TypeScript source code for this type using the specified options.</summary>
    /// <param name="options">The emit options to use.</param>
    public TsOptionalEmit Emit(TsEmitOptions options) =>
        TsEmitter.Emit(this, options);
}
