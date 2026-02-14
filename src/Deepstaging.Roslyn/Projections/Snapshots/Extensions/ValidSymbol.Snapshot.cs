// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Access = Microsoft.CodeAnalysis.Accessibility;

namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for creating snapshots from <see cref="ValidSymbol{TSymbol}"/> types.
/// </summary>
public static class ValidSymbolSnapshotExtensions
{
    extension<TSymbol>(ValidSymbol<TSymbol> symbol) where TSymbol : class, ISymbol
    {
        /// <summary>
        /// Creates the base <see cref="SymbolSnapshot"/> properties shared across all symbol types.
        /// </summary>
        internal SymbolSnapshot ToBaseSnapshot() => new()
        {
            Name = symbol.Name,
            Namespace = symbol.Namespace,
            FullyQualifiedName = symbol.FullyQualifiedName,
            GloballyQualifiedName = symbol.GloballyQualifiedName,
            PropertyName = symbol.PropertyName,
            ParameterName = symbol.ParameterName,
            AccessibilityString = symbol.AccessibilityString,
            IsPublic = symbol.IsPublic,
            IsInternal = symbol.IsInternal,
            IsStatic = symbol.IsStatic,
            IsAbstract = symbol.IsAbstract,
            IsSealed = symbol.IsSealed,
            IsVirtual = symbol.IsVirtual,
            IsOverride = symbol.IsOverride,
            IsReadOnly = symbol.IsReadOnly,
            IsPartial = symbol.IsPartial,
            Kind = symbol.Kind,
            IsValueType = symbol.IsValueType,
            IsReferenceType = symbol.IsReferenceType,
            IsNullable = symbol.IsNullable,
            Documentation = symbol.XmlDocumentation.ToSnapshot(),
        };
    }

    extension(ValidSymbol<INamedTypeSymbol> symbol)
    {
        /// <summary>
        /// Creates a pipeline-safe <see cref="TypeSnapshot"/> from this named type symbol.
        /// </summary>
        public TypeSnapshot ToSnapshot() => new()
        {
            // Base properties
            Name = symbol.Name,
            Namespace = symbol.Namespace,
            FullyQualifiedName = symbol.FullyQualifiedName,
            GloballyQualifiedName = symbol.GloballyQualifiedName,
            PropertyName = symbol.PropertyName,
            ParameterName = symbol.ParameterName,
            AccessibilityString = symbol.AccessibilityString,
            IsPublic = symbol.IsPublic,
            IsInternal = symbol.IsInternal,
            IsStatic = symbol.IsStatic,
            IsAbstract = symbol.IsAbstract,
            IsSealed = symbol.IsSealed,
            IsVirtual = symbol.IsVirtual,
            IsOverride = symbol.IsOverride,
            IsReadOnly = symbol.IsReadOnly,
            IsPartial = symbol.IsPartial,
            Kind = symbol.Kind,
            IsValueType = symbol.IsValueType,
            IsReferenceType = symbol.IsReferenceType,
            IsNullable = symbol.IsNullable,
            Documentation = symbol.XmlDocumentation.ToSnapshot(),

            // Type-specific properties
            IsInterface = symbol.IsInterface,
            IsClass = symbol.IsClass,
            IsStruct = symbol.IsStruct,
            IsRecord = symbol.IsRecord,
            IsEnum = symbol.IsEnum,
            IsDelegate = symbol.IsDelegate,
            IsGenericType = symbol.IsGenericType,
            Arity = symbol.Arity,
            TypeArgumentNames = symbol.Value.IsGenericType
                ? symbol.Value.TypeArguments
                    .Select(t => t.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
                    .ToEquatableArray()
                : [],
            BaseTypeName = symbol.BaseType.OrNull() is { } bt
                ? ValidSymbol<INamedTypeSymbol>.From(bt).GloballyQualifiedName
                : null,
            InterfaceNames = symbol.Interfaces
                .Select(i => i.GloballyQualifiedName)
                .ToEquatableArray(),
        };
    }

