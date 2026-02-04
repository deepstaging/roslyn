// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Emit;

/// <summary>
/// Fluent builder for generic type parameters.
/// Supports constraints like where T : class, IDisposable, new().
/// Immutable - each method returns a new instance.
/// </summary>
public readonly struct TypeParameterBuilder
{
    private readonly string _name;
    private readonly ImmutableArray<string> _constraints;

    private TypeParameterBuilder(
        string name,
        ImmutableArray<string> constraints)
    {
        _name = name;
        _constraints = constraints.IsDefault ? ImmutableArray<string>.Empty : constraints;
    }

    #region Factory Methods

    /// <summary>
    /// Creates a type parameter builder for the specified type parameter.
    /// </summary>
    /// <param name="name">The type parameter name (e.g., "T", "RT", "TResult").</param>
    public static TypeParameterBuilder For(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Type parameter name cannot be null or empty.", nameof(name));

        return new TypeParameterBuilder(name, ImmutableArray<string>.Empty);
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

        return new TypeParameterBuilder(_name, _constraints.Add(constraint));
    }

    /// <summary>
    /// Adds a 'class' constraint (reference type constraint).
    /// </summary>
    public TypeParameterBuilder AsClass()
    {
        return new TypeParameterBuilder(_name, _constraints.Add("class"));
    }

    /// <summary>
    /// Adds a 'struct' constraint (value type constraint).
    /// </summary>
    public TypeParameterBuilder AsStruct()
    {
        return new TypeParameterBuilder(_name, _constraints.Add("struct"));
    }

    /// <summary>
    /// Adds a 'notnull' constraint.
    /// </summary>
    public TypeParameterBuilder AsNotNull()
    {
        return new TypeParameterBuilder(_name, _constraints.Add("notnull"));
    }

    /// <summary>
    /// Adds a 'new()' constraint (parameterless constructor).
    /// </summary>
    public TypeParameterBuilder WithNewConstraint()
    {
        return new TypeParameterBuilder(_name, _constraints.Add("new()"));
    }

    #endregion

    #region Building

    /// <summary>
    /// Builds the type parameter as a TypeParameterSyntax node.
    /// </summary>
    internal TypeParameterSyntax Build()
    {
        return SyntaxFactory.TypeParameter(SyntaxFactory.Identifier(_name));
    }

    /// <summary>
    /// Builds the type parameter constraint clause if there are constraints.
    /// </summary>
    internal TypeParameterConstraintClauseSyntax? BuildConstraintClause()
    {
        if (_constraints.Length == 0)
            return null;

        var constraints = new List<TypeParameterConstraintSyntax>();

        foreach (var constraint in _constraints)
        {
            TypeParameterConstraintSyntax constraintSyntax = constraint switch
            {
                "class" => SyntaxFactory.ClassOrStructConstraint(SyntaxKind.ClassConstraint),
                "struct" => SyntaxFactory.ClassOrStructConstraint(SyntaxKind.StructConstraint),
                "notnull" => SyntaxFactory.TypeConstraint(SyntaxFactory.ParseTypeName("notnull")),
                "new()" => SyntaxFactory.ConstructorConstraint(),
                _ => SyntaxFactory.TypeConstraint(SyntaxFactory.ParseTypeName(constraint))
            };
            constraints.Add(constraintSyntax);
        }

        return SyntaxFactory.TypeParameterConstraintClause(
            SyntaxFactory.IdentifierName(_name),
            SyntaxFactory.SeparatedList(constraints));
    }

    /// <summary>
    /// Gets the type parameter name.
    /// </summary>
    public string Name => _name;

    /// <summary>
    /// Gets whether this type parameter has constraints.
    /// </summary>
    public bool HasConstraints => _constraints.Length > 0;

    #endregion
}
