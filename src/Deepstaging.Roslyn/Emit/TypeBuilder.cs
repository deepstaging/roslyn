// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit;

/// <summary>
/// Fluent builder for type declarations (classes, interfaces, structs, records).
/// Supports adding members (properties, methods, fields, constructors) and emitting compilable C# code.
/// Immutable - each method returns a new instance.
/// </summary>
public record struct TypeBuilder
{
    /// <summary>Gets the type name.</summary>
    public string Name { get; init; }

    /// <summary>Gets the type kind (class, interface, struct).</summary>
    public TypeKind Kind { get; init; }

    /// <summary>Gets the namespace.</summary>
    public string? Namespace { get; init; }

    /// <summary>Gets the accessibility level.</summary>
    public Accessibility Accessibility { get; init; }

    /// <summary>Gets whether the type is static.</summary>
    public bool IsStatic { get; init; }

    /// <summary>Gets whether the type is abstract.</summary>
    public bool IsAbstract { get; init; }

    /// <summary>Gets whether the type is sealed.</summary>
    public bool IsSealed { get; init; }

    /// <summary>Gets whether the type is partial.</summary>
    public bool IsPartial { get; init; }

    /// <summary>Gets whether the type is a record.</summary>
    public bool IsRecord { get; init; }

    /// <summary>Gets the using directives.</summary>
    public ImmutableArray<string> Usings { get; init; }

    /// <summary>Gets the properties.</summary>
    public ImmutableArray<PropertyBuilder> Properties { get; init; }

    /// <summary>Gets the fields.</summary>
    public ImmutableArray<FieldBuilder> Fields { get; init; }

    /// <summary>Gets the events.</summary>
    public ImmutableArray<EventBuilder> Events { get; init; }

    /// <summary>Gets the methods.</summary>
    public ImmutableArray<MethodBuilder> Methods { get; init; }

    /// <summary>Gets the conversion operators.</summary>
    public ImmutableArray<ConversionOperatorBuilder> ConversionOperators { get; init; }

    /// <summary>Gets the operators.</summary>
    public ImmutableArray<OperatorBuilder> Operators { get; init; }

    /// <summary>Gets the constructors.</summary>
    public ImmutableArray<ConstructorBuilder> Constructors { get; init; }

    /// <summary>Gets the nested types.</summary>
    public ImmutableArray<TypeBuilder> NestedTypes { get; init; }

    /// <summary>Gets the interfaces (may include conditional interfaces).</summary>
    public ImmutableArray<ConditionalInterface> Interfaces { get; init; }

    /// <summary>Gets the attributes.</summary>
    public ImmutableArray<AttributeBuilder> Attributes { get; init; }

    /// <summary>Gets the XML documentation builder.</summary>
    public XmlDocumentationBuilder? XmlDoc { get; init; }

    /// <summary>Gets the primary constructor.</summary>
    public ConstructorBuilder? PrimaryConstructor { get; init; }

    /// <summary>Gets the generic type parameters.</summary>
    public ImmutableArray<TypeParameterBuilder> TypeParameters { get; init; }

    /// <summary>Gets user-defined metadata that does not affect code generation.</summary>
    public ImmutableDictionary<string, object?>? Metadata { get; init; }

    #region Factory Methods

    /// <summary>
    /// Creates a class type builder.
    /// </summary>
    /// <param name="name">The class name (e.g., "Customer", "Repository").</param>
    public static TypeBuilder Class(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Type name cannot be null or empty.", nameof(name));

        return new TypeBuilder
        {
            Name = name,
            Kind = TypeKind.Class,
            Accessibility = Accessibility.Public
        };
    }

    /// <summary>
    /// Creates an interface type builder.
    /// </summary>
    /// <param name="name">The interface name (e.g., "IRepository", "IService").</param>
    public static TypeBuilder Interface(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Type name cannot be null or empty.", nameof(name));

        return new TypeBuilder
        {
            Name = name,
            Kind = TypeKind.Interface,
            Accessibility = Accessibility.Public
        };
    }

    /// <summary>
    /// Creates a struct type builder.
    /// </summary>
    /// <param name="name">The struct name (e.g., "Point", "Vector").</param>
    public static TypeBuilder Struct(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Type name cannot be null or empty.", nameof(name));

        return new TypeBuilder
        {
            Name = name,
            Kind = TypeKind.Struct,
            Accessibility = Accessibility.Public
        };
    }

    /// <summary>
    /// Creates a record type builder.
    /// </summary>
    /// <param name="name">The record name (e.g., "Customer", "Person").</param>
    public static TypeBuilder Record(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Type name cannot be null or empty.", nameof(name));

        return new TypeBuilder
        {
            Name = name,
            Kind = TypeKind.Class,
            IsRecord = true,
            Accessibility = Accessibility.Public
        };
    }

    /// <summary>
    /// Creates a type builder by parsing a C# type signature.
    /// </summary>
    /// <param name="signature">The type signature (e.g., "public class CustomerService : IService").</param>
    /// <returns>A configured TypeBuilder with parsed kind, modifiers, and base types.</returns>
    /// <exception cref="ArgumentException">Thrown when the signature cannot be parsed.</exception>
    /// <example>
    /// <code>
    /// // Class with interface
    /// var builder = TypeBuilder.Parse("public class CustomerService : IService, IDisposable");
    /// 
    /// // Sealed class
    /// var builder = TypeBuilder.Parse("public sealed class CacheEntry");
    /// 
    /// // Abstract class
    /// var builder = TypeBuilder.Parse("public abstract class BaseHandler");
    /// 
    /// // Partial record
    /// var builder = TypeBuilder.Parse("public partial record OrderDto");
    /// </code>
    /// </example>
    public static TypeBuilder Parse(string signature)
    {
        return SignatureParser.ParseType(signature);
    }

    #endregion

    #region Namespace & Usings

    /// <summary>
    /// Sets the namespace for the type.
    /// </summary>
    /// <param name="namespace">The namespace (e.g., "MyApp.Domain", "Company.Project").</param>
    public TypeBuilder InNamespace(string @namespace)
    {
        return this with { Namespace = @namespace };
    }

    /// <summary>
    /// Adds a using directive to the compilation unit.
    /// </summary>
    /// <param name="namespace">The namespace to use (e.g., "System", "System.Collections.Generic").</param>
    public TypeBuilder AddUsing(string @namespace)
    {
        var usings = Usings.IsDefault ? [] : Usings;
        return this with { Usings = usings.Add(@namespace) };
    }

    /// <summary>
    ///  Adds multiple using directives to the compilation unit.
    /// </summary>
    /// <param name="namespaces"> The namespaces to use.</param>
    public TypeBuilder AddUsings(params string[] namespaces)
    {
        var usings = Usings.IsDefault ? [] : Usings;
        return this with { Usings = usings.AddRange(namespaces) };
    }

    #endregion

    #region Accessibility & Modifiers

