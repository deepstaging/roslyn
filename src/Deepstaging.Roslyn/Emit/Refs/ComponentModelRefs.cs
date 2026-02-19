// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Refs;

/// <summary>
/// Factory members for common <c>System.ComponentModel</c> types.
/// </summary>
public static class ComponentModelRefs
{
    /// <summary>Gets the <c>System.ComponentModel</c> namespace.</summary>
    public static NamespaceRef Namespace => NamespaceRef.From("System.ComponentModel");

    // ── Change Notification ─────────────────────────────────────────────

    /// <summary>Gets an <c>INotifyPropertyChanged</c> type reference.</summary>
    public static TypeRef INotifyPropertyChanged => Namespace.GlobalType("INotifyPropertyChanged");

    /// <summary>Gets an <c>INotifyPropertyChanging</c> type reference.</summary>
    public static TypeRef INotifyPropertyChanging => Namespace.GlobalType("INotifyPropertyChanging");

    /// <summary>Gets a <c>PropertyChangedEventHandler</c> type reference.</summary>
    public static TypeRef PropertyChangedEventHandler => Namespace.GlobalType("PropertyChangedEventHandler");

    /// <summary>Gets a <c>PropertyChangingEventHandler</c> type reference.</summary>
    public static TypeRef PropertyChangingEventHandler => Namespace.GlobalType("PropertyChangingEventHandler");

    /// <summary>Gets a <c>PropertyChangedEventArgs</c> type reference.</summary>
    public static TypeRef PropertyChangedEventArgs => Namespace.GlobalType("PropertyChangedEventArgs");

    /// <summary>Gets a <c>PropertyChangingEventArgs</c> type reference.</summary>
    public static TypeRef PropertyChangingEventArgs => Namespace.GlobalType("PropertyChangingEventArgs");

    /// <summary>Gets an <c>INotifyDataErrorInfo</c> type reference.</summary>
    public static TypeRef INotifyDataErrorInfo => Namespace.GlobalType("INotifyDataErrorInfo");

    // ── Data Annotations ────────────────────────────────────────────────

    /// <summary>Gets a <c>IEditableObject</c> type reference.</summary>
    public static TypeRef IEditableObject => Namespace.GlobalType("IEditableObject");

    /// <summary>Gets a <c>IRevertibleChangeTracking</c> type reference.</summary>
    public static TypeRef IRevertibleChangeTracking => Namespace.GlobalType("IRevertibleChangeTracking");

    /// <summary>Gets a <c>IChangeTracking</c> type reference.</summary>
    public static TypeRef IChangeTracking => Namespace.GlobalType("IChangeTracking");

    // ── Type Descriptors ────────────────────────────────────────────────

    /// <summary>Gets a <c>TypeDescriptor</c> type reference.</summary>
    public static TypeRef TypeDescriptor => Namespace.GlobalType("TypeDescriptor");

    /// <summary>Gets a <c>TypeConverter</c> type reference.</summary>
    public static TypeRef TypeConverter => Namespace.GlobalType("TypeConverter");

    /// <summary>Gets a <c>BindableAttribute</c> type reference.</summary>
    public static TypeRef BindableAttribute => Namespace.GlobalType("BindableAttribute");

    /// <summary>Gets a <c>BrowsableAttribute</c> type reference.</summary>
    public static TypeRef BrowsableAttribute => Namespace.GlobalType("BrowsableAttribute");

    /// <summary>Gets an <c>EditorBrowsableAttribute</c> type reference.</summary>
    public static TypeRef EditorBrowsableAttribute => Namespace.GlobalType("EditorBrowsableAttribute");

    /// <summary>Gets an <c>EditorBrowsableState</c> type reference.</summary>
    public static TypeRef EditorBrowsableState => Namespace.GlobalType("EditorBrowsableState");
}