    extension(ValidSymbol<IMethodSymbol> method)
    {
        /// <summary>
        /// Creates a pipeline-safe <see cref="MethodSnapshot"/> from this method symbol.
        /// </summary>
        public MethodSnapshot ToSnapshot() => new()
        {
            // Base properties
            Name = method.Name,
            Namespace = method.Namespace,
            FullyQualifiedName = method.FullyQualifiedName,
            GloballyQualifiedName = method.GloballyQualifiedName,
            PropertyName = method.PropertyName,
            ParameterName = method.ParameterName,
            AccessibilityString = method.AccessibilityString,
            IsPublic = method.IsPublic,
            IsInternal = method.IsInternal,
            IsStatic = method.IsStatic,
            IsAbstract = method.IsAbstract,
            IsSealed = method.IsSealed,
            IsVirtual = method.IsVirtual,
            IsOverride = method.IsOverride,
            IsReadOnly = method.IsReadOnly,
            IsPartial = method.IsPartial,
            Kind = method.Kind,
            IsValueType = method.IsValueType,
            IsReferenceType = method.IsReferenceType,
            IsNullable = method.IsNullable,
            Documentation = method.XmlDocumentation.ToSnapshot(),

            // Method-specific properties
            ReturnType = method.ReturnType.GloballyQualifiedName,
            ReturnsVoid = method.ReturnsVoid,
            AsyncKind = method.AsyncKind,
            AsyncReturnType = method.AsyncReturnType.OrNull() is { } art
                ? ValidSymbol<ITypeSymbol>.From(art).GloballyQualifiedName
                : null,
            MethodKind = method.MethodKind,
            IsExtensionMethod = method.IsExtensionMethod(),
            IsGenericMethod = method.IsGenericMethod(),
            IsPartialMethod = method.IsPartialMethod(),
            IsAsync = method.IsAsync(),
            Parameters = method.Parameters
                .Select(p => p.ToSnapshot())
                .ToEquatableArray(),
        };
    }

    extension(ValidSymbol<IParameterSymbol> parameter)
    {
        /// <summary>
        /// Creates a pipeline-safe <see cref="ParameterSnapshot"/> from this parameter symbol.
        /// </summary>
        public ParameterSnapshot ToSnapshot() => new()
        {
            // Base properties
            Name = parameter.Name,
            Namespace = parameter.Namespace,
            FullyQualifiedName = parameter.FullyQualifiedName,
            GloballyQualifiedName = parameter.GloballyQualifiedName,
            PropertyName = parameter.PropertyName,
            ParameterName = parameter.ParameterName,
            AccessibilityString = parameter.AccessibilityString,
            IsPublic = parameter.IsPublic,
            IsInternal = parameter.IsInternal,
            IsStatic = parameter.IsStatic,
            IsAbstract = parameter.IsAbstract,
            IsSealed = parameter.IsSealed,
            IsVirtual = parameter.IsVirtual,
            IsOverride = parameter.IsOverride,
            IsReadOnly = parameter.IsReadOnly,
            IsPartial = parameter.IsPartial,
            Kind = parameter.Kind,
            IsValueType = parameter.IsValueType,
            IsReferenceType = parameter.IsReferenceType,
            IsNullable = parameter.IsNullable,
            Documentation = parameter.XmlDocumentation.ToSnapshot(),

            // Parameter-specific properties
            Type = parameter.Type.GloballyQualifiedName,
            HasExplicitDefaultValue = parameter.HasExplicitDefaultValue,
            DefaultValueExpression = parameter.HasExplicitDefaultValue
                ? FormatDefaultValue(parameter.Value.ExplicitDefaultValue, parameter.Value.Type)
                : null,
            RefKind = parameter.Value.RefKind,
            IsParams = parameter.Value.IsParams,
            IsOptional = parameter.Value.IsOptional,
        };
    }

