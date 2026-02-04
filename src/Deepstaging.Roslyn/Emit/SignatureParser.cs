// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Emit;

/// <summary>
/// Parses C# signature strings into builder instances using Roslyn's parser.
/// </summary>
internal static class SignatureParser
{
    #region Method Parsing

    /// <summary>
    /// Parses a method signature into a MethodBuilder.
    /// </summary>
    /// <param name="signature">The method signature (e.g., "public string GetName(int id)").</param>
    /// <returns>A configured MethodBuilder.</returns>
    /// <exception cref="ArgumentException">Thrown when the signature cannot be parsed.</exception>
    public static MethodBuilder ParseMethod(string signature)
    {
        if (string.IsNullOrWhiteSpace(signature))
            throw new ArgumentException("Method signature cannot be null or empty.", nameof(signature));

        // Wrap in a class context so Roslyn can parse it as a member
        var wrappedCode = $"class __Wrapper__ {{ {signature.Trim().TrimEnd(';')}; }}";
        var tree = CSharpSyntaxTree.ParseText(wrappedCode);
        var root = tree.GetCompilationUnitRoot();

        // Check for parse errors
        var diagnostics = tree.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
        if (diagnostics.Count > 0)
        {
            var errorMessages = string.Join("; ", diagnostics.Select(d => d.GetMessage()));
            throw new ArgumentException($"Failed to parse method signature: {errorMessages}", nameof(signature));
        }

        // Extract the method declaration
        var classDecl = root.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();
        var methodDecl = classDecl?.Members.OfType<MethodDeclarationSyntax>().FirstOrDefault();

        if (methodDecl == null)
            throw new ArgumentException($"Could not find method declaration in signature: {signature}", nameof(signature));

        return BuildMethodFromSyntax(methodDecl);
    }

    private static MethodBuilder BuildMethodFromSyntax(MethodDeclarationSyntax method)
    {
        // Start with the method name
        var builder = MethodBuilder.For(method.Identifier.Text);

        // Set return type
        builder = builder.WithReturnType(method.ReturnType.ToString());

        // Extract modifiers
        builder = ApplyMethodModifiers(builder, method.Modifiers);

        // Add type parameters
        if (method.TypeParameterList != null)
        {
            foreach (var typeParam in method.TypeParameterList.Parameters)
            {
                var constraint = FindConstraintClause(method.ConstraintClauses, typeParam.Identifier.Text);
                builder = AddTypeParameter(builder, typeParam, constraint);
            }
        }

        // Add parameters
        foreach (var param in method.ParameterList.Parameters)
        {
            builder = AddParameter(builder, param);
        }

        return builder;
    }

    private static MethodBuilder ApplyMethodModifiers(MethodBuilder builder, SyntaxTokenList modifiers)
    {
        foreach (var modifier in modifiers)
        {
            builder = modifier.Kind() switch
            {
                SyntaxKind.PublicKeyword => builder.WithAccessibility(Accessibility.Public),
                SyntaxKind.PrivateKeyword => builder.WithAccessibility(Accessibility.Private),
                SyntaxKind.ProtectedKeyword => builder.WithAccessibility(Accessibility.Protected),
                SyntaxKind.InternalKeyword => builder.WithAccessibility(Accessibility.Internal),
                SyntaxKind.StaticKeyword => builder.AsStatic(),
                SyntaxKind.AsyncKeyword => builder.Async(),
                SyntaxKind.VirtualKeyword => builder.AsVirtual(),
                SyntaxKind.OverrideKeyword => builder.AsOverride(),
                SyntaxKind.AbstractKeyword => builder.AsAbstract(),
                _ => builder
            };
        }
        return builder;
    }

    private static TypeParameterConstraintClauseSyntax? FindConstraintClause(
        SyntaxList<TypeParameterConstraintClauseSyntax> clauses,
        string typeParameterName)
    {
        return clauses.FirstOrDefault(c => c.Name.Identifier.Text == typeParameterName);
    }

