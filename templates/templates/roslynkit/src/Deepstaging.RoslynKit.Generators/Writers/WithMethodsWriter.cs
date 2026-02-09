// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Emit;
using Deepstaging.RoslynKit.Projection.Models;

namespace Deepstaging.RoslynKit.Generators.Writers;

/// <summary>
/// Provides extension methods for generating With*() methods from a <see cref="WithMethodsModel"/>.
/// </summary>
public static class WithMethodsWriter
{
    extension(WithMethodsModel model)
    {
        /// <summary>
        /// Generates the partial type with immutable With*() methods.
        /// </summary>
        public OptionalEmit WriteWithMethods()
        {
            var builder = model.IsStruct
                ? TypeBuilder.Struct(model.TypeName)
                : TypeBuilder.Class(model.TypeName);

            return builder
                .AsPartial()
                .InNamespace(model.Namespace)
                .WithEach(model.Properties, (type, property) => type.AddWithMethod(model, property))
                .Emit();
        }
    }

    private static TypeBuilder AddWithMethod(this TypeBuilder type, WithMethodsModel model, WithPropertyModel property)
    {
        var initializers =
            model.Properties.Select(p => $"{p.Name} = {(p.Name == property.Name ? "value" : $"this.{p.Name}")},");

        return type.AddMethod(
            MethodBuilder.Parse($"public {model.TypeName} With{property.Name}({property.TypeName} value)")
                .WithXmlDoc($"Creates a new instance with <see cref=\"{property.Name}\"/> set to the specified value.")
                .WithBody(body => body.AddStatements(
                    $$"""
                      return new {{model.TypeName}}
                      {
                      {{string.Join("\n", initializers)}}
                      };
                      """)));
    }
}