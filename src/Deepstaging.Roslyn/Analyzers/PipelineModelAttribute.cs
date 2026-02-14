// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

// ReSharper disable once CheckNamespace
namespace Deepstaging.Roslyn;

/// <summary>
/// Marks a sealed record as a pipeline model used in incremental source generators.
/// Pipeline models must have correct equality semantics for caching to work:
/// <list type="bullet">
/// <item>Use <see cref="EquatableArray{T}"/> instead of <see cref="System.Collections.Immutable.ImmutableArray{T}"/></item>
/// <item>Do not store <c>ValidSymbol&lt;T&gt;</c> — use snapshots (<see cref="TypeSnapshot"/>, <see cref="MethodSnapshot"/>, etc.)</item>
/// <item>Do not store <c>ISymbol</c> — extract data during the projection step</item>
/// <item>All field types must implement <see cref="IEquatable{T}"/></item>
/// </list>
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class PipelineModelAttribute : Attribute;
