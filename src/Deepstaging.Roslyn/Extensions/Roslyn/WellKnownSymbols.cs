// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable InconsistentNaming

namespace Deepstaging.Roslyn;

/// <summary>
/// Provides access to well-known symbols from the compilation.
/// Access via SemanticModel.WellKnownSymbols property.
/// </summary>
public static class CommonCompilationSymbols
{
    extension(SemanticModel model)
    {
        /// <summary>
        /// Gets access to well-known symbols for this compilation.
        /// </summary>
        public WellKnownSymbols WellKnownSymbols => new(model.Compilation);
    }

    extension(Compilation compilation)
    {
        /// <summary>
        /// Gets access to well-known symbols for this compilation.
        /// </summary>
        public WellKnownSymbols WellKnownSymbols => new(compilation);
    }

    /// <summary>
    /// Lazily-loaded well-known symbols from a compilation.
    /// Provides cached access to commonly used framework types as ValidSymbol projections.
    /// </summary>
    public sealed class WellKnownSymbols(Compilation compilation)
    {
        private INamedTypeSymbol? _string;

        /// <summary>
        /// Gets the System.String type symbol.
        /// </summary>
        public ValidSymbol<INamedTypeSymbol> String => ValidSymbol<INamedTypeSymbol>.From(
            _string ??= compilation.GetTypeByMetadataName("System.String")
                        ?? throw new InvalidOperationException("System.String not found in compilation"));

        private INamedTypeSymbol? _int32;

        /// <summary>
        /// Gets the System.Int32 type symbol.
        /// </summary>
        public ValidSymbol<INamedTypeSymbol> Int32 => ValidSymbol<INamedTypeSymbol>.From(
            _int32 ??= compilation.GetTypeByMetadataName("System.Int32")
                       ?? throw new InvalidOperationException("System.Int32 not found in compilation"));

        private INamedTypeSymbol? _int64;

        /// <summary>
        /// Gets the System.Int64 type symbol.
        /// </summary>
        public ValidSymbol<INamedTypeSymbol> Int64 => ValidSymbol<INamedTypeSymbol>.From(
            _int64 ??= compilation.GetTypeByMetadataName("System.Int64")
                       ?? throw new InvalidOperationException("System.Int64 not found in compilation"));

        private INamedTypeSymbol? _int16;

        /// <summary>
        /// Gets the System.Int16 type symbol.
        /// </summary>
        public ValidSymbol<INamedTypeSymbol> Int16 => ValidSymbol<INamedTypeSymbol>.From(
            _int16 ??= compilation.GetTypeByMetadataName("System.Int16")
                       ?? throw new InvalidOperationException("System.Int16 not found in compilation"));

        private INamedTypeSymbol? _byte;

        /// <summary>
        /// Gets the System.Byte type symbol.
        /// </summary>
        public ValidSymbol<INamedTypeSymbol> Byte => ValidSymbol<INamedTypeSymbol>.From(
            _byte ??= compilation.GetTypeByMetadataName("System.Byte")
                      ?? throw new InvalidOperationException("System.Byte not found in compilation"));

        private INamedTypeSymbol? _single;

        /// <summary>
        /// Gets the System.Single type symbol.
        /// </summary>
        public ValidSymbol<INamedTypeSymbol> Single => ValidSymbol<INamedTypeSymbol>.From(
            _single ??= compilation.GetTypeByMetadataName("System.Single")
                        ?? throw new InvalidOperationException("System.Single not found in compilation"));

        private INamedTypeSymbol? _double;

        /// <summary>
        /// Gets the System.Double type symbol.
        /// </summary>
        public ValidSymbol<INamedTypeSymbol> Double => ValidSymbol<INamedTypeSymbol>.From(
            _double ??= compilation.GetTypeByMetadataName("System.Double")
                        ?? throw new InvalidOperationException("System.Double not found in compilation"));

        private INamedTypeSymbol? _decimal;