    private static MethodBuilder AddTypeParameter(
        MethodBuilder builder,
        TypeParameterSyntax typeParam,
        TypeParameterConstraintClauseSyntax? constraint)
    {
        if (constraint == null)
        {
            return builder.AddTypeParameter(typeParam.Identifier.Text);
        }

        return builder.AddTypeParameter(typeParam.Identifier.Text, tp =>
        {
            foreach (var c in constraint.Constraints)
            {
                tp = c switch
                {
                    ClassOrStructConstraintSyntax { ClassOrStructKeyword.RawKind: (int)SyntaxKind.ClassKeyword } 
                        => tp.AsClass(),
                    ClassOrStructConstraintSyntax { ClassOrStructKeyword.RawKind: (int)SyntaxKind.StructKeyword } 
                        => tp.AsStruct(),
                    ConstructorConstraintSyntax 
                        => tp.WithNewConstraint(),
                    TypeConstraintSyntax typeConstraint 
                        => tp.WithConstraint(typeConstraint.Type.ToString()),
                    _ => tp
                };
            }
            return tp;
        });
    }

    private static MethodBuilder AddParameter(MethodBuilder builder, ParameterSyntax param)
    {
        var paramName = param.Identifier.Text;
        var paramType = param.Type?.ToString() ?? "object";

        // Check for parameter modifiers
        var hasRef = param.Modifiers.Any(m => m.IsKind(SyntaxKind.RefKeyword));
        var hasOut = param.Modifiers.Any(m => m.IsKind(SyntaxKind.OutKeyword));
        var hasIn = param.Modifiers.Any(m => m.IsKind(SyntaxKind.InKeyword));
        var hasParams = param.Modifiers.Any(m => m.IsKind(SyntaxKind.ParamsKeyword));
        var hasThis = param.Modifiers.Any(m => m.IsKind(SyntaxKind.ThisKeyword));

        // Check for default value
        var hasDefault = param.Default != null;

        if (!hasRef && !hasOut && !hasIn && !hasParams && !hasThis && !hasDefault)
        {
            return builder.AddParameter(paramName, paramType);
        }

        return builder.AddParameter(paramName, paramType, p =>
        {
            if (hasRef) p = p.AsRef();
            if (hasOut) p = p.AsOut();
            if (hasIn) p = p.AsIn();
            if (hasParams) p = p.AsParams();
            if (hasThis) p = p.AsThis();
            if (hasDefault) p = p.WithDefaultValue(param.Default!.Value.ToString());
            return p;
        });
    }

    #endregion

    #region Property Parsing

    /// <summary>
    /// Parses a property signature into a PropertyBuilder.
    /// </summary>
    /// <param name="signature">The property signature (e.g., "public string Name { get; set; }").</param>
    /// <returns>A configured PropertyBuilder.</returns>
    /// <exception cref="ArgumentException">Thrown when the signature cannot be parsed.</exception>
    public static PropertyBuilder ParseProperty(string signature)
    {
        if (string.IsNullOrWhiteSpace(signature))
            throw new ArgumentException("Property signature cannot be null or empty.", nameof(signature));

        // Normalize the signature - properties with initializers or expression bodies need semicolons
        var normalizedSignature = signature.Trim();
        
        // If the signature doesn't end with } or ; it likely needs a semicolon
        // (e.g., "public string Name => value" or "public int Count { get; } = 0")
        if (!normalizedSignature.EndsWith("}") && !normalizedSignature.EndsWith(";"))
        {
            normalizedSignature += ";";
        }

        // Wrap in a class context so Roslyn can parse it as a member
        var wrappedCode = $"class __Wrapper__ {{ {normalizedSignature} }}";
        var tree = CSharpSyntaxTree.ParseText(wrappedCode);
        var root = tree.GetCompilationUnitRoot();

        // Check for parse errors
        var diagnostics = tree.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
        if (diagnostics.Count > 0)
        {
            var errorMessages = string.Join("; ", diagnostics.Select(d => d.GetMessage()));
            throw new ArgumentException($"Failed to parse property signature: {errorMessages}", nameof(signature));
        }

        // Extract the property declaration
        var classDecl = root.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();
        var propertyDecl = classDecl?.Members.OfType<PropertyDeclarationSyntax>().FirstOrDefault();

        if (propertyDecl == null)
            throw new ArgumentException($"Could not find property declaration in signature: {signature}", nameof(signature));

        return BuildPropertyFromSyntax(propertyDecl);
    }