    /// <summary>
    /// Sets the accessibility of the type.
    /// </summary>
    public TypeBuilder WithAccessibility(Accessibility accessibility)
    {
        return this with { Accessibility = accessibility };
    }

    /// <summary>
    /// Sets the accessibility of the type from a keyword string (e.g., "public", "internal").
    /// Accepts the same values produced by <see cref="ValidSymbol{TSymbol}.AccessibilityString"/>
    /// and <see cref="SymbolSnapshot.AccessibilityString"/>.
    /// </summary>
    public TypeBuilder WithAccessibility(string accessibilityKeyword) =>
        WithAccessibility(AccessibilityHelper.Parse(accessibilityKeyword));

    /// <summary>
    /// Marks the type as static.
    /// </summary>
    public TypeBuilder AsStatic()
    {
        return this with { IsStatic = true };
    }

    /// <summary>
    /// Marks the type as abstract.
    /// </summary>
    public TypeBuilder AsAbstract()
    {
        return this with { IsAbstract = true };
    }

    /// <summary>
    /// Marks the type as sealed.
    /// </summary>
    public TypeBuilder AsSealed()
    {
        return this with { IsSealed = true };
    }

    /// <summary>
    /// Marks the type as partial.
    /// </summary>
    public TypeBuilder AsPartial()
    {
        return this with { IsPartial = true };
    }

    #endregion

    #region Type Parameters

    /// <summary>
    /// Adds a generic type parameter to the type.
    /// </summary>
    /// <param name="name">The type parameter name (e.g., "T", "RT", "TResult").</param>
    public TypeBuilder AddTypeParameter(string name)
    {
        var typeParameter = TypeParameterBuilder.For(name);
        var typeParams = TypeParameters.IsDefault ? [] : TypeParameters;
        return this with { TypeParameters = typeParams.Add(typeParameter) };
    }

    /// <summary>
    /// Adds a generic type parameter with constraints.
    /// </summary>
    /// <param name="name">The type parameter name.</param>
    /// <param name="configure">A function to configure constraints on the type parameter.</param>
    public TypeBuilder AddTypeParameter(string name, Func<TypeParameterBuilder, TypeParameterBuilder> configure)
    {
        var typeParameter = configure(TypeParameterBuilder.For(name));
        var typeParams = TypeParameters.IsDefault ? [] : TypeParameters;
        return this with { TypeParameters = typeParams.Add(typeParameter) };
    }

    /// <summary>
    /// Adds a pre-configured type parameter builder.
    /// </summary>
    /// <param name="typeParameter">The type parameter builder to add.</param>
    public TypeBuilder AddTypeParameter(TypeParameterBuilder typeParameter)
    {
        var typeParams = TypeParameters.IsDefault ? [] : TypeParameters;
        return this with { TypeParameters = typeParams.Add(typeParameter) };
    }

    #endregion

    #region Interfaces

    /// <summary>
    /// Adds an interface to implement.
    /// </summary>
    /// <param name="interfaceName">The interface name (e.g., "IDisposable", "IEnumerable&lt;T&gt;").</param>
    public TypeBuilder Implements(string interfaceName)
    {
        if (string.IsNullOrWhiteSpace(interfaceName))
            throw new ArgumentException("Interface name cannot be null or empty.", nameof(interfaceName));

        var interfaces = Interfaces.IsDefault ? [] : Interfaces;
        return this with { Interfaces = interfaces.Add(new ConditionalInterface(interfaceName)) };
    }

    /// <summary>
    /// Adds a conditional interface to implement, wrapped in #if/#endif directives.
    /// </summary>
    /// <param name="interfaceName">The interface name (e.g., "ISpanFormattable", "IParsable&lt;T&gt;").</param>
    /// <param name="condition">The preprocessor directive condition (e.g., Directives.Net6OrGreater).</param>
    /// <example>
    /// <code>
    /// TypeBuilder.Struct("MyId")
    ///     .Implements("IEquatable&lt;MyId&gt;")
    ///     .Implements("ISpanFormattable", Directives.Net6OrGreater)
    ///     .Implements("IParsable&lt;MyId&gt;", Directives.Net7OrGreater);
    /// </code>
    /// </example>
    public TypeBuilder Implements(string interfaceName, Directive condition)
    {
        if (string.IsNullOrWhiteSpace(interfaceName))
            throw new ArgumentException("Interface name cannot be null or empty.", nameof(interfaceName));

        var interfaces = Interfaces.IsDefault ? [] : Interfaces;
        return this with { Interfaces = interfaces.Add(new ConditionalInterface(interfaceName, condition)) };
    }

    /// <summary>
    /// Adds multiple interfaces to implement.
    /// </summary>
    /// <param name="interfaceNames">The interface names to implement.</param>
    public TypeBuilder Implements(params string[] interfaceNames)
    {
        var builder = this;
        foreach (var name in interfaceNames) builder = builder.Implements(name);

        return builder;
    }

    #endregion

    #region Add Members - Properties

    /// <summary>
    /// Adds a property with lambda configuration.
    /// </summary>
    public TypeBuilder AddProperty(string name, string type, Func<PropertyBuilder, PropertyBuilder> configure)
    {
        var property = configure(PropertyBuilder.For(name, type));
        var properties = Properties.IsDefault ? [] : Properties;
        return this with { Properties = properties.Add(property) };
    }

    /// <summary>
    /// Adds a property using a symbol's globally qualified name as the type.
    /// </summary>
    public TypeBuilder AddProperty<T>(string name, ValidSymbol<T> type, Func<PropertyBuilder, PropertyBuilder> configure) where T : class, ITypeSymbol
        => AddProperty(name, type.GloballyQualifiedName, configure);

    /// <summary>
    /// Adds a pre-configured property.
    /// </summary>
    public TypeBuilder AddProperty(PropertyBuilder property)
    {
        var properties = Properties.IsDefault ? [] : Properties;
        return this with { Properties = properties.Add(property) };
    }

    #endregion

    #region Add Members - Fields

    /// <summary>
    /// Adds a field with lambda configuration.
    /// </summary>
    public TypeBuilder AddField(string name, string type, Func<FieldBuilder, FieldBuilder> configure)
    {
        var field = configure(FieldBuilder.For(name, type));
        var fields = Fields.IsDefault ? [] : Fields;
        return this with { Fields = fields.Add(field) };
    }

    /// <summary>
    /// Adds a field using a symbol's globally qualified name as the type.
    /// </summary>
    public TypeBuilder AddField<T>(string name, ValidSymbol<T> type, Func<FieldBuilder, FieldBuilder> configure) where T : class, ITypeSymbol
        => AddField(name, type.GloballyQualifiedName, configure);

    /// <summary>
    /// Adds a pre-configured field.
    /// </summary>
    public TypeBuilder AddField(FieldBuilder field)
    {
        var fields = Fields.IsDefault ? [] : Fields;
        return this with { Fields = fields.Add(field) };
    }

    #endregion

    #region Add Members - Events