        /// <summary>
        /// Gets the System.Decimal type symbol.
        /// </summary>
        public ValidSymbol<INamedTypeSymbol> Decimal => ValidSymbol<INamedTypeSymbol>.From(
            _decimal ??= compilation.GetTypeByMetadataName("System.Decimal")
                         ?? throw new InvalidOperationException("System.Decimal not found in compilation"));

        private INamedTypeSymbol? _char;

        /// <summary>
        /// Gets the System.Char type symbol.
        /// </summary>
        public ValidSymbol<INamedTypeSymbol> Char => ValidSymbol<INamedTypeSymbol>.From(
            _char ??= compilation.GetTypeByMetadataName("System.Char")
                      ?? throw new InvalidOperationException("System.Char not found in compilation"));

        private INamedTypeSymbol? _boolean;

        /// <summary>
        /// Gets the System.Boolean type symbol.
        /// </summary>
        public ValidSymbol<INamedTypeSymbol> Boolean => ValidSymbol<INamedTypeSymbol>.From(
            _boolean ??= compilation.GetTypeByMetadataName("System.Boolean")
                         ?? throw new InvalidOperationException("System.Boolean not found in compilation"));

        private INamedTypeSymbol? _object;

        /// <summary>
        /// Gets the System.Object type symbol.
        /// </summary>
        public ValidSymbol<INamedTypeSymbol> Object => ValidSymbol<INamedTypeSymbol>.From(
            _object ??= compilation.GetTypeByMetadataName("System.Object")
                        ?? throw new InvalidOperationException("System.Object not found in compilation"));

        private INamedTypeSymbol? _void;

        /// <summary>
        /// Gets the System.Void type symbol.
        /// </summary>
        public ValidSymbol<INamedTypeSymbol> Void => ValidSymbol<INamedTypeSymbol>.From(
            _void ??= compilation.GetTypeByMetadataName("System.Void")
                      ?? throw new InvalidOperationException("System.Void not found in compilation"));

        private INamedTypeSymbol? _dateTime;

        /// <summary>
        /// Gets the System.DateTime type symbol.
        /// </summary>
        public ValidSymbol<INamedTypeSymbol> DateTime => ValidSymbol<INamedTypeSymbol>.From(
            _dateTime ??= compilation.GetTypeByMetadataName("System.DateTime")
                          ?? throw new InvalidOperationException("System.DateTime not found in compilation"));

        private INamedTypeSymbol? _dateTimeOffset;

        /// <summary>
        /// Gets the System.DateTimeOffset type symbol.
        /// </summary>
        public ValidSymbol<INamedTypeSymbol> DateTimeOffset => ValidSymbol<INamedTypeSymbol>.From(
            _dateTimeOffset ??= compilation.GetTypeByMetadataName("System.DateTimeOffset")
                                ?? throw new InvalidOperationException("System.DateTimeOffset not found in compilation"));

        private INamedTypeSymbol? _timeSpan;

        /// <summary>
        /// Gets the System.TimeSpan type symbol.
        /// </summary>
        public ValidSymbol<INamedTypeSymbol> TimeSpan => ValidSymbol<INamedTypeSymbol>.From(
            _timeSpan ??= compilation.GetTypeByMetadataName("System.TimeSpan")
                          ?? throw new InvalidOperationException("System.TimeSpan not found in compilation"));

        private INamedTypeSymbol? _guid;

        /// <summary>
        /// Gets the System.Guid type symbol.
        /// </summary>
        public ValidSymbol<INamedTypeSymbol> Guid => ValidSymbol<INamedTypeSymbol>.From(
            _guid ??= compilation.GetTypeByMetadataName("System.Guid")
                      ?? throw new InvalidOperationException("System.Guid not found in compilation"));

        private INamedTypeSymbol? _cancellationToken;

        /// <summary>
        /// Gets the System.Threading.CancellationToken type symbol.
        /// </summary>
        public ValidSymbol<INamedTypeSymbol> CancellationToken => ValidSymbol<INamedTypeSymbol>.From(
            _cancellationToken ??= compilation.GetTypeByMetadataName("System.Threading.CancellationToken")
                                   ?? throw new InvalidOperationException("System.Threading.CancellationToken not found in compilation"));