    private static PropertyBuilder BuildPropertyFromSyntax(PropertyDeclarationSyntax property)
    {
        var builder = PropertyBuilder.For(property.Identifier.Text, property.Type.ToString());

        // Apply modifiers
        builder = ApplyPropertyModifiers(builder, property.Modifiers);

        // Handle accessor list
        if (property.AccessorList != null)
        {
            var hasGetter = property.AccessorList.Accessors.Any(a => a.IsKind(SyntaxKind.GetAccessorDeclaration));
            var hasSetter = property.AccessorList.Accessors.Any(a => a.IsKind(SyntaxKind.SetAccessorDeclaration));

            if (hasGetter && hasSetter)
            {
                builder = builder.WithAutoPropertyAccessors();
            }
            else if (hasGetter && !hasSetter)
            {
                builder = builder.WithAutoPropertyAccessors().AsReadOnly();
            }
        }
        else if (property.ExpressionBody != null)
        {
            // Expression-bodied property: public string Name => "value";
            builder = builder.WithGetter(property.ExpressionBody.Expression.ToString());
        }

        // Handle initializer
        if (property.Initializer != null)
        {
            builder = builder.WithInitializer(property.Initializer.Value.ToString());
        }

        return builder;
    }

    private static PropertyBuilder ApplyPropertyModifiers(PropertyBuilder builder, SyntaxTokenList modifiers)
    {
        foreach (var modifier in modifiers)
        {
            builder = modifier.Kind() switch
            {
                SyntaxKind.PublicKeyword => builder.WithAccessibility(Accessibility.Public),
                SyntaxKind.PrivateKeyword => builder.WithAccessibility(Accessibility.Private),
                SyntaxKind.ProtectedKeyword => builder.WithAccessibility(Accessibility.Protected),
                SyntaxKind.InternalKeyword => builder.WithAccessibility(Accessibility.Internal),
                SyntaxKind.StaticKeyword => builder.AsStatic(),
                SyntaxKind.VirtualKeyword => builder.AsVirtual(),
                SyntaxKind.OverrideKeyword => builder.AsOverride(),
                SyntaxKind.AbstractKeyword => builder.AsAbstract(),
                _ => builder
            };
        }
        return builder;
    }

    #endregion

    #region Field Parsing

    /// <summary>
    /// Parses a field signature into a FieldBuilder.
    /// </summary>
    /// <param name="signature">The field signature (e.g., "private readonly string _name").</param>
    /// <returns>A configured FieldBuilder.</returns>
    /// <exception cref="ArgumentException">Thrown when the signature cannot be parsed.</exception>
    public static FieldBuilder ParseField(string signature)
    {
        if (string.IsNullOrWhiteSpace(signature))
            throw new ArgumentException("Field signature cannot be null or empty.", nameof(signature));

        // Ensure the signature ends with semicolon for proper parsing
        var normalizedSignature = signature.Trim();
        if (!normalizedSignature.EndsWith(";"))
            normalizedSignature += ";";

        // Wrap in a class context so Roslyn can parse it as a member
        var wrappedCode = $"class __Wrapper__ {{ {normalizedSignature} }}";
        var tree = CSharpSyntaxTree.ParseText(wrappedCode);
        var root = tree.GetCompilationUnitRoot();

        // Check for parse errors
        var diagnostics = tree.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
        if (diagnostics.Count > 0)
        {
            var errorMessages = string.Join("; ", diagnostics.Select(d => d.GetMessage()));
            throw new ArgumentException($"Failed to parse field signature: {errorMessages}", nameof(signature));
        }

        // Extract the field declaration
        var classDecl = root.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();
        var fieldDecl = classDecl?.Members.OfType<FieldDeclarationSyntax>().FirstOrDefault();

        if (fieldDecl == null)
            throw new ArgumentException($"Could not find field declaration in signature: {signature}", nameof(signature));

        return BuildFieldFromSyntax(fieldDecl);
    }

    private static FieldBuilder BuildFieldFromSyntax(FieldDeclarationSyntax field)
    {
        var variable = field.Declaration.Variables.First();
        var fieldName = variable.Identifier.Text;
        var fieldType = field.Declaration.Type.ToString();

        var builder = FieldBuilder.For(fieldName, fieldType);

        // Apply modifiers
        builder = ApplyFieldModifiers(builder, field.Modifiers);

        // Handle initializer
        if (variable.Initializer != null)
        {
            builder = builder.WithInitializer(variable.Initializer.Value.ToString());
        }

        return builder;
    }

