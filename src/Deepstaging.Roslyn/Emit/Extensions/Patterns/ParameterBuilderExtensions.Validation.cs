// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Patterns;

/// <summary>
/// ParameterBuilder extensions for adding validation rules.
/// Validation is applied when the parameter is used in a constructor.
/// </summary>
public static class ParameterBuilderValidationExtensions
{
    #region Null Validation

    /// <summary>
    /// Adds null validation. Throws ArgumentNullException if the parameter is null.
    /// </summary>
    public static ParameterBuilder ThrowIfNull(this ParameterBuilder builder)
    {
        var validations = builder.Validations.IsDefault ? [] : builder.Validations;

        return builder with
        {
            Validations = validations.Add(new ParameterValidation(ParameterValidationKind.ThrowIfNull))
        };
    }

    /// <summary>
    /// Adds null-or-empty validation. Throws ArgumentException if the parameter is null or empty.
    /// </summary>
    public static ParameterBuilder ThrowIfNullOrEmpty(this ParameterBuilder builder)
    {
        var validations = builder.Validations.IsDefault ? [] : builder.Validations;

        return builder with
        {
            Validations = validations.Add(new ParameterValidation(ParameterValidationKind.ThrowIfNullOrEmpty))
        };
    }

    /// <summary>
    /// Adds null-or-whitespace validation. Throws ArgumentException if the parameter is null, empty, or whitespace.
    /// </summary>
    public static ParameterBuilder ThrowIfNullOrWhiteSpace(this ParameterBuilder builder)
    {
        var validations = builder.Validations.IsDefault ? [] : builder.Validations;

        return builder with
        {
            Validations = validations.Add(new ParameterValidation(ParameterValidationKind.ThrowIfNullOrWhiteSpace))
        };
    }

    #endregion

    #region Range Validation

    /// <summary>
    /// Adds range validation. Throws ArgumentOutOfRangeException if outside the specified range.
    /// </summary>
    /// <param name="builder">The parameter builder.</param>
    /// <param name="minValue">The minimum allowed value (inclusive).</param>
    /// <param name="maxValue">The maximum allowed value (inclusive).</param>
    public static ParameterBuilder ThrowIfOutOfRange(this ParameterBuilder builder, string minValue, string maxValue)
    {
        var validations = builder.Validations.IsDefault ? [] : builder.Validations;

        return builder with
        {
            Validations =
            validations.Add(new ParameterValidation(ParameterValidationKind.ThrowIfOutOfRange, minValue, maxValue))
        };
    }

    /// <summary>
    /// Adds positive validation. Throws ArgumentOutOfRangeException if zero or negative.
    /// </summary>
    public static ParameterBuilder ThrowIfNotPositive(this ParameterBuilder builder)
    {
        var validations = builder.Validations.IsDefault ? [] : builder.Validations;

        return builder with
        {
            Validations = validations.Add(new ParameterValidation(ParameterValidationKind.ThrowIfNotPositive))
        };
    }

    /// <summary>
    /// Adds non-negative validation. Throws ArgumentOutOfRangeException if negative.
    /// </summary>
    public static ParameterBuilder ThrowIfNegative(this ParameterBuilder builder)
    {
        var validations = builder.Validations.IsDefault ? [] : builder.Validations;

        return builder with
        {
            Validations = validations.Add(new ParameterValidation(ParameterValidationKind.ThrowIfNegative))
        };
    }

    /// <summary>
    /// Adds zero validation. Throws ArgumentOutOfRangeException if zero.
    /// </summary>
    public static ParameterBuilder ThrowIfZero(this ParameterBuilder builder)
    {
        var validations = builder.Validations.IsDefault ? [] : builder.Validations;

        return builder with
        {
            Validations = validations.Add(new ParameterValidation(ParameterValidationKind.ThrowIfZero))
        };
    }

    #endregion

    #region Assignment

    /// <summary>
    /// Specifies that this parameter should be assigned to a member in the constructor body.
    /// </summary>
    /// <param name="builder">The parameter builder.</param>
    /// <param name="memberName">The property or field name to assign to.</param>
    public static ParameterBuilder AssignsTo(this ParameterBuilder builder, string memberName) =>
        builder with { AssignsToMember = memberName };

    /// <summary>
    /// Specifies that this parameter should be assigned to a property.
    /// </summary>
    public static ParameterBuilder AssignsTo(this ParameterBuilder builder, PropertyBuilder property) =>
        builder with { AssignsToMember = property.Name };

    /// <summary>
    /// Specifies that this parameter should be assigned to a field.
    /// </summary>
    public static ParameterBuilder AssignsTo(this ParameterBuilder builder, FieldBuilder field) =>
        builder with { AssignsToMember = field.Name };

    #endregion

    #region Code Generation

    /// <summary>
    /// Generates the validation statements for this parameter.
    /// </summary>
    internal static IEnumerable<string> GetValidationStatements(this ParameterBuilder builder)
    {
        var validations = builder.Validations.IsDefault ? [] : builder.Validations;

        foreach (var validation in validations)
            yield return validation.Kind switch
            {
                ParameterValidationKind.ThrowIfNull =>
                    $"global::System.ArgumentNullException.ThrowIfNull({builder.Name});",
                ParameterValidationKind.ThrowIfNullOrEmpty =>
                    $"global::System.ArgumentException.ThrowIfNullOrEmpty({builder.Name});",
                ParameterValidationKind.ThrowIfNullOrWhiteSpace =>
                    $"global::System.ArgumentException.ThrowIfNullOrWhiteSpace({builder.Name});",
                ParameterValidationKind.ThrowIfOutOfRange =>
                    $"global::System.ArgumentOutOfRangeException.ThrowIfLessThan({builder.Name}, {validation.MinValue});\n" +
                    $"global::System.ArgumentOutOfRangeException.ThrowIfGreaterThan({builder.Name}, {validation.MaxValue});",
                ParameterValidationKind.ThrowIfNotPositive =>
                    $"global::System.ArgumentOutOfRangeException.ThrowIfNegativeOrZero({builder.Name});",
                ParameterValidationKind.ThrowIfNegative =>
                    $"global::System.ArgumentOutOfRangeException.ThrowIfNegative({builder.Name});",
                ParameterValidationKind.ThrowIfZero =>
                    $"global::System.ArgumentOutOfRangeException.ThrowIfZero({builder.Name});",
                _ => throw new InvalidOperationException($"Unknown validation kind: {validation.Kind}")
            };
    }

    /// <summary>
    /// Gets the assignment statement if AssignsTo was specified.
    /// </summary>
    internal static string? GetAssignmentStatement(this ParameterBuilder builder) =>
        builder.AssignsToMember is { } member ? $"{member} = {builder.Name};" : null;

    #endregion
}