        private INamedTypeSymbol? _task;

        /// <summary>
        /// Gets the System.Threading.Tasks.Task type symbol.
        /// </summary>
        public ValidSymbol<INamedTypeSymbol> Task => ValidSymbol<INamedTypeSymbol>.From(
            _task ??= compilation.GetTypeByMetadataName("System.Threading.Tasks.Task")
                      ?? throw new InvalidOperationException("System.Threading.Tasks.Task not found in compilation"));

        private INamedTypeSymbol? _taskOfT;

        /// <summary>
        /// Gets the System.Threading.Tasks.Task&lt;T&gt; generic type symbol.
        /// </summary>
        public ValidSymbol<INamedTypeSymbol> TaskOfT => ValidSymbol<INamedTypeSymbol>.From(
            _taskOfT ??= compilation.GetTypeByMetadataName("System.Threading.Tasks.Task`1")
                         ?? throw new InvalidOperationException("System.Threading.Tasks.Task`1 not found in compilation"));

        private INamedTypeSymbol? _valueTask;

        /// <summary>
        /// Gets the System.Threading.Tasks.ValueTask type symbol.
        /// </summary>
        public ValidSymbol<INamedTypeSymbol> ValueTask => ValidSymbol<INamedTypeSymbol>.From(
            _valueTask ??= compilation.GetTypeByMetadataName("System.Threading.Tasks.ValueTask")
                           ?? throw new InvalidOperationException("System.Threading.Tasks.ValueTask not found in compilation"));

        private INamedTypeSymbol? _valueTaskOfT;

        /// <summary>
        /// Gets the System.Threading.Tasks.ValueTask&lt;T&gt; generic type symbol.
        /// </summary>
        public ValidSymbol<INamedTypeSymbol> ValueTaskOfT => ValidSymbol<INamedTypeSymbol>.From(
            _valueTaskOfT ??= compilation.GetTypeByMetadataName("System.Threading.Tasks.ValueTask`1")
                              ?? throw new InvalidOperationException("System.Threading.Tasks.ValueTask`1 not found in compilation"));

        private INamedTypeSymbol? _iEnumerable;

        /// <summary>
        /// Gets the System.Collections.Generic.IEnumerable&lt;T&gt; generic type symbol.
        /// </summary>
        public ValidSymbol<INamedTypeSymbol> IEnumerable => ValidSymbol<INamedTypeSymbol>.From(
            _iEnumerable ??= compilation.GetTypeByMetadataName("System.Collections.Generic.IEnumerable`1")
                             ?? throw new InvalidOperationException("System.Collections.Generic.IEnumerable`1 not found in compilation"));

        private INamedTypeSymbol? _list;

        /// <summary>
        /// Gets the System.Collections.Generic.List&lt;T&gt; generic type symbol.
        /// </summary>
        public ValidSymbol<INamedTypeSymbol> List => ValidSymbol<INamedTypeSymbol>.From(
            _list ??= compilation.GetTypeByMetadataName("System.Collections.Generic.List`1")
                      ?? throw new InvalidOperationException("System.Collections.Generic.List`1 not found in compilation"));

        private INamedTypeSymbol? _dictionary;

        /// <summary>
        /// Gets the System.Collections.Generic.Dictionary&lt;TKey, TValue&gt; generic type symbol.
        /// </summary>
        public ValidSymbol<INamedTypeSymbol> Dictionary => ValidSymbol<INamedTypeSymbol>.From(
            _dictionary ??= compilation.GetTypeByMetadataName("System.Collections.Generic.Dictionary`2")
                            ?? throw new InvalidOperationException("System.Collections.Generic.Dictionary`2 not found in compilation"));

        private INamedTypeSymbol? _hashSet;

        /// <summary>
        /// Gets the System.Collections.Generic.HashSet&lt;T&gt; generic type symbol.
        /// </summary>
        public ValidSymbol<INamedTypeSymbol> HashSet => ValidSymbol<INamedTypeSymbol>.From(
            _hashSet ??= compilation.GetTypeByMetadataName("System.Collections.Generic.HashSet`1")
                         ?? throw new InvalidOperationException("System.Collections.Generic.HashSet`1 not found in compilation"));

