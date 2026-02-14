// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn;

/// <summary>
/// Snapshot terminal methods for query builders.
/// Provides <c>.Snapshots()</c> and <c>.Snapshot()</c> terminals that materialize
/// query results into pipeline-safe snapshot types.
/// </summary>
public static class QuerySnapshotExtensions
{
    extension(TypeQuery query)
    {
        /// <summary>
        /// Materializes query results into pipeline-safe <see cref="TypeSnapshot"/> instances.
        /// </summary>
        public EquatableArray<TypeSnapshot> Snapshots()
            => query.GetAll().SelectEquatable(t => t.ToSnapshot());

        /// <summary>
        /// Materializes the first matching result into a <see cref="TypeSnapshot"/>, or null.
        /// </summary>
        public TypeSnapshot? Snapshot()
            => query.FirstOrDefault().OrNull() is { } symbol
                ? ValidSymbol<INamedTypeSymbol>.From(symbol).ToSnapshot()
                : null;
    }

    extension(MethodQuery query)
    {
        /// <summary>
        /// Materializes query results into pipeline-safe <see cref="MethodSnapshot"/> instances.
        /// </summary>
        public EquatableArray<MethodSnapshot> Snapshots()
            => query.GetAll().SelectEquatable(m => m.ToSnapshot());

        /// <summary>
        /// Materializes the first matching result into a <see cref="MethodSnapshot"/>, or null.
        /// </summary>
        public MethodSnapshot? Snapshot()
            => query.FirstOrDefault().OrNull() is { } symbol
                ? ValidSymbol<IMethodSymbol>.From(symbol).ToSnapshot()
                : null;
    }

    extension(PropertyQuery query)
    {
        /// <summary>
        /// Materializes query results into pipeline-safe <see cref="PropertySnapshot"/> instances.
        /// </summary>
        public EquatableArray<PropertySnapshot> Snapshots()
            => query.GetAll().SelectEquatable(p => p.ToSnapshot());

        /// <summary>
        /// Materializes the first matching result into a <see cref="PropertySnapshot"/>, or null.
        /// </summary>
        public PropertySnapshot? Snapshot()
            => query.FirstOrDefault().OrNull() is { } symbol
                ? ValidSymbol<IPropertySymbol>.From(symbol).ToSnapshot()
                : null;
    }

    extension(FieldQuery query)
    {
        /// <summary>
        /// Materializes query results into pipeline-safe <see cref="FieldSnapshot"/> instances.
        /// </summary>
        public EquatableArray<FieldSnapshot> Snapshots()
            => query.GetAll().SelectEquatable(f => f.ToSnapshot());

        /// <summary>
        /// Materializes the first matching result into a <see cref="FieldSnapshot"/>, or null.
        /// </summary>
        public FieldSnapshot? Snapshot()
            => query.FirstOrDefault().OrNull() is { } symbol
                ? ValidSymbol<IFieldSymbol>.From(symbol).ToSnapshot()
                : null;
    }

    extension(ParameterQuery query)
    {
        /// <summary>
        /// Materializes query results into pipeline-safe <see cref="ParameterSnapshot"/> instances.
        /// </summary>
        public EquatableArray<ParameterSnapshot> Snapshots()
            => query.GetAll().SelectEquatable(p => p.ToSnapshot());

        /// <summary>
        /// Materializes the first matching result into a <see cref="ParameterSnapshot"/>, or null.
        /// </summary>
        public ParameterSnapshot? Snapshot()
            => query.FirstOrDefault().OrNull() is { } symbol
                ? ValidSymbol<IParameterSymbol>.From(symbol).ToSnapshot()
                : null;
    }

    extension(EventQuery query)
    {
        /// <summary>
        /// Materializes query results into pipeline-safe <see cref="EventSnapshot"/> instances.
        /// </summary>
        public EquatableArray<EventSnapshot> Snapshots()
            => query.GetAll().SelectEquatable(e => e.ToSnapshot());

        /// <summary>
        /// Materializes the first matching result into an <see cref="EventSnapshot"/>, or null.
        /// </summary>
        public EventSnapshot? Snapshot()
            => query.FirstOrDefault().OrNull() is { } symbol
                ? ValidSymbol<IEventSymbol>.From(symbol).ToSnapshot()
                : null;
    }

    extension(ConstructorQuery query)
    {
        /// <summary>
        /// Materializes query results into pipeline-safe <see cref="MethodSnapshot"/> instances.
        /// </summary>
        public EquatableArray<MethodSnapshot> Snapshots()
            => query.GetAll().SelectEquatable(c => c.ToSnapshot());

        /// <summary>
        /// Materializes the first matching result into a <see cref="MethodSnapshot"/>, or null.
        /// </summary>
        public MethodSnapshot? Snapshot()
            => query.FirstOrDefault().OrNull() is { } symbol
                ? ValidSymbol<IMethodSymbol>.From(symbol).ToSnapshot()
                : null;
    }
}
