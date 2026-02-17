// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for creating snapshots from <see cref="ValidSymbol{TSymbol}"/> types.
/// </summary>
public static class ValidSymbolSnapshotExtensions
{
    /// <summary>
    /// Format that produces canonical globally-qualified names without C# keyword substitution.
    /// <c>System.Int32</c> → <c>global::System.Int32</c> (not <c>int</c>).
    /// </summary>
    private static readonly SymbolDisplayFormat CanonicalGlobalFormat = new(
        SymbolDisplayGlobalNamespaceStyle.Included,
        SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        SymbolDisplayGenericsOptions.IncludeTypeParameters,
        miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers);

    extension<TSymbol>(ValidSymbol<TSymbol> symbol) where TSymbol : class, ISymbol
    {
        /// <summary>
        /// Populates the base <see cref="SymbolSnapshot"/> properties on a derived snapshot
        /// using a record <c>with</c> expression.
        /// Uses <see cref="SymbolDisplayFormat.FullyQualifiedFormat"/> for
        /// <see cref="SymbolSnapshot.GloballyQualifiedName"/> so that
        /// <c>System.Int32</c> becomes <c>global::System.Int32</c> (not <c>int</c>),
        /// ensuring reliable type identity comparisons in pipeline-safe code.
        /// </summary>
        internal T WithBaseProperties<T>(T snapshot) where T : SymbolSnapshot =>
            snapshot with
            {
                Name = symbol.Name,
                Namespace = symbol.Namespace,
                FullyQualifiedName = symbol.FullyQualifiedName,
                GloballyQualifiedName = symbol.Value.ToDisplayString(CanonicalGlobalFormat),
                CodeName = symbol.GloballyQualifiedName,
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
                Documentation = symbol.XmlDocumentation.ToSnapshot()
            };
    }

    extension(ValidSymbol<INamedTypeSymbol> symbol)
    {
        /// <summary>
        /// Creates a pipeline-safe <see cref="TypeSnapshot"/> from this named type symbol.
        /// </summary>
        public TypeSnapshot ToSnapshot() => symbol.WithBaseProperties(new TypeSnapshot
        {
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
                .ToEquatableArray()
        });
    }

    extension(ValidSymbol<IMethodSymbol> method)
    {
        /// <summary>
        /// Creates a pipeline-safe <see cref="MethodSnapshot"/> from this method symbol.
        /// </summary>
        public MethodSnapshot ToSnapshot() => method.WithBaseProperties(new MethodSnapshot
        {
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
                .ToEquatableArray()
        });
    }

    extension(ValidSymbol<IParameterSymbol> parameter)
    {
        /// <summary>
        /// Creates a pipeline-safe <see cref="ParameterSnapshot"/> from this parameter symbol.
        /// </summary>
        public ParameterSnapshot ToSnapshot() => parameter.WithBaseProperties(new ParameterSnapshot
        {
            Type = parameter.Type.GloballyQualifiedName,
            HasExplicitDefaultValue = parameter.HasExplicitDefaultValue,
            DefaultValueExpression = parameter.HasExplicitDefaultValue
                ? FormatDefaultValue(parameter.Value.ExplicitDefaultValue, parameter.Value.Type)
                : null,
            RefKind = parameter.Value.RefKind,
            IsParams = parameter.Value.IsParams,
            IsOptional = parameter.Value.IsOptional
        });
    }

    extension(ValidSymbol<IPropertySymbol> property)
    {
        /// <summary>
        /// Creates a pipeline-safe <see cref="PropertySnapshot"/> from this property symbol.
        /// </summary>
        public PropertySnapshot ToSnapshot() => property.WithBaseProperties(new PropertySnapshot
        {
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
                : []
        });
    }

    extension(ValidSymbol<IFieldSymbol> field)
    {
        /// <summary>
        /// Creates a pipeline-safe <see cref="FieldSnapshot"/> from this field symbol.
        /// </summary>
        public FieldSnapshot ToSnapshot() => field.WithBaseProperties(new FieldSnapshot
        {
            Type = field.Type.GloballyQualifiedName,
            IsConst = field.Value.IsConst,
            IsVolatile = field.Value.IsVolatile,
            HasConstantValue = field.Value.HasConstantValue,
            ConstantValueExpression = field.Value.HasConstantValue
                ? FormatDefaultValue(field.Value.ConstantValue, field.Value.Type)
                : null
        });
    }

    extension(ValidSymbol<IEventSymbol> @event)
    {
        /// <summary>
        /// Creates a pipeline-safe <see cref="EventSnapshot"/> from this event symbol.
        /// </summary>
        public EventSnapshot ToSnapshot() => @event.WithBaseProperties(new EventSnapshot
        {
            Type = @event.Type.GloballyQualifiedName
        });
    }

    /// <summary>
    /// Formats a constant/default value as a C# expression string.
    /// </summary>
    private static string? FormatDefaultValue(object? value, ITypeSymbol type) =>
        value switch
        {
            null => "default",
            string s => $"\"{s}\"",
            char c => $"'{c}'",
            bool b => b ? "true" : "false",
            _ => value.ToString()
        };
}