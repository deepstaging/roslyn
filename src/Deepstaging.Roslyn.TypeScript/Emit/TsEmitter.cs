// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System;
using System.Collections.Immutable;
using System.Text;

namespace Deepstaging.Roslyn.TypeScript.Emit;

/// <summary>
/// Core rendering engine that converts <see cref="TsTypeBuilder"/> instances into formatted TypeScript source code.
/// </summary>
internal static class TsEmitter
{
    internal static TsOptionalEmit Emit(TsTypeBuilder builder, TsEmitOptions options)
    {
        var diagnostics = ImmutableArray.CreateBuilder<string>();

        if (string.IsNullOrWhiteSpace(builder.Name))
        {
            diagnostics.Add("error: Type name is required.");
            return new TsOptionalEmit(null, diagnostics.ToImmutable());
        }

        try
        {
            var sb = new StringBuilder();
            var eol = options.EndOfLine;

            // Header comment
            if (options.HeaderComment != null)
            {
                sb.Append(options.HeaderComment);
                sb.Append(eol);
                sb.Append(eol);
            }

            // Imports
            if (!builder.Imports.IsDefaultOrEmpty)
            {
                foreach (var import in builder.Imports)
                {
                    sb.Append(import);
                    sb.Append(eol);
                }

                sb.Append(eol);
            }

            EmitType(sb, builder, options, indent: "", diagnostics);

            // Ensure file ends with a newline
            var code = sb.ToString();
            if (!code.EndsWith(eol, StringComparison.Ordinal))
                code += eol;

            // Optional formatting
            if (options.FormatOutput)
                code = TsFormatter.Format(code, options);

            // Optional tsc validation
            if (options.ValidationLevel >= TsValidationLevel.Syntax)
            {
                var tscDiagnostics = TsValidator.Validate(code, options.TscPath);
                diagnostics.AddRange(tscDiagnostics);
            }

            return new TsOptionalEmit(code, diagnostics.ToImmutable());
        }
        catch (Exception ex)
        {
            diagnostics.Add($"error: {ex.Message}");
            return new TsOptionalEmit(null, diagnostics.ToImmutable());
        }
    }

    private static void EmitType(
        StringBuilder sb,
        TsTypeBuilder builder,
        TsEmitOptions options,
        string indent,
        ImmutableArray<string>.Builder diagnostics)
    {
        var eol = options.EndOfLine;
        var semi = options.UseSemicolons ? ";" : "";
        var innerIndent = indent + options.Indentation;

        // JSDoc comment
        if (builder.JsDocComment != null)
        {
            sb.Append(indent);
            sb.Append("/**");
            sb.Append(eol);
            foreach (var line in builder.JsDocComment.Split('\n'))
            {
                sb.Append(indent);
                sb.Append(" * ");
                sb.Append(line.TrimEnd('\r'));
                sb.Append(eol);
            }

            sb.Append(indent);
            sb.Append(" */");
            sb.Append(eol);
        }

        // Decorators
        if (!builder.Decorators.IsDefaultOrEmpty)
        {
            foreach (var decorator in builder.Decorators)
            {
                sb.Append(indent);
                if (!decorator.StartsWith("@", StringComparison.Ordinal))
                    sb.Append('@');
                sb.Append(decorator);
                sb.Append(eol);
            }
        }

        switch (builder.Kind)
        {
            case TsTypeKind.TypeAlias:
                EmitTypeAlias(sb, builder, options, indent, semi, eol);
                break;
            case TsTypeKind.Enum:
            case TsTypeKind.ConstEnum:
                EmitEnum(sb, builder, options, indent, innerIndent, eol);
                break;
            default:
                EmitClassOrInterface(sb, builder, options, indent, innerIndent, semi, eol, diagnostics);
                break;
        }
    }

