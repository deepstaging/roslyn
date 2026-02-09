// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit;
using Deepstaging.RoslynKit.Projection.Models;

namespace Deepstaging.RoslynKit.Generators.Writers;

/// <summary>
/// Provides extension methods for generating INotifyPropertyChanged source code from an <see cref="AutoNotifyModel"/>.
/// </summary>
public static class AutoNotifyWriter
{
    extension(AutoNotifyModel model)
    {
        /// <summary>
        /// Generates the partial class implementing INotifyPropertyChanged.
        /// </summary>
        public OptionalEmit WriteAutoNotifyClass()
        {
            return TypeBuilder
                .Class(model.TypeName)
                .AsPartial()
                .InNamespace(model.Namespace)
                .WithAccessibility(model.Accessibility)
                .AddUsing("System.ComponentModel")
                .AddUsing("System.Runtime.CompilerServices")
                .Implements("INotifyPropertyChanged")
                .GenerateBaseImplementation(model)
                .WithEach(model.Properties, AddProperty).Emit();
        }
    }

    private static TypeBuilder AddProperty(TypeBuilder type, NotifyPropertyModel property)
    {
        return type.AddProperty(
            PropertyBuilder.Parse($"public {property.TypeName} {property.PropertyName} {{ get; set; }}")
                .WithXmlDoc($"Gets or sets the {property.PropertyName} value.")
                .WithGetter(b => b.AddStatement($"return {property.FieldName}"))
                .WithSetter(b => b.AddIf($"SetField(ref {property.FieldName}, value)",
                    ifBody => ifBody.WithEach(property.AlsoNotify, (body, notify) =>
                        body.AddStatement($"OnPropertyChanged(nameof({notify})))")))));
    }

    private static TypeBuilder GenerateBaseImplementation(this TypeBuilder builder, AutoNotifyModel model)
    {
        return builder.If(model.GenerateBaseImplementation, type => type
            .AddEvent("PropertyChanged", "PropertyChangedEventHandler?",
                e => e.WithXmlDoc("Occurs when a property value changes."))
            .AddMethod(OnPropertyChangedMethod)
            .AddMethod(SetFieldMethod));
    }

    private static MethodBuilder SetFieldMethod =>
        MethodBuilder.Parse("protected bool SetField<T>(ref T field, T value, string? propertyName = null)")
            .AddParameter("propertyName", "string?", p => p.WithAttribute("CallerMemberName").WithDefaultValue("null"))
            .WithXmlDoc("Sets a field value and raises PropertyChanged if the value changed.")
            .WithBody(b => b.AddStatements(
                """
                if (EqualityComparer<T>.Default.Equals(field, value)) return false;
                field = value;
                OnPropertyChanged(propertyName);
                return true;
                """));

    private static MethodBuilder OnPropertyChangedMethod =>
        MethodBuilder.Parse("protected virtual void OnPropertyChanged(string? propertyName = null)")
            .AddParameter("propertyName", "string?", p => p.WithAttribute("CallerMemberName").WithDefaultValue("null"))
            .WithXmlDoc("Raises the PropertyChanged event.")
            .WithBody(b => b.AddStatement("PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName))"));
}