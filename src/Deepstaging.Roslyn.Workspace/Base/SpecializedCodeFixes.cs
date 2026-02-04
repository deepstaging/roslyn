// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deepstaging.Roslyn;

/// <summary>
/// Base class for code fixes that operate on class declarations.
/// </summary>
public abstract class ClassCodeFix : SyntaxCodeFix<ClassDeclarationSyntax>;

/// <summary>
/// Base class for code fixes that operate on struct declarations.
/// </summary>
public abstract class StructCodeFix : SyntaxCodeFix<StructDeclarationSyntax>;

/// <summary>
/// Base class for code fixes that operate on interface declarations.
/// </summary>
public abstract class InterfaceCodeFix : SyntaxCodeFix<InterfaceDeclarationSyntax>;

/// <summary>
/// Base class for code fixes that operate on record declarations.
/// </summary>
public abstract class RecordCodeFix : SyntaxCodeFix<RecordDeclarationSyntax>;

/// <summary>
/// Base class for code fixes that operate on enum declarations.
/// </summary>
public abstract class EnumCodeFix : SyntaxCodeFix<EnumDeclarationSyntax>;

/// <summary>
/// Base class for code fixes that operate on method declarations.
/// </summary>
public abstract class MethodCodeFix : SyntaxCodeFix<MethodDeclarationSyntax>;

/// <summary>
/// Base class for code fixes that operate on property declarations.
/// </summary>
public abstract class PropertyCodeFix : SyntaxCodeFix<PropertyDeclarationSyntax>;

/// <summary>
/// Base class for code fixes that operate on field declarations.
/// </summary>
public abstract class FieldCodeFix : SyntaxCodeFix<FieldDeclarationSyntax>;

/// <summary>
/// Base class for code fixes that operate on constructor declarations.
/// </summary>
public abstract class ConstructorCodeFix : SyntaxCodeFix<ConstructorDeclarationSyntax>;

/// <summary>
/// Base class for code fixes that operate on event declarations.
/// </summary>
public abstract class EventCodeFix : SyntaxCodeFix<EventDeclarationSyntax>;

/// <summary>
/// Base class for code fixes that operate on parameter syntax.
/// </summary>
public abstract class ParameterCodeFix : SyntaxCodeFix<ParameterSyntax>;
