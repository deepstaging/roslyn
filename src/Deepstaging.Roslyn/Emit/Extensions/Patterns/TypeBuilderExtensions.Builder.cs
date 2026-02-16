// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Patterns;

/// <summary>
/// TypeBuilder extensions for implementing the Builder pattern.
/// Adds a nested Builder class with With* methods for each property.
/// </summary>
public static class TypeBuilderBuilderPatternExtensions
{
    /// <summary>
    /// Adds a nested Builder class for the fluent builder pattern.
    /// Generates With* methods for each settable property.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <returns>The modified type builder with a nested Builder class.</returns>
    public static TypeBuilder WithBuilder(this TypeBuilder builder) => builder.WithBuilder("Builder");

    /// <summary>
    /// Adds a nested Builder class with a custom name for the fluent builder pattern.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="builderClassName">The name of the nested builder class.</param>
    /// <returns>The modified type builder with a nested Builder class.</returns>
    /// <remarks>Only classes support builders. Records already have native 'with' syntax,
    /// interfaces cannot be instantiated, and structs are value types.</remarks>
    public static TypeBuilder WithBuilder(this TypeBuilder builder, string builderClassName)
    {
        // Only classes support builders
        if (builder is not { Kind: TypeKind.Class, IsRecord: false }) return builder;

        var typeName = builder.Name;
        var properties = builder.Properties.IsDefault ? [] : builder.Properties;

        // Create the builder as a record to leverage native 'with' syntax
        var builderClass = TypeBuilder
            .Record(builderClassName)
            .WithAccessibility(Accessibility.Public);

        // Add init properties and With* methods using 'with' expression
        foreach (var prop in properties)
            builderClass = builderClass
                .AddProperty(prop.Name, prop.Type!, p => p.WithAutoPropertyAccessors().WithInitOnlySetter())
                .AddMethod($"With{prop.Name}", m => m
                    .WithAccessibility(Accessibility.Public)
                    .WithReturnType(builderClassName)
                    .AddParameter("value", prop.Type!)
                    .WithExpressionBody($"this with {{ {prop.Name} = value }}"));

        // Build constructor parameters using property names
        var constructorParams = string.Join(", ", properties.Select(p => p.Name));

        builderClass = builderClass
            .AddMethod("Build", m => m
                .WithAccessibility(Accessibility.Public)
                .WithReturnType(typeName)
                .WithExpressionBody($"new {typeName}({constructorParams})"));

        return builder
            .AddNestedType(builderClass)
            .AddMethod("CreateBuilder", m => m
                .WithAccessibility(Accessibility.Public)
                .AsStatic()
                .WithReturnType(builderClassName)
                .WithExpressionBody($"new {builderClassName}()"));
    }
}