    /// <summary>
    /// Adds an event with lambda configuration.
    /// </summary>
    /// <param name="name">The event name (e.g., "PropertyChanged", "Clicked").</param>
    /// <param name="type">The event handler type (e.g., "EventHandler", "PropertyChangedEventHandler?").</param>
    /// <param name="configure">Configuration callback for the event.</param>
    public TypeBuilder AddEvent(string name, string type, Func<EventBuilder, EventBuilder> configure)
    {
        var @event = configure(EventBuilder.For(name, type));
        var events = Events.IsDefault ? [] : Events;
        return this with { Events = events.Add(@event) };
    }

    /// <summary>
    /// Adds an event using a symbol's globally qualified name as the type.
    /// </summary>
    public TypeBuilder AddEvent<T>(string name, ValidSymbol<T> type, Func<EventBuilder, EventBuilder> configure) where T : class, ITypeSymbol
        => AddEvent(name, type.GloballyQualifiedName, configure);

    /// <summary>
    /// Adds a pre-configured event.
    /// </summary>
    public TypeBuilder AddEvent(EventBuilder @event)
    {
        var events = Events.IsDefault ? [] : Events;
        return this with { Events = events.Add(@event) };
    }

    #endregion

    #region Add Members - Methods

    /// <summary>
    /// Adds a method with lambda configuration.
    /// </summary>
    public TypeBuilder AddMethod(string name, Func<MethodBuilder, MethodBuilder> configure)
    {
        var method = configure(MethodBuilder.For(name));
        var methods = Methods.IsDefault ? [] : Methods;
        return this with { Methods = methods.Add(method) };
    }

    /// <summary>
    /// Adds a pre-configured method.
    /// </summary>
    public TypeBuilder AddMethod(MethodBuilder method)
    {
        var methods = Methods.IsDefault ? [] : Methods;
        return this with { Methods = methods.Add(method) };
    }

    #endregion

    #region Add Members - Conversion Operators

    /// <summary>
    /// Adds a pre-configured conversion operator.
    /// </summary>
    public TypeBuilder AddConversionOperator(ConversionOperatorBuilder op)
    {
        var conversionOperators = ConversionOperators.IsDefault ? [] : ConversionOperators;
        return this with { ConversionOperators = conversionOperators.Add(op) };
    }

    /// <summary>
    /// Adds a conversion operator with lambda configuration.
    /// Use ConversionOperatorBuilder.Explicit() or ConversionOperatorBuilder.Implicit() as the starting point.
    /// </summary>
    /// <param name="configure">Configuration callback for the conversion operator.</param>
    public TypeBuilder AddConversionOperator(Func<ConversionOperatorBuilder, ConversionOperatorBuilder> configure)
    {
        var op = configure(default);
        var conversionOperators = ConversionOperators.IsDefault ? [] : ConversionOperators;
        return this with { ConversionOperators = conversionOperators.Add(op) };
    }

    /// <summary>
    /// Adds an explicit conversion operator from the specified source type to this type.
    /// </summary>
    /// <param name="sourceType">The source type to convert from.</param>
    /// <param name="configure">Configuration callback for the conversion operator.</param>
    /// <param name="parameterName">The parameter name (default: "value").</param>
    public TypeBuilder AddExplicitConversion(string sourceType, Func<ConversionOperatorBuilder, ConversionOperatorBuilder> configure, string parameterName = "value")
    {
        var op = configure(ConversionOperatorBuilder.Explicit(Name, sourceType, parameterName));
        return AddConversionOperator(op);
    }

    /// <summary>
    /// Adds an explicit conversion operator from this type to the specified target type.
    /// </summary>
    /// <param name="targetType">The target type to convert to.</param>
    /// <param name="configure">Configuration callback for the conversion operator.</param>
    /// <param name="parameterName">The parameter name (default: "value").</param>
    public TypeBuilder AddExplicitConversionTo(string targetType, Func<ConversionOperatorBuilder, ConversionOperatorBuilder> configure, string parameterName = "value")
    {
        var op = configure(ConversionOperatorBuilder.Explicit(targetType, Name, parameterName));
        return AddConversionOperator(op);
    }

    /// <summary>
    /// Adds an implicit conversion operator from the specified source type to this type.
    /// </summary>
    /// <param name="sourceType">The source type to convert from.</param>
    /// <param name="configure">Configuration callback for the conversion operator.</param>
    /// <param name="parameterName">The parameter name (default: "value").</param>
    public TypeBuilder AddImplicitConversion(string sourceType, Func<ConversionOperatorBuilder, ConversionOperatorBuilder> configure, string parameterName = "value")
    {
        var op = configure(ConversionOperatorBuilder.Implicit(Name, sourceType, parameterName));
        return AddConversionOperator(op);
    }

    /// <summary>
    /// Adds an implicit conversion operator from this type to the specified target type.
    /// </summary>
    /// <param name="targetType">The target type to convert to.</param>
    /// <param name="configure">Configuration callback for the conversion operator.</param>
    /// <param name="parameterName">The parameter name (default: "value").</param>
    public TypeBuilder AddImplicitConversionTo(string targetType, Func<ConversionOperatorBuilder, ConversionOperatorBuilder> configure, string parameterName = "value")
    {
        var op = configure(ConversionOperatorBuilder.Implicit(targetType, Name, parameterName));
        return AddConversionOperator(op);
    }

    #endregion

    #region Add Members - Operators

    /// <summary>
    /// Adds a pre-configured operator.
    /// </summary>
    public TypeBuilder AddOperator(OperatorBuilder op)
    {
        var operators = Operators.IsDefault ? [] : Operators;
        return this with { Operators = operators.Add(op) };
    }

    /// <summary>
    /// Adds an operator with lambda configuration.
    /// </summary>
    public TypeBuilder AddOperator(Func<OperatorBuilder, OperatorBuilder> configure)
    {
        var op = configure(default);
        return AddOperator(op);
    }

    /// <summary>
    /// Adds an equality operator (==) for this type.
    /// </summary>
    /// <param name="expressionBody">The expression body (e.g., "left.Equals(right)").</param>
    public TypeBuilder AddEqualityOperator(string expressionBody)
    {
        return AddOperator(OperatorBuilder.Equality(Name).WithExpressionBody(expressionBody));
    }

    /// <summary>
    /// Adds an inequality operator (!=) for this type.
    /// </summary>
    /// <param name="expressionBody">The expression body (e.g., "!left.Equals(right)").</param>
    public TypeBuilder AddInequalityOperator(string expressionBody)
    {
        return AddOperator(OperatorBuilder.Inequality(Name).WithExpressionBody(expressionBody));
    }

    #endregion

    #region Add Members - Constructors

    /// <summary>
    /// Adds a constructor with lambda configuration.
    /// </summary>
    public TypeBuilder AddConstructor(Func<ConstructorBuilder, ConstructorBuilder> configure)
    {
        var constructor = configure(ConstructorBuilder.For(Name));
        var constructors = Constructors.IsDefault ? [] : Constructors;
        return this with { Constructors = constructors.Add(constructor) };
    }

