// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Interfaces.Observable;

/// <summary>
/// TypeBuilder extensions for implementing INotifyPropertyChanged.
/// Adds the PropertyChanged event and OnPropertyChanged helper method.
/// </summary>
public static class TypeBuilderNotifyPropertyChangedExtensions
{
    /// <summary>
    /// Implements INotifyPropertyChanged with a protected OnPropertyChanged helper method.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <returns>The modified type builder with INotifyPropertyChanged implementation.</returns>
    public static TypeBuilder ImplementsINotifyPropertyChanged(this TypeBuilder builder) =>
        builder
            .Implements("global::System.ComponentModel.INotifyPropertyChanged")
            .AddEvent(EventBuilder
                .For("PropertyChanged", "global::System.ComponentModel.PropertyChangedEventHandler?"))
            .AddMethod("OnPropertyChanged", m => m
                .WithAccessibility(Accessibility.Protected)
                .AddParameter("propertyName", "string?", p => p
                    .WithAttribute("global::System.Runtime.CompilerServices.CallerMemberName")
                    .WithDefaultValue("null"))
                .WithBody(b => b
                    .AddStatement(
                        "PropertyChanged?.Invoke(this, new global::System.ComponentModel.PropertyChangedEventArgs(propertyName));")));

    /// <summary>
    /// Implements INotifyPropertyChanged with a custom OnPropertyChanged method name.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="onPropertyChangedMethodName">The name of the OnPropertyChanged helper method.</param>
    /// <returns>The modified type builder with INotifyPropertyChanged implementation.</returns>
    public static TypeBuilder ImplementsINotifyPropertyChanged(
        this TypeBuilder builder,
        string onPropertyChangedMethodName) =>
        builder
            .Implements("global::System.ComponentModel.INotifyPropertyChanged")
            .AddEvent(EventBuilder
                .For("PropertyChanged", "global::System.ComponentModel.PropertyChangedEventHandler?"))
            .AddMethod(onPropertyChangedMethodName, m => m
                .WithAccessibility(Accessibility.Protected)
                .AddParameter("propertyName", "string?", p => p
                    .WithAttribute("global::System.Runtime.CompilerServices.CallerMemberName")
                    .WithDefaultValue("null"))
                .WithBody(b => b
                    .AddStatement(
                        "PropertyChanged?.Invoke(this, new global::System.ComponentModel.PropertyChangedEventArgs(propertyName));")));
}