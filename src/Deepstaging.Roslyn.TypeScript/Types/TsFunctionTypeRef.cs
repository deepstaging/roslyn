// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System.Collections.Immutable;
using System.Linq;

namespace Deepstaging.Roslyn.TypeScript.Types;

/// <summary>
/// A type-safe wrapper representing a TypeScript function type: <c>(param1: T1, param2: T2) =&gt; R</c>.
/// Carries parameter types and the return type for typed expression building.
/// </summary>
public readonly record struct TsFunctionTypeRef
{
    /// <summary>Gets the function parameters as (Type, Name) pairs.</summary>
    public ImmutableArray<(TsTypeRef Type, string Name)> Parameters { get; }

    /// <summary>Gets the return type of the function.</summary>
    public TsTypeRef ReturnType { get; }

    /// <summary>Creates a <c>TsFunctionTypeRef</c> with the given parameters and return type.</summary>
    /// <param name="parameters">The function parameters as (Type, Name) pairs.</param>
    /// <param name="returnType">The return type of the function.</param>
    public TsFunctionTypeRef(ImmutableArray<(TsTypeRef Type, string Name)> parameters, TsTypeRef returnType)
    {
        Parameters = parameters;
        ReturnType = returnType;
    }

    /// <summary>Gets the function type string: <c>(name: Type, ...) =&gt; ReturnType</c>.</summary>
    public override string ToString()
    {
        var parms = string.Join(", ", Parameters.Select(p => $"{p.Name}: {p.Type.Value}"));
        return $"({parms}) => {ReturnType.Value}";
    }

    /// <summary>Implicitly converts to <see cref="TsTypeRef"/> for use in type declarations.</summary>
    public static implicit operator TsTypeRef(TsFunctionTypeRef self) =>
        TsTypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(TsFunctionTypeRef self) =>
        self.ToString();
}
