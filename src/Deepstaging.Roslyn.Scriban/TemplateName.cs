// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using System.Reflection;

// ReSharper disable UnusedMember.Global

namespace Deepstaging.Roslyn.Scriban;

/// <summary>
/// Represents a unique identifier for a Scriban template, including its name and assembly location.
/// Used to load embedded template resources during source generation.
/// </summary>
public readonly struct TemplateName : IEquatable<TemplateName>
{
    /// <summary>
    /// Gets the fully qualified name of the template (e.g., "Namespace.Templates.TemplateName.scriban-cs").
    /// </summary>
    public string Value { get; }
    
    /// <summary>
    /// Gets the assembly containing the embedded template resource.
    /// </summary>
    public Assembly Assembly { get; }

    private TemplateName(string value, Assembly assembly)
    {
        Value = value.Trim();
        Assembly = assembly;
    }

    /// <summary>
    /// Implicitly converts a TemplateName to its string value.
    /// </summary>
    public static implicit operator string(TemplateName templateName) => templateName.Value;
    
    /// <inheritdoc />
    public bool Equals(TemplateName other) => Value == other.Value && Assembly == other.Assembly;
    
    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is TemplateName other && Equals(other);
    
    /// <inheritdoc />
    public override int GetHashCode() => (Value, Assembly).GetHashCode();
    
    /// <inheritdoc />
    public override string ToString() => Value;
    
    /// <summary>
    /// Equality operator for TemplateName comparison.
    /// </summary>
    public static bool operator ==(TemplateName left, TemplateName right) => left.Equals(right);
    
    /// <summary>
    /// Inequality operator for TemplateName comparison.
    /// </summary>
    public static bool operator !=(TemplateName left, TemplateName right) => !left.Equals(right);

    /// <summary>
    /// Creates a template name factory for a specific generator type.
    /// The factory function constructs fully qualified template names in the format:
    /// "{GeneratorNamespace}.Templates.{templateName}"
    /// </summary>
    /// <typeparam name="T">The generator type that will use these templates.</typeparam>
    /// <returns>A function that creates TemplateName instances for the given generator.</returns>
    /// <example>
    /// <code>
    /// var Named = TemplateName.ForGenerator&lt;MyGenerator&gt;();
    /// var templateName = Named("MyTemplate.scriban-cs");
    /// // Results in: "MyNamespace.Templates.MyTemplate.scriban-cs"
    /// </code>
    /// </example>
    public static Func<string, TemplateName> ForGenerator<T>()
    {
        var generatorType = typeof(T);
        var assembly = generatorType.Assembly;
        var namespaceName = generatorType.Namespace!;

        return templateName => new TemplateName($"{namespaceName}.Templates.{templateName}", assembly);
    }
}