    private static void EmitTypeAlias(
        StringBuilder sb,
        TsTypeBuilder builder,
        TsEmitOptions options,
        string indent,
        string semi,
        string eol)
    {
        sb.Append(indent);
        AppendExportModifiers(sb, builder);

        if (builder.IsDeclare)
            sb.Append("declare ");

        sb.Append("type ");
        sb.Append(builder.Name);
        AppendTypeParameters(sb, builder);
        sb.Append(" = ");
        sb.Append(builder.TypeAliasDefinition ?? "unknown");
        sb.Append(semi);
        sb.Append(eol);
    }

    private static void EmitEnum(
        StringBuilder sb,
        TsTypeBuilder builder,
        TsEmitOptions options,
        string indent,
        string innerIndent,
        string eol)
    {
        var semi = options.UseSemicolons ? ";" : "";
        var trailingComma = options.UseTrailingCommas ? "," : "";

        sb.Append(indent);
        AppendExportModifiers(sb, builder);

        if (builder.IsDeclare)
            sb.Append("declare ");

        if (builder.Kind == TsTypeKind.ConstEnum)
            sb.Append("const ");

        sb.Append("enum ");
        sb.Append(builder.Name);
        sb.Append(" {");
        sb.Append(eol);

        if (!builder.EnumMembers.IsDefaultOrEmpty)
        {
            for (var i = 0; i < builder.EnumMembers.Length; i++)
            {
                var (name, value) = builder.EnumMembers[i];
                sb.Append(innerIndent);
                sb.Append(name);

                if (value != null)
                {
                    sb.Append(" = ");
                    sb.Append(value);
                }

                if (i < builder.EnumMembers.Length - 1 || options.UseTrailingCommas)
                    sb.Append(',');

                sb.Append(eol);
            }
        }

        sb.Append(indent);
        sb.Append('}');
        sb.Append(eol);
    }

    private static void EmitClassOrInterface(
        StringBuilder sb,
        TsTypeBuilder builder,
        TsEmitOptions options,
        string indent,
        string innerIndent,
        string semi,
        string eol,
        ImmutableArray<string>.Builder diagnostics)
    {
        // Declaration line
        sb.Append(indent);
        AppendExportModifiers(sb, builder);

        if (builder.IsDeclare)
            sb.Append("declare ");

        if (builder.IsAbstract && builder.Kind == TsTypeKind.Class)
            sb.Append("abstract ");

        sb.Append(builder.Kind == TsTypeKind.Interface ? "interface " : "class ");
        sb.Append(builder.Name);
        AppendTypeParameters(sb, builder);

        // Extends
        if (!builder.ExtendsClause.IsDefaultOrEmpty)
        {
            sb.Append(" extends ");
            sb.Append(string.Join(", ", builder.ExtendsClause));
        }

        // Implements (class only)
        if (!builder.ImplementsClause.IsDefaultOrEmpty && builder.Kind == TsTypeKind.Class)
        {
            sb.Append(" implements ");
            sb.Append(string.Join(", ", builder.ImplementsClause));
        }

        sb.Append(" {");
        sb.Append(eol);

        var needsBlankLine = false;

        // Index signatures
        if (!builder.IndexSignatures.IsDefaultOrEmpty)
        {
            foreach (var sig in builder.IndexSignatures)
            {
                sb.Append(innerIndent);
                sb.Append(sig);
                sb.Append(semi);
                sb.Append(eol);
            }

            needsBlankLine = true;
        }

        // Fields
        if (!builder.Fields.IsDefaultOrEmpty)
        {
            if (needsBlankLine)
                sb.Append(eol);

            foreach (var field in builder.Fields)
            {
                EmitField(sb, field, options, innerIndent, semi, eol);
            }

            needsBlankLine = true;
        }

        // Constructors
        if (!builder.Constructors.IsDefaultOrEmpty)
        {
            if (needsBlankLine)
                sb.Append(eol);

            foreach (var ctor in builder.Constructors)
            {
                EmitConstructor(sb, ctor, options, innerIndent, semi, eol);
            }

            needsBlankLine = true;
        }

        // Properties (getters/setters)
        if (!builder.Properties.IsDefaultOrEmpty)
        {
            if (needsBlankLine)
                sb.Append(eol);

            foreach (var prop in builder.Properties)
            {
                EmitProperty(sb, prop, builder.Kind, options, innerIndent, semi, eol);
            }

            needsBlankLine = true;
        }

        // Methods
        if (!builder.Methods.IsDefaultOrEmpty)
        {
            if (needsBlankLine)
                sb.Append(eol);

            for (var i = 0; i < builder.Methods.Length; i++)
            {
                if (i > 0)
                    sb.Append(eol);
                EmitMethod(sb, builder.Methods[i], builder.Kind, options, innerIndent, semi, eol);
            }
        }

        // Nested types
        if (!builder.NestedTypes.IsDefaultOrEmpty)
        {
            if (needsBlankLine)
                sb.Append(eol);

            foreach (var nested in builder.NestedTypes)
            {
                EmitType(sb, nested, options, innerIndent, diagnostics);
            }
        }

        sb.Append(indent);
        sb.Append('}');
        sb.Append(eol);
    }