        private INamedTypeSymbol? _iList;

        /// <summary>
        /// Gets the System.Collections.Generic.IList&lt;T&gt; generic type symbol.
        /// </summary>
        public ValidSymbol<INamedTypeSymbol> IList => ValidSymbol<INamedTypeSymbol>.From(
            _iList ??= compilation.GetTypeByMetadataName("System.Collections.Generic.IList`1")
                       ?? throw new InvalidOperationException("System.Collections.Generic.IList`1 not found in compilation"));

        private INamedTypeSymbol? _iCollection;

        /// <summary>
        /// Gets the System.Collections.Generic.ICollection&lt;T&gt; generic type symbol.
        /// </summary>
        public ValidSymbol<INamedTypeSymbol> ICollection => ValidSymbol<INamedTypeSymbol>.From(
            _iCollection ??= compilation.GetTypeByMetadataName("System.Collections.Generic.ICollection`1")
                             ?? throw new InvalidOperationException("System.Collections.Generic.ICollection`1 not found in compilation"));

        private INamedTypeSymbol? _iReadOnlyList;

        /// <summary>
        /// Gets the System.Collections.Generic.IReadOnlyList&lt;T&gt; generic type symbol.
        /// </summary>
        public ValidSymbol<INamedTypeSymbol> IReadOnlyList => ValidSymbol<INamedTypeSymbol>.From(
            _iReadOnlyList ??= compilation.GetTypeByMetadataName("System.Collections.Generic.IReadOnlyList`1")
                               ?? throw new InvalidOperationException("System.Collections.Generic.IReadOnlyList`1 not found in compilation"));

        private INamedTypeSymbol? _iReadOnlyCollection;

        /// <summary>
        /// Gets the System.Collections.Generic.IReadOnlyCollection&lt;T&gt; generic type symbol.
        /// </summary>
        public ValidSymbol<INamedTypeSymbol> IReadOnlyCollection => ValidSymbol<INamedTypeSymbol>.From(
            _iReadOnlyCollection ??= compilation.GetTypeByMetadataName("System.Collections.Generic.IReadOnlyCollection`1")
                                     ?? throw new InvalidOperationException("System.Collections.Generic.IReadOnlyCollection`1 not found in compilation"));

        private INamedTypeSymbol? _iAsyncEnumerable;

        /// <summary>
        /// Gets the System.Collections.Generic.IAsyncEnumerable&lt;T&gt; generic type symbol.
        /// </summary>
        public ValidSymbol<INamedTypeSymbol> IAsyncEnumerable => ValidSymbol<INamedTypeSymbol>.From(
            _iAsyncEnumerable ??= compilation.GetTypeByMetadataName("System.Collections.Generic.IAsyncEnumerable`1")
                                  ?? throw new InvalidOperationException("System.Collections.Generic.IAsyncEnumerable`1 not found in compilation"));

        private INamedTypeSymbol? _nullableOfT;

        /// <summary>
        /// Gets the System.Nullable&lt;T&gt; generic type symbol.
        /// </summary>
        public ValidSymbol<INamedTypeSymbol> NullableOfT => ValidSymbol<INamedTypeSymbol>.From(
            _nullableOfT ??= compilation.GetTypeByMetadataName("System.Nullable`1")
                             ?? throw new InvalidOperationException("System.Nullable`1 not found in compilation"));

        private INamedTypeSymbol? _type;

        /// <summary>
        /// Gets the System.Type type symbol.
        /// </summary>
        public ValidSymbol<INamedTypeSymbol> Type => ValidSymbol<INamedTypeSymbol>.From(
            _type ??= compilation.GetTypeByMetadataName("System.Type")
                      ?? throw new InvalidOperationException("System.Type not found in compilation"));

        private INamedTypeSymbol? _attribute;