    private static FieldBuilder ApplyFieldModifiers(FieldBuilder builder, SyntaxTokenList modifiers)
    {
        foreach (var modifier in modifiers)
        {
            builder = modifier.Kind() switch
            {
                SyntaxKind.PublicKeyword => builder.WithAccessibility(Accessibility.Public),
                SyntaxKind.PrivateKeyword => builder.WithAccessibility(Accessibility.Private),
                SyntaxKind.ProtectedKeyword => builder.WithAccessibility(Accessibility.Protected),
                SyntaxKind.InternalKeyword => builder.WithAccessibility(Accessibility.Internal),
                SyntaxKind.StaticKeyword => builder.AsStatic(),
                SyntaxKind.ReadOnlyKeyword => builder.AsReadonly(),
                SyntaxKind.ConstKeyword => builder.AsConst(),
                _ => builder
            };
        }
        return builder;
    }

    #endregion

    #region Type Parsing

    /// <summary>
    /// Parses a type signature into a TypeBuilder.
    /// </summary>
    /// <param name="signature">The type signature (e.g., "public class CustomerService : IService").</param>
    /// <returns>A configured TypeBuilder with parsed modifiers and base types.</returns>
    /// <exception cref="ArgumentException">Thrown when the signature cannot be parsed.</exception>
    public static TypeBuilder ParseType(string signature)
    {
        if (string.IsNullOrWhiteSpace(signature))
            throw new ArgumentException("Type signature cannot be null or empty.", nameof(signature));

        // Parse directly - type declarations are top-level
        var normalizedSignature = signature.Trim();
        
        // Add empty body if not present
        if (!normalizedSignature.Contains("{"))
        {
            normalizedSignature += " { }";
        }

        var tree = CSharpSyntaxTree.ParseText(normalizedSignature);
        var root = tree.GetCompilationUnitRoot();

        // Check for parse errors
        var diagnostics = tree.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
        if (diagnostics.Count > 0)
        {
            var errorMessages = string.Join("; ", diagnostics.Select(d => d.GetMessage()));
            throw new ArgumentException($"Failed to parse type signature: {errorMessages}", nameof(signature));
        }

        // Extract the type declaration
        var typeDecl = root.DescendantNodes().OfType<TypeDeclarationSyntax>().FirstOrDefault();

        if (typeDecl == null)
            throw new ArgumentException($"Could not find type declaration in signature: {signature}", nameof(signature));

        return BuildTypeFromSyntax(typeDecl);
    }

    private static TypeBuilder BuildTypeFromSyntax(TypeDeclarationSyntax type)
    {
        // Determine the type kind and create the appropriate builder
        var builder = type switch
        {
            ClassDeclarationSyntax => TypeBuilder.Class(type.Identifier.Text),
            InterfaceDeclarationSyntax => TypeBuilder.Interface(type.Identifier.Text),
            StructDeclarationSyntax => TypeBuilder.Struct(type.Identifier.Text),
            RecordDeclarationSyntax => TypeBuilder.Record(type.Identifier.Text),
            _ => throw new ArgumentException($"Unsupported type kind: {type.GetType().Name}")
        };

        // Apply modifiers
        builder = ApplyTypeModifiers(builder, type.Modifiers);

        // Add base types (interfaces and base class)
        if (type.BaseList != null)
        {
            foreach (var baseType in type.BaseList.Types)
            {
                builder = builder.Implements(baseType.Type.ToString());
            }
        }

        return builder;
    }

    private static TypeBuilder ApplyTypeModifiers(TypeBuilder builder, SyntaxTokenList modifiers)
    {
        foreach (var modifier in modifiers)
        {
            builder = modifier.Kind() switch
            {
                SyntaxKind.PublicKeyword => builder.WithAccessibility(Accessibility.Public),
                SyntaxKind.PrivateKeyword => builder.WithAccessibility(Accessibility.Private),
                SyntaxKind.ProtectedKeyword => builder.WithAccessibility(Accessibility.Protected),
                SyntaxKind.InternalKeyword => builder.WithAccessibility(Accessibility.Internal),
                SyntaxKind.StaticKeyword => builder.AsStatic(),
                SyntaxKind.AbstractKeyword => builder.AsAbstract(),
                SyntaxKind.SealedKeyword => builder.AsSealed(),
                SyntaxKind.PartialKeyword => builder.AsPartial(),
                _ => builder
            };
        }
        return builder;
    }

    #endregion
}