    private static void EmitField(
        StringBuilder sb,
        TsFieldBuilder field,
        TsEmitOptions options,
        string indent,
        string semi,
        string eol)
    {
        sb.Append(indent);
        AppendAccessibility(sb, field.Accessibility);

        if (field.IsAbstract)
            sb.Append("abstract ");
        if (field.IsDeclare)
            sb.Append("declare ");
        if (field.IsOverride)
            sb.Append("override ");
        if (field.IsStatic)
            sb.Append("static ");
        if (field.IsReadonly)
            sb.Append("readonly ");

        if (field.IsEsPrivate)
            sb.Append('#');

        sb.Append(field.Name);

        if (field.IsOptional)
            sb.Append('?');

        sb.Append(": ");
        sb.Append(field.Type);

        if (field.Initializer != null)
        {
            sb.Append(" = ");
            sb.Append(field.Initializer);
        }

        sb.Append(semi);
        sb.Append(eol);
    }

    private static void EmitConstructor(
        StringBuilder sb,
        TsConstructorBuilder ctor,
        TsEmitOptions options,
        string indent,
        string semi,
        string eol)
    {
        var bodyIndent = indent + options.Indentation;

        // Overload signatures
        if (!ctor.OverloadSignatures.IsDefaultOrEmpty)
        {
            foreach (var sig in ctor.OverloadSignatures)
            {
                sb.Append(indent);
                sb.Append(sig);
                sb.Append(semi);
                sb.Append(eol);
            }
        }

        sb.Append(indent);
        AppendAccessibility(sb, ctor.Accessibility);
        sb.Append("constructor(");
        AppendParameters(sb, ctor.Parameters, options);
        sb.Append(')');

        if (ctor.Body == null && ctor.SuperArguments.IsDefaultOrEmpty)
        {
            sb.Append(" {}");
            sb.Append(eol);
            return;
        }

        sb.Append(" {");
        sb.Append(eol);

        // super() call
        if (!ctor.SuperArguments.IsDefaultOrEmpty)
        {
            sb.Append(bodyIndent);
            sb.Append("super(");
            sb.Append(string.Join(", ", ctor.SuperArguments));
            sb.Append(')');
            sb.Append(semi);
            sb.Append(eol);
        }

        // Body statements
        if (ctor.Body != null)
            EmitBodyStatements(sb, ctor.Body.Value, options, bodyIndent, semi, eol);

        sb.Append(indent);
        sb.Append('}');
        sb.Append(eol);
    }

