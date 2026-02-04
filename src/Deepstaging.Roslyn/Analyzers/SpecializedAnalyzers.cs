// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Analyzers;

/// <summary>
/// Base class for analyzers that report diagnostics on type symbols (classes, structs, interfaces, etc.).
/// </summary>
public abstract class TypeAnalyzer : SymbolAnalyzer<INamedTypeSymbol>;

/// <summary>
/// Base class for analyzers that report diagnostics on method symbols.
/// </summary>
public abstract class MethodAnalyzer : SymbolAnalyzer<IMethodSymbol>;

/// <summary>
/// Base class for analyzers that report diagnostics on property symbols.
/// </summary>
public abstract class PropertyAnalyzer : SymbolAnalyzer<IPropertySymbol>;

/// <summary>
/// Base class for analyzers that report diagnostics on field symbols.
/// </summary>
public abstract class FieldAnalyzer : SymbolAnalyzer<IFieldSymbol>;

/// <summary>
/// Base class for analyzers that report diagnostics on event symbols.
/// </summary>
public abstract class EventAnalyzer : SymbolAnalyzer<IEventSymbol>;

/// <summary>
/// Base class for analyzers that report diagnostics on parameter symbols.
/// </summary>
public abstract class ParameterAnalyzer : SymbolAnalyzer<IParameterSymbol>;
