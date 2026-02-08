// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Patterns;

/// <summary>
/// TypeBuilder extensions for implementing the Singleton pattern.
/// Adds a private constructor and static Instance property.
/// </summary>
public static class TypeBuilderSingletonExtensions
{
    /// <summary>
    /// Implements the Singleton pattern with a static Instance property.
    /// Uses Lazy&lt;T&gt; for thread-safe lazy initialization.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <returns>The modified type builder with Singleton implementation.</returns>
    public static TypeBuilder AsSingleton(this TypeBuilder builder)
    {
        var typeName = builder.Name;

        return builder
            .AddField(FieldBuilder
                .Parse($"private static readonly global::System.Lazy<{typeName}> _instance = new(() => new {typeName}());"))
            .AddConstructor(ConstructorBuilder
                .For(typeName)
                .WithAccessibility(Accessibility.Private))
            .AddProperty(PropertyBuilder
                .For("Instance", typeName)
                .AsStatic()
                .AsReadOnly()
                .WithGetter($"_instance.Value"));
    }

    /// <summary>
    /// Implements the Singleton pattern with a custom instance field and property name.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="instancePropertyName">The name of the static Instance property.</param>
    /// <returns>The modified type builder with Singleton implementation.</returns>
    public static TypeBuilder AsSingleton(this TypeBuilder builder, string instancePropertyName)
    {
        var typeName = builder.Name;
        var fieldName = instancePropertyName.ToBackingFieldName();

        return builder
            .AddField(FieldBuilder
                .Parse($"private static readonly global::System.Lazy<{typeName}> {fieldName} = new(() => new {typeName}());"))
            .AddConstructor(ConstructorBuilder
                .For(typeName)
                .WithAccessibility(Accessibility.Private))
            .AddProperty(PropertyBuilder
                .For(instancePropertyName, typeName)
                .AsStatic()
                .AsReadOnly()
                .WithGetter($"{fieldName}.Value"));
    }
}