    private static void EmitProperty(
        StringBuilder sb,
        TsPropertyBuilder prop,
        TsTypeKind parentKind,
        TsEmitOptions options,
        string indent,
        string semi,
        string eol)
    {
        var hasAccessor = prop.GetterExpression != null || prop.GetterBody != null || prop.SetterBody != null;

        if (hasAccessor)
        {
            EmitPropertyAccessors(sb, prop, options, indent, semi, eol);
            return;
        }

        // Simple property declaration (interface or class)
        sb.Append(indent);
        AppendAccessibility(sb, prop.Accessibility);

        if (prop.IsAbstract)
            sb.Append("abstract ");
        if (prop.IsOverride)
            sb.Append("override ");
        if (prop.IsStatic)
            sb.Append("static ");
        if (prop.IsReadonly)
            sb.Append("readonly ");

        sb.Append(prop.Name);

        if (prop.IsOptional)
            sb.Append('?');

        sb.Append(": ");
        sb.Append(prop.Type);

        if (prop.Initializer != null)
        {
            sb.Append(" = ");
            sb.Append(prop.Initializer);
        }

        sb.Append(semi);
        sb.Append(eol);
    }

    private static void EmitPropertyAccessors(
        StringBuilder sb,
        TsPropertyBuilder prop,
        TsEmitOptions options,
        string indent,
        string semi,
        string eol)
    {
        var bodyIndent = indent + options.Indentation;

        // Getter
        if (prop.GetterExpression != null || prop.GetterBody != null)
        {
            sb.Append(indent);
            AppendAccessibility(sb, prop.Accessibility);

            if (prop.IsStatic)
                sb.Append("static ");

            sb.Append("get ");
            sb.Append(prop.Name);
            sb.Append("(): ");
            sb.Append(prop.Type);
            sb.Append(" {");
            sb.Append(eol);

            if (prop.GetterExpression != null)
            {
                sb.Append(bodyIndent);
                sb.Append("return ");
                sb.Append(prop.GetterExpression);
                sb.Append(semi);
                sb.Append(eol);
            }
            else if (prop.GetterBody != null)
            {
                EmitBodyStatements(sb, prop.GetterBody.Value, options, bodyIndent, semi, eol);
            }

            sb.Append(indent);
            sb.Append('}');
            sb.Append(eol);
        }

        // Setter
        if (prop.SetterBody != null)
        {
            sb.Append(indent);
            AppendAccessibility(sb, prop.Accessibility);

            if (prop.IsStatic)
                sb.Append("static ");

            sb.Append("set ");
            sb.Append(prop.Name);
            sb.Append("(value: ");
            sb.Append(prop.Type);
            sb.Append(") {");
            sb.Append(eol);

            EmitBodyStatements(sb, prop.SetterBody.Value, options, bodyIndent, semi, eol);

            sb.Append(indent);
            sb.Append('}');
            sb.Append(eol);
        }
    }

    private static void EmitMethod(
        StringBuilder sb,
        TsMethodBuilder method,
        TsTypeKind parentKind,
        TsEmitOptions options,
        string indent,
        string semi,
        string eol)
    {
        var bodyIndent = indent + options.Indentation;

        // Decorators
        if (!method.Decorators.IsDefaultOrEmpty)
        {
            foreach (var decorator in method.Decorators)
            {
                sb.Append(indent);
                if (!decorator.StartsWith("@", StringComparison.Ordinal))
                    sb.Append('@');
                sb.Append(decorator);
                sb.Append(eol);
            }
        }

        // Overload signatures
        if (!method.OverloadSignatures.IsDefaultOrEmpty)
        {
            foreach (var sig in method.OverloadSignatures)
            {
                sb.Append(indent);
                sb.Append(sig);
                sb.Append(semi);
                sb.Append(eol);
            }
        }

        sb.Append(indent);
        AppendAccessibility(sb, method.Accessibility);

        if (method.IsAbstract)
            sb.Append("abstract ");
        if (method.IsOverride)
            sb.Append("override ");
        if (method.IsStatic)
            sb.Append("static ");
        if (method.IsAsync)
            sb.Append("async ");
        if (method.IsGenerator)
            sb.Append('*');

        sb.Append(method.Name);

        if (method.IsOptional)
            sb.Append('?');

        AppendMethodTypeParameters(sb, method);

        sb.Append('(');
        AppendParameters(sb, method.Parameters, options);
        sb.Append(')');

        if (method.ReturnType != null)
        {
            sb.Append(": ");
            sb.Append(method.ReturnType);
        }

        // Abstract or interface members without a body
        if (method.IsAbstract || (parentKind == TsTypeKind.Interface && method.Body == null && method.ExpressionBody == null))
        {
            sb.Append(semi);
            sb.Append(eol);
            return;
        }

        // Expression body
        if (method.ExpressionBody != null)
        {
            sb.Append(" {");
            sb.Append(eol);
            sb.Append(bodyIndent);
            sb.Append("return ");
            sb.Append(method.ExpressionBody);
            sb.Append(semi);
            sb.Append(eol);
            sb.Append(indent);
            sb.Append('}');
            sb.Append(eol);
            return;
        }

        // Block body
        if (method.Body != null)
        {
            sb.Append(" {");
            sb.Append(eol);
            EmitBodyStatements(sb, method.Body.Value, options, bodyIndent, semi, eol);
            sb.Append(indent);
            sb.Append('}');
            sb.Append(eol);
            return;
        }

        // Empty body
        sb.Append(" {}");
        sb.Append(eol);
    }

