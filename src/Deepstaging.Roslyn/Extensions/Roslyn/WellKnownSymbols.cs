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

    /// <summary>
    /// Lazily-loaded well-known symbols from a compilation.
    /// Provides cached access to commonly used framework types.
    /// </summary>
    public sealed class WellKnownSymbols(Compilation compilation)
    {
        private INamedTypeSymbol? _string;

        /// <summary>
        /// Gets the System.String type symbol.
        /// </summary>
        public INamedTypeSymbol String => _string ??=
            compilation.GetTypeByMetadataName("System.String")
            ?? throw new InvalidOperationException("System.String not found in compilation");

        private INamedTypeSymbol? _int32;

        /// <summary>
        /// Gets the System.Int32 type symbol.
        /// </summary>
        public INamedTypeSymbol Int32 => _int32 ??=
            compilation.GetTypeByMetadataName("System.Int32")
            ?? throw new InvalidOperationException("System.Int32 not found in compilation");

        private INamedTypeSymbol? _boolean;

        /// <summary>
        /// Gets the System.Boolean type symbol.
        /// </summary>
        public INamedTypeSymbol Boolean => _boolean ??=
            compilation.GetTypeByMetadataName("System.Boolean")
            ?? throw new InvalidOperationException("System.Boolean not found in compilation");

        private INamedTypeSymbol? _object;

        /// <summary>
        /// Gets the System.Object type symbol.
        /// </summary>
        public INamedTypeSymbol Object => _object ??=
            compilation.GetTypeByMetadataName("System.Object")
            ?? throw new InvalidOperationException("System.Object not found in compilation");

        private INamedTypeSymbol? _void;

        /// <summary>
        /// Gets the System.Void type symbol.
        /// </summary>
        public INamedTypeSymbol Void => _void ??=
            compilation.GetTypeByMetadataName("System.Void")
            ?? throw new InvalidOperationException("System.Void not found in compilation");

        private INamedTypeSymbol? _dateTime;

        /// <summary>
        /// Gets the System.DateTime type symbol.
        /// </summary>
        public INamedTypeSymbol DateTime => _dateTime ??=
            compilation.GetTypeByMetadataName("System.DateTime")
            ?? throw new InvalidOperationException("System.DateTime not found in compilation");

        private INamedTypeSymbol? _cancellationToken;

        /// <summary>
        /// Gets the System.Threading.CancellationToken type symbol.
        /// </summary>
        public INamedTypeSymbol CancellationToken => _cancellationToken ??=
            compilation.GetTypeByMetadataName("System.Threading.CancellationToken")
            ?? throw new InvalidOperationException("System.Threading.CancellationToken not found in compilation");

        private INamedTypeSymbol? _task;

        /// <summary>
        /// Gets the System.Threading.Tasks.Task type symbol.
        /// </summary>
        public INamedTypeSymbol Task => _task ??=
            compilation.GetTypeByMetadataName("System.Threading.Tasks.Task")
            ?? throw new InvalidOperationException("System.Threading.Tasks.Task not found in compilation");

        private INamedTypeSymbol? _taskOfT;

        /// <summary>
        /// Gets the System.Threading.Tasks.Task&lt;T&gt; generic type symbol.
        /// </summary>
        public INamedTypeSymbol TaskOfT => _taskOfT ??=
            compilation.GetTypeByMetadataName("System.Threading.Tasks.Task`1")
            ?? throw new InvalidOperationException("System.Threading.Tasks.Task`1 not found in compilation");

        private INamedTypeSymbol? _valueTask;

        /// <summary>
        /// Gets the System.Threading.Tasks.ValueTask type symbol.
        /// </summary>
        public INamedTypeSymbol ValueTask => _valueTask ??=
            compilation.GetTypeByMetadataName("System.Threading.Tasks.ValueTask")
            ?? throw new InvalidOperationException("System.Threading.Tasks.ValueTask not found in compilation");

        private INamedTypeSymbol? _valueTaskOfT;

        /// <summary>
        /// Gets the System.Threading.Tasks.ValueTask&lt;T&gt; generic type symbol.
        /// </summary>
        public INamedTypeSymbol ValueTaskOfT => _valueTaskOfT ??=
            compilation.GetTypeByMetadataName("System.Threading.Tasks.ValueTask`1")
            ?? throw new InvalidOperationException("System.Threading.Tasks.ValueTask`1 not found in compilation");

        private INamedTypeSymbol? _iEnumerable;

        /// <summary>
        /// Gets the System.Collections.Generic.IEnumerable&lt;T&gt; generic type symbol.
        /// </summary>
        public INamedTypeSymbol IEnumerable => _iEnumerable ??=
            compilation.GetTypeByMetadataName("System.Collections.Generic.IEnumerable`1")
            ?? throw new InvalidOperationException("System.Collections.Generic.IEnumerable`1 not found in compilation");

        private INamedTypeSymbol? _list;

        /// <summary>
        /// Gets the System.Collections.Generic.List&lt;T&gt; generic type symbol.
        /// </summary>
        public INamedTypeSymbol List => _list ??=
            compilation.GetTypeByMetadataName("System.Collections.Generic.List`1")
            ?? throw new InvalidOperationException("System.Collections.Generic.List`1 not found in compilation");

        private INamedTypeSymbol? _dictionary;

        /// <summary>
        /// Gets the System.Collections.Generic.Dictionary&lt;TKey, TValue&gt; generic type symbol.
        /// </summary>
        public INamedTypeSymbol Dictionary => _dictionary ??=
            compilation.GetTypeByMetadataName("System.Collections.Generic.Dictionary`2")
            ?? throw new InvalidOperationException("System.Collections.Generic.Dictionary`2 not found in compilation");

        private INamedTypeSymbol? _obsoleteAttribute;

        /// <summary>
        /// Gets the System.ObsoleteAttribute type symbol.
        /// </summary>
        public INamedTypeSymbol ObsoleteAttribute => _obsoleteAttribute ??=
            compilation.GetTypeByMetadataName("System.ObsoleteAttribute")
            ?? throw new InvalidOperationException("System.ObsoleteAttribute not found in compilation");

        private INamedTypeSymbol? _entityFrameworkDbContext;
        private bool _entityFrameworkDbContextChecked;

        /// <summary>
        /// Gets the Microsoft.EntityFrameworkCore.DbContext symbol if EF Core is referenced.
        /// Returns null if EF Core is not available in the compilation.
        /// </summary>
        public INamedTypeSymbol? EntityFrameworkDbContext
        {
            get
            {
                if (!_entityFrameworkDbContextChecked)
                {
                    _entityFrameworkDbContext =
                        compilation.GetTypeByMetadataName("Microsoft.EntityFrameworkCore.DbContext");
                    _entityFrameworkDbContextChecked = true;
                }

                return _entityFrameworkDbContext;
            }
        }
    }
}