    /// <summary>
    /// Adds a pre-configured constructor.
    /// </summary>
    public TypeBuilder AddConstructor(ConstructorBuilder constructor)
    {
        var constructors = Constructors.IsDefault ? [] : Constructors;
        return this with { Constructors = constructors.Add(constructor) };
    }

    /// <summary>
    /// Sets a primary constructor for the type.
    /// Primary constructors are declared in the type declaration itself (e.g., "class Person(string name)").
    /// </summary>
    /// <param name="configure">Configuration callback for the primary constructor.</param>
    public TypeBuilder WithPrimaryConstructor(Func<ConstructorBuilder, ConstructorBuilder> configure)
    {
        var constructor = configure(ConstructorBuilder.For(Name).AsPrimary());
        return this with { PrimaryConstructor = constructor };
    }

    /// <summary>
    /// Sets a pre-configured primary constructor for the type.
    /// Primary constructors are declared in the type declaration itself (e.g., "class Person(string name)").
    /// </summary>
    public TypeBuilder WithPrimaryConstructor(ConstructorBuilder constructor)
    {
        return this with { PrimaryConstructor = constructor.AsPrimary() };
    }

    #endregion

    #region Add Members - Nested Types

    /// <summary>
    /// Adds a nested type with lambda configuration.
    /// </summary>
    public TypeBuilder AddNestedType(string name, Func<TypeBuilder, TypeBuilder> configure)
    {
        var nestedType = configure(Class(name));
        var nestedTypes = NestedTypes.IsDefault ? [] : NestedTypes;
        return this with { NestedTypes = nestedTypes.Add(nestedType) };
    }

    /// <summary>
    /// Adds a pre-configured nested type.
    /// </summary>
    public TypeBuilder AddNestedType(TypeBuilder nestedType)
    {
        var nestedTypes = NestedTypes.IsDefault ? [] : NestedTypes;
        return this with { NestedTypes = nestedTypes.Add(nestedType) };
    }

    #endregion

    #region XML Documentation

    /// <summary>
    /// Sets the XML documentation for the type.
    /// </summary>
    /// <param name="configure">Configuration callback for the XML documentation.</param>
    public TypeBuilder WithXmlDoc(Func<XmlDocumentationBuilder, XmlDocumentationBuilder> configure)
    {
        var xmlDoc = configure(XmlDocumentationBuilder.Create());
        return this with { XmlDoc = xmlDoc };
    }

    /// <summary>
    /// Sets the XML documentation for the type with a simple summary.
    /// </summary>
    /// <param name="summary">The summary text.</param>
    public TypeBuilder WithXmlDoc(string summary)
    {
        var xmlDoc = XmlDocumentationBuilder.ForSummary(summary);
        return this with { XmlDoc = xmlDoc };
    }

    /// <summary>
    /// Sets the XML documentation for the type from a pipeline-safe <see cref="DocumentationSnapshot"/>.
    /// </summary>
    /// <param name="snapshot">The documentation snapshot to copy.</param>
    public TypeBuilder WithXmlDoc(DocumentationSnapshot snapshot)
    {
        if (!snapshot.HasValue && snapshot.Params.Count == 0 && snapshot.TypeParams.Count == 0)
            return this;

        var xmlDoc = XmlDocumentationBuilder.From(snapshot);
        return this with { XmlDoc = xmlDoc };
    }

    #endregion

    #region Attributes

    /// <summary>
    /// Adds an attribute to the type.
    /// </summary>
    /// <param name="name">The attribute name (e.g., "Serializable", "Obsolete").</param>
    public TypeBuilder WithAttribute(string name)
    {
        var attribute = AttributeBuilder.For(name);
        var attributes = Attributes.IsDefault ? [] : Attributes;
        return this with { Attributes = attributes.Add(attribute) };
    }

    /// <summary>
    /// Adds an attribute to the type with configuration.
    /// </summary>
    /// <param name="name">The attribute name (e.g., "Obsolete", "JsonConverter").</param>
    /// <param name="configure">Configuration callback for the attribute.</param>
    public TypeBuilder WithAttribute(string name, Func<AttributeBuilder, AttributeBuilder> configure)
    {
        var attribute = configure(AttributeBuilder.For(name));
        var attributes = Attributes.IsDefault ? [] : Attributes;
        return this with { Attributes = attributes.Add(attribute) };
    }

    /// <summary>
    /// Adds a pre-configured attribute to the type.
    /// </summary>
    public TypeBuilder WithAttribute(AttributeBuilder attribute)
    {
        var attributes = Attributes.IsDefault ? [] : Attributes;
        return this with { Attributes = attributes.Add(attribute) };
    }

    #endregion

    #region Metadata

    /// <summary>
    /// Attaches a metadata entry to this builder. Metadata does not affect code generation.
    /// </summary>
    /// <param name="key">The metadata key.</param>
    /// <param name="value">The metadata value.</param>
    public TypeBuilder WithMetadata(string key, object? value) =>
        this with { Metadata = (Metadata ?? ImmutableDictionary<string, object?>.Empty).SetItem(key, value) };

    /// <summary>
    /// Retrieves a metadata entry by key.
    /// </summary>
    /// <typeparam name="T">The expected metadata type.</typeparam>
    /// <param name="key">The metadata key.</param>
    /// <returns>The metadata value cast to <typeparamref name="T"/>.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the key is not found.</exception>
    /// <exception cref="InvalidCastException">Thrown when the value is not of the expected type.</exception>
    public T GetMetadata<T>(string key) where T : class =>
        (T)(Metadata ?? throw new KeyNotFoundException($"Metadata key '{key}' not found."))[key]!;

    #endregion

    #region Region Grouping

    /// <summary>
    /// Adds members to a named region. All members added within the configure action
    /// are automatically tagged with the specified region name.
    /// </summary>
    /// <param name="regionName">The region name (e.g., "Properties", "Helpers").</param>
    /// <param name="configure">Action to add members that will be grouped in the region.</param>
    /// <example>
    /// <code>
    /// TypeBuilder.Class("Customer")
    ///     .AddRegion("Properties", r => r
    ///         .AddProperty("Name", "string", p => p.WithAutoPropertyAccessors())
    ///         .AddProperty("Age", "int", p => p.WithAutoPropertyAccessors()))
    ///     .AddRegion("Methods", r => r
    ///         .AddMethod("ToString", m => m.WithExpressionBody("Name")));
    /// </code>
    /// </example>
    public TypeBuilder AddRegion(string regionName, Func<TypeBuilder, TypeBuilder> configure)
    {
        if (string.IsNullOrWhiteSpace(regionName))
            throw new ArgumentException("Region name cannot be null or empty.", nameof(regionName));

        // Snapshot current member counts
        var snapshot = this;
        var result = configure(this);

        // Tag all newly added members with the region name
        result = TagNewMembers(snapshot, result, regionName);

        return result;
    }