    extension(ValidSymbol<IPropertySymbol> property)
    {
        /// <summary>
        /// Creates a pipeline-safe <see cref="PropertySnapshot"/> from this property symbol.
        /// </summary>
        public PropertySnapshot ToSnapshot() => new()
        {
            // Base properties
            Name = property.Name,
            Namespace = property.Namespace,
            FullyQualifiedName = property.FullyQualifiedName,
            GloballyQualifiedName = property.GloballyQualifiedName,
            PropertyName = property.PropertyName,
            ParameterName = property.ParameterName,
            AccessibilityString = property.AccessibilityString,
            IsPublic = property.IsPublic,
            IsInternal = property.IsInternal,
            IsStatic = property.IsStatic,
            IsAbstract = property.IsAbstract,
            IsSealed = property.IsSealed,
            IsVirtual = property.IsVirtual,
            IsOverride = property.IsOverride,
            IsReadOnly = property.IsReadOnly,
            IsPartial = property.IsPartial,
            Kind = property.Kind,
            IsValueType = property.IsValueType,
            IsReferenceType = property.IsReferenceType,
            IsNullable = property.IsNullable,
            Documentation = property.XmlDocumentation.ToSnapshot(),

            // Property-specific properties
            Type = property.Type.GloballyQualifiedName,
            HasGetter = property.Value.GetMethod != null,
            HasSetter = property.Value.SetMethod != null,
            IsInitOnly = property.Value.SetMethod?.IsInitOnly == true,
            IsRequired = property.Value.IsRequired,
            IsIndexer = property.Value.IsIndexer,
            Parameters = property.Value.IsIndexer
                ? property.Value.Parameters
                    .Select(p => ValidSymbol<IParameterSymbol>.From(p).ToSnapshot())
                    .ToEquatableArray()
                : [],
        };
    }

    extension(ValidSymbol<IFieldSymbol> field)
    {
        /// <summary>
        /// Creates a pipeline-safe <see cref="FieldSnapshot"/> from this field symbol.
        /// </summary>
        public FieldSnapshot ToSnapshot() => new()
        {
            // Base properties
            Name = field.Name,
            Namespace = field.Namespace,
            FullyQualifiedName = field.FullyQualifiedName,
            GloballyQualifiedName = field.GloballyQualifiedName,
            PropertyName = field.PropertyName,
            ParameterName = field.ParameterName,
            AccessibilityString = field.AccessibilityString,
            IsPublic = field.IsPublic,
            IsInternal = field.IsInternal,
            IsStatic = field.IsStatic,
            IsAbstract = field.IsAbstract,
            IsSealed = field.IsSealed,
            IsVirtual = field.IsVirtual,
            IsOverride = field.IsOverride,
            IsReadOnly = field.IsReadOnly,
            IsPartial = field.IsPartial,
            Kind = field.Kind,
            IsValueType = field.IsValueType,
            IsReferenceType = field.IsReferenceType,
            IsNullable = field.IsNullable,
            Documentation = field.XmlDocumentation.ToSnapshot(),

            // Field-specific properties
            Type = field.Type.GloballyQualifiedName,
            IsConst = field.Value.IsConst,
            IsVolatile = field.Value.IsVolatile,
            HasConstantValue = field.Value.HasConstantValue,
            ConstantValueExpression = field.Value.HasConstantValue
                ? FormatDefaultValue(field.Value.ConstantValue, field.Value.Type)
                : null,
        };
    }

    extension(ValidSymbol<IEventSymbol> @event)
    {
        /// <summary>
        /// Creates a pipeline-safe <see cref="EventSnapshot"/> from this event symbol.
        /// </summary>
        public EventSnapshot ToSnapshot() => new()
        {
            // Base properties
            Name = @event.Name,
            Namespace = @event.Namespace,
            FullyQualifiedName = @event.FullyQualifiedName,
            GloballyQualifiedName = @event.GloballyQualifiedName,
            PropertyName = @event.PropertyName,
            ParameterName = @event.ParameterName,
            AccessibilityString = @event.AccessibilityString,
            IsPublic = @event.IsPublic,
            IsInternal = @event.IsInternal,
            IsStatic = @event.IsStatic,
            IsAbstract = @event.IsAbstract,
            IsSealed = @event.IsSealed,
            IsVirtual = @event.IsVirtual,
            IsOverride = @event.IsOverride,
            IsReadOnly = @event.IsReadOnly,
            IsPartial = @event.IsPartial,
            Kind = @event.Kind,
            IsValueType = @event.IsValueType,
            IsReferenceType = @event.IsReferenceType,
            IsNullable = @event.IsNullable,
            Documentation = @event.XmlDocumentation.ToSnapshot(),

            // Event-specific properties
            Type = @event.Type.GloballyQualifiedName,
        };
    }

    /// <summary>
    /// Formats a constant/default value as a C# expression string.
    /// </summary>
    private static string? FormatDefaultValue(object? value, ITypeSymbol type)
    {
        return value switch
        {
            null => "default",
            string s => $"\"{s}\"",
            char c => $"'{c}'",
            bool b => b ? "true" : "false",
            _ => value.ToString()
        };
    }
}
