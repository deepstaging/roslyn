// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.RoslynKit.Projection;

using Attributes;
using Deepstaging.Roslyn;
using Models;

/// <summary>Query extensions for extracting AutoNotify models from symbols.</summary>
public static class AutoNotifyProjection
{
    extension(ValidSymbol<INamedTypeSymbol> symbol)
    {
        /// <summary>Extracts an <see cref="AutoNotifyModel"/> from a type symbol.</summary>
        public AutoNotifyModel QueryAutoNotify() => new ()
        {
            TypeName = symbol.Name,
            Namespace = symbol.Namespace ?? "Global",
            Accessibility = symbol.AccessibilityString,
            Fields =
            [
                ..symbol.QueryFields()
                    .ThatArePrivate()
                    .ThatAreInstance()
                    .Where(f => f.Name.StartsWith("_"))
                    .Select(AutoNotifyFieldModel)
            ]
        };
    }

    private static AutoNotifyFieldModel AutoNotifyFieldModel(this ValidSymbol<IFieldSymbol> field) => new()
    {
        FieldName = field.Name,
        PropertyName = field.PropertyName,
        TypeName = field.Type.GloballyQualifiedName,
        AlsoNotify =
        [
            ..field.GetAttributes<AlsoNotifyAttribute>()
                .Select(attr => attr.AsQuery<AlsoNotifyAttributeQuery>())
                .SelectMany(q => q.AlsoNotify)
        ]
    };
}