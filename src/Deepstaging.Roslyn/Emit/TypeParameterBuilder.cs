// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit;

/// <summary>
/// Fluent builder for generic type parameters.
/// Supports constraints like where T : class, IDisposable, new().
/// Immutable - each method returns a new instance.
/// </summary>
public record struct TypeParameterBuilder()
{
    /// <summary>
    /// The type parameter name (e.g., "T", "RT", "TResult").
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// The type constraints.
    /// </summary>
    public ImmutableArray<string> Constraints { get; init; }

    #region Factory Methods

    /// <summary>
    /// Creates a type parameter builder for the specified type parameter.
    /// </summary>
    /// <param name="name">The type parameter name (e.g., "T", "RT", "TResult").</param>
    public static TypeParameterBuilder For(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Type parameter name cannot be null or empty.", nameof(name));

        return new TypeParameterBuilder { Name = name };
    }

    #endregion

    #region Constraints

    /// <summary>
    /// Adds a type constraint (e.g., "IDisposable", "BaseClass").
    /// </summary>
    /// <param name="constraint">The type constraint.</param>
    public TypeParameterBuilder WithConstraint(string constraint)
    {
        if (string.IsNullOrWhiteSpace(constraint))
            throw new ArgumentException("Constraint cannot be null or empty.", nameof(constraint));

        var constraints = Constraints.IsDefault ? [] : Constraints;
        return this with { Constraints = constraints.Add(constraint) };
    }

    /// <summary>
    /// Adds a 'class' constraint (reference type constraint).
    /// </summary>
    public TypeParameterBuilder AsClass()
    {
        var constraints = Constraints.IsDefault ? [] : Constraints;
        return this with { Constraints = constraints.Add("class") };
    }

    /// <summary>
    /// Adds a 'struct' constraint (value type constraint).
    /// </summary>
    public TypeParameterBuilder AsStruct()
    {
        var constraints = Constraints.IsDefault ? [] : Constraints;
        return this with { Constraints = constraints.Add("struct") };
    }

    /// <summary>
    /// Adds a 'notnull' constraint.
    /// </summary>
    public TypeParameterBuilder AsNotNull()
    {
        var constraints = Constraints.IsDefault ? [] : Constraints;
        return this with { Constraints = constraints.Add("notnull") };
    }

    /// <summary>
    /// Adds a 'new()' constraint (parameterless constructor).
    /// </summary>
    public TypeParameterBuilder WithNewConstraint()
    {
        var constraints = Constraints.IsDefault ? [] : Constraints;
        return this with { Constraints = constraints.Add("new()") };
    }

    #endregion

    #region Building

    /// <summary>
    /// Builds the type parameter as a TypeParameterSyntax node.
    /// </summary>
    internal TypeParameterSyntax Build() =>
        SyntaxFactory.TypeParameter(SyntaxFactory.Identifier(Name));

    /// <summary>
    /// Builds the type parameter constraint clause if there are constraints.
    /// </summary>
    internal TypeParameterConstraintClauseSyntax? BuildConstraintClause()
    {
        var constraints = Constraints.IsDefault ? [] : Constraints;
        if (constraints.Length == 0)
            return null;

        var constraintSyntaxList = new List<TypeParameterConstraintSyntax>();

        foreach (var constraint in constraints)
        {
            TypeParameterConstraintSyntax constraintSyntax = constraint switch
            {
                "class" => SyntaxFactory.ClassOrStructConstraint(SyntaxKind.ClassConstraint),
                "struct" => SyntaxFactory.ClassOrStructConstraint(SyntaxKind.StructConstraint),
                "notnull" => SyntaxFactory.TypeConstraint(SyntaxFactory.ParseTypeName("notnull")),
                "new()" => SyntaxFactory.ConstructorConstraint(),
                _ => SyntaxFactory.TypeConstraint(SyntaxFactory.ParseTypeName(constraint))
            };
            constraintSyntaxList.Add(constraintSyntax);
        }

        return SyntaxFactory.TypeParameterConstraintClause(
            SyntaxFactory.IdentifierName(Name),
            SyntaxFactory.SeparatedList(constraintSyntaxList));
    }

    /// <summary>
    /// Gets whether this type parameter has constraints.
    /// </summary>
    public bool HasConstraints
    {
        get
        {
            var constraints = Constraints.IsDefault ? [] : Constraints;
            return constraints.Length > 0;
        }
    }

    #endregion
}