    private static void EmitBodyStatements(
        StringBuilder sb,
        TsBodyBuilder body,
        TsEmitOptions options,
        string indent,
        string semi,
        string eol)
    {
        if (body.IsEmpty)
            return;

        foreach (var statement in body.Statements)
        {
            // Multi-line statements (e.g., from control flow) need inner indentation
            var lines = statement.Split('\n');
            for (var i = 0; i < lines.Length; i++)
            {
                sb.Append(indent);
                sb.Append(lines[i].TrimEnd('\r'));
                sb.Append(eol);
            }
        }
    }

    private static void AppendExportModifiers(StringBuilder sb, TsTypeBuilder builder)
    {
        if (builder.IsDefaultExport)
        {
            sb.Append("export default ");
        }
        else if (builder.IsExported)
        {
            sb.Append("export ");
        }
    }

    private static void AppendTypeParameters(StringBuilder sb, TsTypeBuilder builder)
    {
        if (builder.TypeParameters.IsDefaultOrEmpty)
            return;

        sb.Append('<');
        sb.Append(string.Join(", ", builder.TypeParameters));
        sb.Append('>');
    }

    private static void AppendMethodTypeParameters(StringBuilder sb, TsMethodBuilder method)
    {
        if (method.TypeParameters.IsDefaultOrEmpty)
            return;

        sb.Append('<');
        sb.Append(string.Join(", ", method.TypeParameters));
        sb.Append('>');
    }

    private static void AppendAccessibility(StringBuilder sb, TsAccessibility accessibility)
    {
        switch (accessibility)
        {
            case TsAccessibility.Public:
                sb.Append("public ");
                break;
            case TsAccessibility.Private:
                sb.Append("private ");
                break;
            case TsAccessibility.Protected:
                sb.Append("protected ");
                break;
        }
    }

    private static void AppendParameters(
        StringBuilder sb,
        ImmutableArray<TsParameterBuilder> parameters,
        TsEmitOptions options)
    {
        if (parameters.IsDefaultOrEmpty)
            return;

        for (var i = 0; i < parameters.Length; i++)
        {
            if (i > 0)
                sb.Append(", ");

            var param = parameters[i];

            // Parameter property accessibility
            if (param.ParameterPropertyAccessibility != null)
            {
                switch (param.ParameterPropertyAccessibility.Value)
                {
                    case TsAccessibility.Public:
                        sb.Append("public ");
                        break;
                    case TsAccessibility.Private:
                        sb.Append("private ");
                        break;
                    case TsAccessibility.Protected:
                        sb.Append("protected ");
                        break;
                }
            }

            if (param.IsReadonlyParameterProperty)
                sb.Append("readonly ");

            if (param.IsRest)
                sb.Append("...");

            sb.Append(param.Name);

            if (param.IsOptional)
                sb.Append('?');

            sb.Append(": ");
            sb.Append(param.Type);

            if (param.DefaultValue != null)
            {
                sb.Append(" = ");
                sb.Append(param.DefaultValue);
            }
        }
    }
}