    /// <summary>
    /// Tags any members added between snapshot and current with the given region name.
    /// </summary>
    private static TypeBuilder TagNewMembers(TypeBuilder snapshot, TypeBuilder current, string regionName)
    {
        current = TagNew(snapshot.Fields, current.Fields,
            (f, r) => f with { Region = r }, regionName,
            tagged => current with { Fields = tagged });

        current = TagNew(snapshot.Events, current.Events,
            (e, r) => e with { Region = r }, regionName,
            tagged => current with { Events = tagged });

        current = TagNew(snapshot.Constructors, current.Constructors,
            (c, r) => c with { Region = r }, regionName,
            tagged => current with { Constructors = tagged });

        current = TagNew(snapshot.Properties, current.Properties,
            (p, r) => p with { Region = r }, regionName,
            tagged => current with { Properties = tagged });

        current = TagNew(snapshot.Methods, current.Methods,
            (m, r) => m with { Region = r }, regionName,
            tagged => current with { Methods = tagged });

        current = TagNew(snapshot.Operators, current.Operators,
            (o, r) => o with { Region = r }, regionName,
            tagged => current with { Operators = tagged });

        current = TagNew(snapshot.ConversionOperators, current.ConversionOperators,
            (c, r) => c with { Region = r }, regionName,
            tagged => current with { ConversionOperators = tagged });

        return current;
    }

    private static TypeBuilder TagNew<T>(
        ImmutableArray<T> snapshotArray,
        ImmutableArray<T> currentArray,
        Func<T, string, T> setRegion,
        string regionName,
        Func<ImmutableArray<T>, TypeBuilder> apply)
    {
        var oldCount = snapshotArray.IsDefault ? 0 : snapshotArray.Length;
        var newCount = currentArray.IsDefault ? 0 : currentArray.Length;

        if (newCount <= oldCount)
            return apply(currentArray);

        var builder = currentArray.ToBuilder();
        for (var i = oldCount; i < newCount; i++)
            builder[i] = setRegion(builder[i], regionName);

        return apply(builder.ToImmutable());
    }

    #endregion

    #region Emit

    /// <summary>
    /// Emits the type to a compilation unit with default options.
    /// Uses syntax validation by default.
    /// </summary>
    public OptionalEmit Emit()
    {
        return Emit(EmitOptions.Default);
    }

