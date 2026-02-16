// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit;

/// <summary>
/// Represents an interface implementation that may be conditionally compiled.
/// </summary>
/// <param name="Name">The interface name (e.g., "ISpanFormattable", "IEquatable&lt;T&gt;").</param>
/// <param name="Condition">Optional preprocessor directive condition. When null, the interface is always included.</param>
public readonly record struct ConditionalInterface(string Name, Directive? Condition = null)
{
    /// <summary>
    /// Creates an unconditional interface implementation.
    /// </summary>
    /// <param name="name">The interface name.</param>
    public static implicit operator ConditionalInterface(string name) => new(name);
}