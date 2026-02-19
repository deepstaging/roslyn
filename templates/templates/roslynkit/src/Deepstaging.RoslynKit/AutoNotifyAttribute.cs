// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.RoslynKit;

/// <summary>Marks a class for INotifyPropertyChanged generation. Private fields become public properties.</summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class AutoNotifyAttribute : Attribute;

/// <summary>Specifies additional properties to notify when this field's property changes.</summary>
[AttributeUsage(AttributeTargets.Field)]
public sealed class AlsoNotifyAttribute : Attribute
{
    /// <summary>The property names to also raise PropertyChanged for.</summary>
    public string[] PropertyNames { get; }

    /// <summary>Initializes a new instance with the specified property names.</summary>
    public AlsoNotifyAttribute(params string[] propertyNames)
    {
        PropertyNames = propertyNames;
    }
}