        /// <summary>
        /// Gets the System.Attribute type symbol.
        /// </summary>
        public ValidSymbol<INamedTypeSymbol> Attribute => ValidSymbol<INamedTypeSymbol>.From(
            _attribute ??= compilation.GetTypeByMetadataName("System.Attribute")
                           ?? throw new InvalidOperationException("System.Attribute not found in compilation"));

        private INamedTypeSymbol? _exception;

        /// <summary>
        /// Gets the System.Exception type symbol.
        /// </summary>
        public ValidSymbol<INamedTypeSymbol> Exception => ValidSymbol<INamedTypeSymbol>.From(
            _exception ??= compilation.GetTypeByMetadataName("System.Exception")
                           ?? throw new InvalidOperationException("System.Exception not found in compilation"));

        private INamedTypeSymbol? _argumentException;

        /// <summary>
        /// Gets the System.ArgumentException type symbol.
        /// </summary>
        public ValidSymbol<INamedTypeSymbol> ArgumentException => ValidSymbol<INamedTypeSymbol>.From(
            _argumentException ??= compilation.GetTypeByMetadataName("System.ArgumentException")
                                   ?? throw new InvalidOperationException("System.ArgumentException not found in compilation"));

        private INamedTypeSymbol? _argumentNullException;

        /// <summary>
        /// Gets the System.ArgumentNullException type symbol.
        /// </summary>
        public ValidSymbol<INamedTypeSymbol> ArgumentNullException => ValidSymbol<INamedTypeSymbol>.From(
            _argumentNullException ??= compilation.GetTypeByMetadataName("System.ArgumentNullException")
                                       ?? throw new InvalidOperationException("System.ArgumentNullException not found in compilation"));

        private INamedTypeSymbol? _iDisposable;

        /// <summary>
        /// Gets the System.IDisposable type symbol.
        /// </summary>
        public ValidSymbol<INamedTypeSymbol> IDisposable => ValidSymbol<INamedTypeSymbol>.From(
            _iDisposable ??= compilation.GetTypeByMetadataName("System.IDisposable")
                             ?? throw new InvalidOperationException("System.IDisposable not found in compilation"));

        private INamedTypeSymbol? _iAsyncDisposable;

        /// <summary>
        /// Gets the System.IAsyncDisposable type symbol.
        /// </summary>
        public ValidSymbol<INamedTypeSymbol> IAsyncDisposable => ValidSymbol<INamedTypeSymbol>.From(
            _iAsyncDisposable ??= compilation.GetTypeByMetadataName("System.IAsyncDisposable")
                                  ?? throw new InvalidOperationException("System.IAsyncDisposable not found in compilation"));

        private INamedTypeSymbol? _obsoleteAttribute;

        /// <summary>
        /// Gets the System.ObsoleteAttribute type symbol.
        /// </summary>
        public ValidSymbol<INamedTypeSymbol> ObsoleteAttribute => ValidSymbol<INamedTypeSymbol>.From(
            _obsoleteAttribute ??= compilation.GetTypeByMetadataName("System.ObsoleteAttribute")
                                   ?? throw new InvalidOperationException("System.ObsoleteAttribute not found in compilation"));

        private INamedTypeSymbol? _entityFrameworkDbContext;
        private bool _entityFrameworkDbContextChecked;

        /// <summary>
        /// Gets the Microsoft.EntityFrameworkCore.DbContext symbol if EF Core is referenced.
        /// Returns null if EF Core is not available in the compilation.
        /// </summary>
        public ValidSymbol<INamedTypeSymbol>? EntityFrameworkDbContext
        {
            get
            {
                if (!_entityFrameworkDbContextChecked)
                {
                    _entityFrameworkDbContext =
                        compilation.GetTypeByMetadataName("Microsoft.EntityFrameworkCore.DbContext");
                    _entityFrameworkDbContextChecked = true;
                }

                return _entityFrameworkDbContext is not null
                    ? ValidSymbol<INamedTypeSymbol>.From(_entityFrameworkDbContext)
                    : null;
            }
        }
    }
}