    /// <summary>
    /// Emits the type to a compilation unit with specified options.
    /// </summary>
    public OptionalEmit Emit(EmitOptions options)
    {
        try
        {
            var compilationUnit = BuildCompilationUnit(options);

            // Format the code
            var formatted = FormatCode(compilationUnit, options);

            // Insert preprocessor directives for conditional interfaces
            if (HasConditionalInterfaces)
            {
                formatted = InsertInterfaceDirectives(formatted);

                // Skip validation when directives are present (would fail due to #if)
                return OptionalEmit.FromSuccess(compilationUnit, formatted);
            }

            // Validate if requested
            if (options.ValidationLevel != ValidationLevel.None)
            {
                var diagnostics = Validate(formatted, options.ValidationLevel);
                if (diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error))
                    return OptionalEmit.FromDiagnostics(
                        compilationUnit, formatted, WrapValidationErrors(diagnostics, formatted));

                return diagnostics.Any()
                    ? OptionalEmit.FromDiagnostics(compilationUnit, formatted, diagnostics)
                    : OptionalEmit.FromSuccess(compilationUnit, formatted);
            }

            return OptionalEmit.FromSuccess(compilationUnit, formatted);
        }
        catch (Exception ex)
        {
            return OptionalEmit.FromFailure([
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "EMIT001",
                        "Emit failed",
                        $"Failed to emit type: {ex.Message}",
                        "Emit",
                        DiagnosticSeverity.Error,
                        true),
                    Location.None)
            ]);
        }
    }

    private CompilationUnitSyntax BuildCompilationUnit(EmitOptions options)
    {
        var compilationUnit = SyntaxFactory.CompilationUnit();

        // Collect all usings including from nested types, sorted alphabetically
        var allUsings = CollectAllUsings()
            .Distinct()
            .OrderBy(u => u, StringComparer.Ordinal);

        // Add usings
        foreach (var @using in allUsings)
        {
            UsingDirectiveSyntax usingDirective;

            if (@using.StartsWith("static ", StringComparison.Ordinal))
            {
                // Static using: "static LanguageExt.Prelude" -> using static LanguageExt.Prelude;
                var typeName = @using.Substring(7); // Remove "static " prefix
                usingDirective = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(typeName))
                    .WithStaticKeyword(SyntaxFactory.Token(SyntaxKind.StaticKeyword));
            }
            else
            {
                usingDirective = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(@using));
            }

            compilationUnit = compilationUnit.AddUsings(usingDirective);
        }

        // Build type declaration
        var typeDecl = BuildTypeDeclaration(options);

        // Wrap in namespace if specified
        if (!string.IsNullOrWhiteSpace(Namespace))
        {
            var namespaceDecl = SyntaxFactory.FileScopedNamespaceDeclaration(
                    SyntaxFactory.ParseName(Namespace!))
                .AddMembers(typeDecl);

            compilationUnit = compilationUnit.AddMembers(namespaceDecl);
        }
        else
        {
            compilationUnit = compilationUnit.AddMembers(typeDecl);
        }

        // Add header comment and nullable enable directive to the first token
        var headerComment = SyntaxFactory.Comment(options.HeaderComment);
        var nullableDirective = SyntaxFactory.Comment("#nullable enable");
        var newLine = SyntaxFactory.EndOfLine(Environment.NewLine);
        var firstToken = compilationUnit.GetFirstToken();

        // Build trivia list: header, newline, license (if present), nullable, newline
        var triviaList = new List<SyntaxTrivia> { headerComment, newLine };

        if (!string.IsNullOrWhiteSpace(options.LicenseHeader))
            // Add each line of the license header as a separate comment trivia
            foreach (var line in options.LicenseHeader!.Split('\n'))
            {
                var trimmedLine = line.TrimStart();
                if (!string.IsNullOrEmpty(trimmedLine))
                {
                    triviaList.Add(SyntaxFactory.Comment(trimmedLine));
                    triviaList.Add(newLine);
                }
            }

        triviaList.Add(nullableDirective);
        triviaList.Add(newLine);

        var newFirstToken = firstToken.WithLeadingTrivia(
            SyntaxFactory.TriviaList(triviaList)
                .AddRange(firstToken.LeadingTrivia));
        compilationUnit = compilationUnit.ReplaceToken(firstToken, newFirstToken);

        return compilationUnit;
    }

    private TypeDeclarationSyntax BuildTypeDeclaration(EmitOptions? options = null)
    {
        TypeDeclarationSyntax typeDecl = IsRecord
            ? SyntaxFactory.RecordDeclaration(SyntaxFactory.Token(SyntaxKind.RecordKeyword), Name)
            : Kind switch
            {
                TypeKind.Class => SyntaxFactory.ClassDeclaration(Name),
                TypeKind.Interface => SyntaxFactory.InterfaceDeclaration(Name),
                TypeKind.Struct => SyntaxFactory.StructDeclaration(Name),
                _ => throw new InvalidOperationException($"Unsupported type kind: {Kind}")
            };

        // Records can use semicolon termination for types without members
        // (or with only a primary constructor). Other types get braces automatically.
        if (IsRecord)
        {
            var hasMembers = HasAnyMembers();
            if (hasMembers)
            {
                typeDecl = typeDecl
                    .WithOpenBraceToken(SyntaxFactory.Token(SyntaxKind.OpenBraceToken))
                    .WithCloseBraceToken(SyntaxFactory.Token(SyntaxKind.CloseBraceToken));
            }
            else
            {
                typeDecl = typeDecl.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
            }
        }

        // Add attributes
        var attributes = Attributes.IsDefault ? [] : Attributes;
        if (attributes.Length > 0)
        {
            var attributeLists = attributes.Select(a => a.BuildList()).ToArray();
            typeDecl = typeDecl.WithAttributeLists(SyntaxFactory.List(attributeLists));
        }

        // Add modifiers
        var modifiers = new List<SyntaxKind>();
        modifiers.Add(AccessibilityToSyntaxKind(Accessibility));
        if (IsStatic) modifiers.Add(SyntaxKind.StaticKeyword);
        if (IsAbstract) modifiers.Add(SyntaxKind.AbstractKeyword);
        if (IsSealed) modifiers.Add(SyntaxKind.SealedKeyword);
        if (IsPartial) modifiers.Add(SyntaxKind.PartialKeyword);

        typeDecl = typeDecl.WithModifiers(
            SyntaxFactory.TokenList(modifiers.Select(SyntaxFactory.Token)));

        // Add type parameters
        var typeParameters = TypeParameters.IsDefault ? [] : TypeParameters;
        if (typeParameters.Length > 0)
        {
            var typeParameterList = SyntaxFactory.TypeParameterList(
                SyntaxFactory.SeparatedList(typeParameters.Select(tp => tp.Build())));
            typeDecl = typeDecl.WithTypeParameterList(typeParameterList);

            // Add constraint clauses
            var constraintClauses = typeParameters
                .Select(tp => tp.BuildConstraintClause())
                .Where(c => c != null)
                .Cast<TypeParameterConstraintClauseSyntax>()
                .ToList();

            if (constraintClauses.Count > 0)
                typeDecl = typeDecl.WithConstraintClauses(SyntaxFactory.List(constraintClauses));
        }

        // Add primary constructor parameter list
        if (PrimaryConstructor.HasValue)
        {
            var parameters = PrimaryConstructor.Value.Parameters;
            var normalizedParams = parameters.IsDefault ? [] : parameters;
            var parameterList = SyntaxFactory.ParameterList(
                SyntaxFactory.SeparatedList(normalizedParams.Select(p => p.Build())));
            typeDecl = typeDecl.WithParameterList(parameterList);
        }

        // Add base list (unconditional interfaces only - conditional ones added via post-processing)
        var interfaces = Interfaces.IsDefault ? [] : Interfaces;
        if (interfaces.Length > 0)
        {
            var baseList = BuildBaseListWithDirectives(interfaces);
            if (baseList != null)
            {
                typeDecl = typeDecl.WithBaseList(baseList);
            }
        }

        // Add members in logical order, with optional region grouping
        typeDecl = AddMembersWithRegions(typeDecl, options);

        // Add XML documentation
        if (XmlDoc.HasValue && XmlDoc.Value.HasContent)
        {
            var trivia = XmlDoc.Value.Build();
            typeDecl = typeDecl.WithLeadingTrivia(trivia);
        }

        return typeDecl;
    }

    /// <summary>
    /// Builds the type as a nested type declaration (no namespace wrapper).
    /// </summary>
    internal TypeDeclarationSyntax BuildNestedTypeDeclaration()
    {
        return BuildTypeDeclaration();
    }

    /// <summary>
    /// Adds members to the type declaration, applying #region/#endregion grouping as needed.
    /// Members with explicit Region tags are grouped under their named region.
    /// When UseRegions is enabled, remaining untagged members are grouped by category.
    /// </summary>
    private TypeDeclarationSyntax AddMembersWithRegions(
        TypeDeclarationSyntax typeDecl,
        EmitOptions? options)
    {
        var useRegions = options?.UseRegions ?? false;

        var categories = CollectMemberCategories();

        // If no regions needed, add members directly
        if (!useRegions && !HasAnyExplicitRegions(categories))
        {
            foreach (var (_, _, members) in categories)
            {
                foreach (var member in members)
                    typeDecl = typeDecl.AddMembers(member);
            }

            return typeDecl;
        }

        // Collect all members with their region assignments
        var regionGroups = new List<(string regionName, List<MemberDeclarationSyntax> members)>();
        var currentRegion = (string?)null;
        List<MemberDeclarationSyntax>? currentMembers = null;

        foreach (var (categoryName, explicitRegion, members) in categories)
        {
            foreach (var member in members)
            {
                var regionName = explicitRegion ?? (useRegions ? categoryName : null);

                if (regionName != currentRegion)
                {
                    if (currentMembers != null && currentRegion != null)
                        regionGroups.Add((currentRegion, currentMembers));
                    else if (currentMembers != null)
                        regionGroups.Add(("", currentMembers));

                    currentRegion = regionName;
                    currentMembers = [];
                }

                currentMembers ??= [];
                currentMembers.Add(member);
            }
        }

        // Flush last group
        if (currentMembers is { Count: > 0 })
        {
            if (currentRegion != null)
                regionGroups.Add((currentRegion, currentMembers));
            else
                regionGroups.Add(("", currentMembers));
        }

        // Emit each group, wrapping in regions where appropriate
        foreach (var (regionName, groupMembers) in regionGroups)
        {
            if (!string.IsNullOrEmpty(regionName))
            {
                var wrapped = RegionHelper.WrapMembersInRegion(
                    [..groupMembers], regionName);
                typeDecl = typeDecl.AddMembers(wrapped);
            }
            else
            {
                foreach (var member in groupMembers)
                    typeDecl = typeDecl.AddMembers(member);
            }
        }

        return typeDecl;
    }

    /// <summary>
    /// Collects all members organized by category, in the standard emission order.
    /// Each entry is (categoryName, explicitRegion, builtMembers).
    /// </summary>
    private List<(string Category, string? ExplicitRegion, MemberDeclarationSyntax[] Members)>
        CollectMemberCategories()
    {
        var categories =
            new List<(string Category, string? ExplicitRegion, MemberDeclarationSyntax[] Members)>();

        var fields = Fields.IsDefault ? [] : Fields;
        AddCategory(categories, fields, "Fields",
            f => f.Region, f => f.Build());

        var events = Events.IsDefault ? [] : Events;
        AddCategory(categories, events, "Events",
            e => e.Region, e => e.Build());

        var constructors = Constructors.IsDefault ? [] : Constructors;
        AddCategory(categories, constructors, "Constructors",
            c => c.Region, c => c.Build());

        var properties = Properties.IsDefault ? [] : Properties;
        AddCategory(categories, properties, "Properties",
            p => p.Region, p => p.Build());

        var methods = Methods.IsDefault ? [] : Methods;
        AddCategory(categories, methods, "Methods",
            m => m.Region, m => m.Build());

        var operators = Operators.IsDefault ? [] : Operators;
        AddCategory(categories, operators, "Operators",
            o => o.Region, o => o.Build());

        var conversionOperators = ConversionOperators.IsDefault ? [] : ConversionOperators;
        AddCategory(categories, conversionOperators, "Conversion Operators",
            c => c.Region, c => c.Build());

        var nestedTypes = NestedTypes.IsDefault ? [] : NestedTypes;
        if (nestedTypes.Length > 0)
        {
            categories.Add(("Nested Types", null,
                nestedTypes.Select(t => (MemberDeclarationSyntax)t.BuildNestedTypeDeclaration())
                    .ToArray()));
        }

        return categories;
    }

    private static void AddCategory<T>(
        List<(string Category, string? ExplicitRegion, MemberDeclarationSyntax[] Members)> categories,
        ImmutableArray<T> builders,
        string categoryName,
        Func<T, string?> getRegion,
        Func<T, MemberDeclarationSyntax> build)
    {
        if (builders.Length == 0) return;

        // Group consecutive builders by their explicit region
        string? currentRegion = null;
        var currentGroup = new List<MemberDeclarationSyntax>();
        var isFirst = true;

        foreach (var builder in builders)
        {
            var region = getRegion(builder);

            if (isFirst)
            {
                currentRegion = region;
                isFirst = false;
            }
            else if (region != currentRegion)
            {
                if (currentGroup.Count > 0)
                    categories.Add((categoryName, currentRegion, [..currentGroup]));
                currentGroup = [];
                currentRegion = region;
            }

            currentGroup.Add(build(builder));
        }

        if (currentGroup.Count > 0)
            categories.Add((categoryName, currentRegion, [..currentGroup]));
    }

    private static bool HasAnyExplicitRegions(
        List<(string Category, string? ExplicitRegion, MemberDeclarationSyntax[] Members)> categories)
    {
        return categories.Any(c => c.ExplicitRegion != null);
    }

    /// <summary>
    /// Checks if the type has any members (fields, properties, methods, etc.) beyond a primary constructor.
    /// </summary>
    private bool HasAnyMembers()
    {
        var fields = Fields.IsDefault ? [] : Fields;
        var properties = Properties.IsDefault ? [] : Properties;
        var methods = Methods.IsDefault ? [] : Methods;
        var constructors = Constructors.IsDefault ? [] : Constructors;
        var events = Events.IsDefault ? [] : Events;
        var operators = Operators.IsDefault ? [] : Operators;
        var conversionOperators = ConversionOperators.IsDefault ? [] : ConversionOperators;
        var nestedTypes = NestedTypes.IsDefault ? [] : NestedTypes;

        return fields.Length > 0 ||
               properties.Length > 0 ||
               methods.Length > 0 ||
               constructors.Length > 0 ||
               events.Length > 0 ||
               operators.Length > 0 ||
               conversionOperators.Length > 0 ||
               nestedTypes.Length > 0;
    }

    /// <summary>
    /// Builds the base list with only unconditional interfaces.
    /// Conditional interfaces are added via post-processing in InsertInterfaceDirectives.
    /// </summary>
    private static BaseListSyntax? BuildBaseListWithDirectives(ImmutableArray<ConditionalInterface> interfaces)
    {
        // Only include unconditional interfaces in the syntax tree
        var unconditional = interfaces.Where(i => !i.Condition.HasValue).ToArray();
        if (unconditional.Length == 0)
            return null;

        var baseTypes = unconditional
            .Select(i => SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(i.Name)))
            .ToArray<BaseTypeSyntax>();

        return SyntaxFactory.BaseList(SyntaxFactory.SeparatedList(baseTypes));
    }

    /// <summary>
    /// Gets whether this type has any conditional interfaces that need directive wrapping.
    /// </summary>
    private bool HasConditionalInterfaces =>
        !Interfaces.IsDefault && Interfaces.Any(i => i.Condition.HasValue);

    /// <summary>
    /// Post-processes the formatted code to insert #if/#endif directives for conditional interfaces.
    /// Inserts them after the unconditional interfaces with proper comma placement.
    /// </summary>
    private string InsertInterfaceDirectives(string code)
    {
        var interfaces = Interfaces.IsDefault ? [] : Interfaces;
        var conditional = interfaces.Where(i => i.Condition.HasValue).ToList();
        if (conditional.Count == 0)
            return code;

        var unconditional = interfaces.Where(i => !i.Condition.HasValue).ToList();

        // Group conditional interfaces by directive condition, preserving order
        var groupedConditional = conditional
            .GroupBy(i => i.Condition!.Value.Condition)
            .ToList();

        var lines = code.Split('\n').ToList();

        // Find the line containing the type declaration with base list
        // Pattern: "struct TypeName : Interface1, Interface2" or just "struct TypeName"
        var typeDeclarationIndex = -1;
        var baseListEndIndex = -1;

        for (var i = 0; i < lines.Count; i++)
        {
            var line = lines[i];
            var trimmed = line.Trim();

            // Look for struct/class/record declaration
            if ((trimmed.Contains("struct ") || trimmed.Contains("class ") || trimmed.Contains("record ")) &&
                !trimmed.StartsWith("//") && !trimmed.StartsWith("*"))
            {
                typeDeclarationIndex = i;

                // Check if this line or subsequent lines contain the base list
                if (trimmed.Contains(":"))
                {
                    // Base list starts on this line - find where it ends (the opening brace)
                    for (var j = i; j < lines.Count; j++)
                    {
                        if (lines[j].Contains("{"))
                        {
                            baseListEndIndex = j;
                            break;
                        }
                    }
                }
                else
                {
                    // No base list yet - the opening brace should be on this or next line
                    baseListEndIndex = i;
                    for (var j = i; j < lines.Count; j++)
                    {
                        if (lines[j].Contains("{"))
                        {
                            baseListEndIndex = j;
                            break;
                        }
                    }
                }
                break;
            }
        }

        if (typeDeclarationIndex == -1)
            return code;

        // Build the conditional interface blocks
        var conditionalBlocks = new StringBuilder();
        foreach (var group in groupedConditional)
        {
            var condition = group.Key;
            var interfaceNames = group.Select(i => i.Name).ToList();

            conditionalBlocks.AppendLine($"#if {condition}");
            foreach (var interfaceName in interfaceNames)
            {
                // Comma goes BEFORE the interface name (continuing the list)
                conditionalBlocks.AppendLine($"        , {interfaceName}");
            }
            conditionalBlocks.AppendLine("#endif");
        }

        // Find insertion point - just before the opening brace
        if (baseListEndIndex >= 0 && baseListEndIndex < lines.Count)
        {
            var braceLine = lines[baseListEndIndex];
            var braceIndex = braceLine.IndexOf('{');

            if (braceIndex >= 0)
            {
                // If unconditional interfaces exist, we need to ensure no trailing comma
                // and insert conditional blocks before the brace
                if (unconditional.Count > 0)
                {
                    // Find and fix the line with the last unconditional interface
                    for (var i = baseListEndIndex; i >= typeDeclarationIndex; i--)
                    {
                        var line = lines[i];
                        // Check if this line has an interface (contains the last unconditional interface name)
                        var lastUnconditional = unconditional.Last().Name;
                        if (line.Contains(lastUnconditional))
                        {
                            // Remove trailing comma if present (conditional interfaces will add their own commas)
                            var trimmedLine = line.TrimEnd();
                            if (trimmedLine.EndsWith(","))
                            {
                                lines[i] = line.TrimEnd().TrimEnd(',');
                            }
                            break;
                        }
                    }
                }
                else
                {
                    // No unconditional interfaces - we need to add the colon
                    // Find the type declaration line and add ":"
                    var declLine = lines[typeDeclarationIndex];
                    if (!declLine.Contains(":"))
                    {
                        // Insert colon before conditional blocks
                        conditionalBlocks.Insert(0, "    :\n");
                    }
                }

                // Insert conditional blocks before the opening brace
                var beforeBrace = braceLine.Substring(0, braceIndex);
                var afterBrace = braceLine.Substring(braceIndex);
                lines[baseListEndIndex] = beforeBrace.TrimEnd() + "\n" + conditionalBlocks.ToString().TrimEnd() + "\n" + afterBrace.TrimStart();
            }
        }

        return string.Join("\n", lines);
    }

    /// <summary>
    /// Collects all using directives from this type and all nested types recursively.
    /// </summary>
    private IEnumerable<string> CollectAllUsings()
    {
        // Type-level usings
        var usings = Usings.IsDefault ? [] : Usings;
        foreach (var @using in usings)
            yield return @using;

        // Method usings
        var methods = Methods.IsDefault ? [] : Methods;
        foreach (var method in methods)
        {
            var methodUsings = method.Usings.IsDefault ? [] : method.Usings;
            foreach (var @using in methodUsings)
                yield return @using;
        }

        // Conversion operator usings
        var conversionOperators = ConversionOperators.IsDefault ? [] : ConversionOperators;
        foreach (var convOp in conversionOperators)
        {
            var convOpUsings = convOp.Usings.IsDefault ? [] : convOp.Usings;
            foreach (var @using in convOpUsings)
                yield return @using;
        }

        // Operator usings
        var operators = Operators.IsDefault ? [] : Operators;
        foreach (var op in operators)
        {
            var opUsings = op.Usings.IsDefault ? [] : op.Usings;
            foreach (var @using in opUsings)
                yield return @using;
        }

        // Property usings
        var properties = Properties.IsDefault ? [] : Properties;
        foreach (var property in properties)
        {
            var propUsings = property.Usings.IsDefault ? [] : property.Usings;
            foreach (var @using in propUsings)
                yield return @using;
        }

        // Field usings
        var fields = Fields.IsDefault ? [] : Fields;
        foreach (var field in fields)
        {
            var fieldUsings = field.Usings.IsDefault ? [] : field.Usings;
            foreach (var @using in fieldUsings)
                yield return @using;
        }

        // Event usings
        var events = Events.IsDefault ? [] : Events;
        foreach (var @event in events)
        {
            var eventUsings = @event.Usings.IsDefault ? [] : @event.Usings;
            foreach (var @using in eventUsings)
                yield return @using;
        }

        // Constructor usings
        var constructors = Constructors.IsDefault ? [] : Constructors;
        foreach (var constructor in constructors)
        {
            var ctorUsings = constructor.Usings.IsDefault ? [] : constructor.Usings;
            foreach (var @using in ctorUsings)
                yield return @using;
        }

        // Attribute usings (on the type itself)
        var attributes = Attributes.IsDefault ? [] : Attributes;
        foreach (var attribute in attributes)
        {
            var attrUsings = attribute.Usings.IsDefault ? [] : attribute.Usings;
            foreach (var @using in attrUsings)
                yield return @using;
        }

        // Nested types (recursive)
        var nestedTypes = NestedTypes.IsDefault ? [] : NestedTypes;
        foreach (var nestedType in nestedTypes)
        foreach (var @using in nestedType.CollectAllUsings())
            yield return @using;
    }

    private string FormatCode(CompilationUnitSyntax compilationUnit, EmitOptions options)
    {
        // First normalize whitespace for proper indentation
        var normalized = compilationUnit.NormalizeWhitespace(options.Indentation, options.EndOfLine);
        return normalized.ToFullString();
    }

    private ImmutableArray<Diagnostic> Validate(string code, ValidationLevel level)
    {
        if (level == ValidationLevel.None)
            return ImmutableArray<Diagnostic>.Empty;

        if (level == ValidationLevel.Syntax)
        {
            var parseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest);
            return [..CSharpSyntaxTree.ParseText(code, parseOptions).GetDiagnostics()];
        }

        // Semantic and Full validation require Compilation (Phase 2)
        // For now, return empty diagnostics
        return ImmutableArray<Diagnostic>.Empty;
    }

    private static readonly DiagnosticDescriptor EmitValidationFailed = new(
#pragma warning disable RS2008 // Emit diagnostics don't need analyzer release tracking
        "EMIT002",
#pragma warning restore RS2008
        "Generated code failed syntax validation",
        "TypeBuilder '{0}' produced invalid C# with {1} syntax error(s). First error: {2}. Code preview: {3}.",
        "Deepstaging.Emit",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The Emit API produced C# code that could not be parsed. This typically indicates a bug in the code generation logic.");

    private ImmutableArray<Diagnostic> WrapValidationErrors(
        ImmutableArray<Diagnostic> rawDiagnostics, string code)
    {
        var errors = rawDiagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
        if (errors.Count == 0) return rawDiagnostics;

        var firstError = errors[0].GetMessage();
        var codePreview = string.Join(" | ",
            code.Split('\n').Where(l => !string.IsNullOrWhiteSpace(l)).Take(5).Select(l => l.Trim()));

        var summary = Diagnostic.Create(
            EmitValidationFailed,
            Location.None,
            Name,
            errors.Count,
            firstError,
            codePreview);

        return [summary, ..rawDiagnostics];
    }

    private static SyntaxKind AccessibilityToSyntaxKind(Accessibility accessibility)
    {
        return accessibility switch
        {
            Accessibility.Public => SyntaxKind.PublicKeyword,
            Accessibility.Private => SyntaxKind.PrivateKeyword,
            Accessibility.Protected => SyntaxKind.ProtectedKeyword,
            Accessibility.Internal => SyntaxKind.InternalKeyword,
            _ => SyntaxKind.PublicKeyword
        };
    }

    #endregion
}