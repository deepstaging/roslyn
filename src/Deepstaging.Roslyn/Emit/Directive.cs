// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit;

/// <summary>
/// Represents a preprocessor directive condition for conditional compilation.
/// Used to wrap code in #if/#endif blocks.
/// </summary>
/// <param name="Condition">The preprocessor condition (e.g., "NET6_0_OR_GREATER").</param>
public readonly record struct Directive(string Condition)
{
    /// <summary>
    /// Combines this directive with another using logical AND.
    /// </summary>
    /// <param name="other">The other directive to combine with.</param>
    /// <returns>A new directive representing the combined condition.</returns>
    public Directive And(Directive other) => new($"{Condition} && {other.Condition}");

    /// <summary>
    /// Combines this directive with another using logical OR.
    /// </summary>
    /// <param name="other">The other directive to combine with.</param>
    /// <returns>A new directive representing the combined condition.</returns>
    public Directive Or(Directive other) => new($"{Condition} || {other.Condition}");

    /// <summary>
    /// Negates this directive.
    /// </summary>
    /// <returns>A new directive representing the negated condition.</returns>
    public Directive Not() => new($"!{Condition}");

    /// <summary>
    /// Returns the condition string.
    /// </summary>
    public override string ToString() => Condition;
}

/// <summary>
/// Pre-defined preprocessor directives for common .NET compilation targets.
/// Use these to conditionally include code for specific framework versions.
/// </summary>
/// <example>
/// <code>
/// // Conditional interface implementation
/// TypeBuilder.Struct("MyId")
///     .Implements("ISpanFormattable", Directives.Net6OrGreater)
///     .Implements("IParsable&lt;MyId&gt;", Directives.Net7OrGreater);
///
/// // Conditional method
/// builder.AddMethod("Parse", m => m
///     .When(Directives.Net7OrGreater)
///     .WithBody(b => b.AddReturn("new()")));
///
/// // Custom condition
/// builder.When(Directives.Custom("MY_FEATURE_FLAG"));
///
/// // Compound conditions
/// builder.When(Directives.Net6OrGreater.And(Directives.Custom("ENABLE_SPANS")));
/// </code>
/// </example>
public static class Directives
{
    #region .NET Version Directives

    /// <summary>Targets .NET 5.0 or greater.</summary>
    public static Directive Net5OrGreater => new("NET5_0_OR_GREATER");

    /// <summary>Targets .NET 6.0 or greater.</summary>
    public static Directive Net6OrGreater => new("NET6_0_OR_GREATER");

    /// <summary>Targets .NET 6.0 only (not .NET 7.0 or greater).</summary>
    public static Directive Net6Only => new("NET6_0_OR_GREATER && !NET7_0_OR_GREATER");

    /// <summary>Targets .NET 7.0 or greater.</summary>
    public static Directive Net7OrGreater => new("NET7_0_OR_GREATER");

    /// <summary>Targets versions before .NET 7.0.</summary>
    public static Directive NotNet7OrGreater => new("!NET7_0_OR_GREATER");

    /// <summary>Targets .NET 8.0 or greater.</summary>
    public static Directive Net8OrGreater => new("NET8_0_OR_GREATER");

    /// <summary>Targets .NET 9.0 or greater.</summary>
    public static Directive Net9OrGreater => new("NET9_0_OR_GREATER");

    #endregion

    #region .NET Core Directives

    /// <summary>Targets .NET Core (any version).</summary>
    public static Directive NetCoreApp => new("NETCOREAPP");

    /// <summary>Targets .NET Core 2.1 or greater.</summary>
    public static Directive NetCoreApp21OrGreater => new("NETCOREAPP2_1_OR_GREATER");

    /// <summary>Targets .NET Core 3.0 or greater.</summary>
    public static Directive NetCoreApp30OrGreater => new("NETCOREAPP3_0_OR_GREATER");

    /// <summary>Targets .NET Core 3.1 or greater.</summary>
    public static Directive NetCoreApp31OrGreater => new("NETCOREAPP3_1_OR_GREATER");

    #endregion

    #region .NET Standard Directives

    /// <summary>Targets .NET Standard (any version).</summary>
    public static Directive NetStandard => new("NETSTANDARD");

    /// <summary>Targets .NET Standard 2.0 or greater.</summary>
    public static Directive NetStandard20OrGreater => new("NETSTANDARD2_0_OR_GREATER");

    /// <summary>Targets .NET Standard 2.1 or greater.</summary>
    public static Directive NetStandard21OrGreater => new("NETSTANDARD2_1_OR_GREATER");

    #endregion

    #region .NET Framework Directives

    /// <summary>Targets .NET Framework (any version).</summary>
    public static Directive NetFramework => new("NETFRAMEWORK");

    /// <summary>Targets .NET Framework 4.6.1 or greater.</summary>
    public static Directive NetFramework461OrGreater => new("NET461_OR_GREATER");

    /// <summary>Targets .NET Framework 4.7.2 or greater.</summary>
    public static Directive NetFramework472OrGreater => new("NET472_OR_GREATER");

    /// <summary>Targets .NET Framework 4.8 or greater.</summary>
    public static Directive NetFramework48OrGreater => new("NET48_OR_GREATER");

    #endregion

    #region Custom Directives

    /// <summary>
    /// Creates a custom directive with the specified condition.
    /// </summary>
    /// <param name="condition">The preprocessor condition (e.g., "MY_FEATURE_FLAG").</param>
    /// <returns>A directive with the custom condition.</returns>
    public static Directive Custom(string condition) => new(condition);

    #endregion
}