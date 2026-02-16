// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn;

/// <summary>
/// Extension methods for creating <see cref="DocumentationSnapshot"/> from <see cref="XmlDocumentation"/>.
/// </summary>
public static class XmlDocumentationSnapshotExtensions
{
    extension(XmlDocumentation doc)
    {
        /// <summary>
        /// Creates a pipeline-safe <see cref="DocumentationSnapshot"/> from this documentation.
        /// </summary>
        public DocumentationSnapshot ToSnapshot()
        {
            if (!doc.HasValue)
                return DocumentationSnapshot.Empty;

            return new DocumentationSnapshot
            {
                Summary = doc.Summary,
                Remarks = doc.Remarks,
                Returns = doc.Returns,
                Value = doc.Value,
                Example = doc.Example,
                Params = doc.Params
                    .Select(p => new ParamDocumentation(p.Name, p.Description))
                    .ToEquatableArray(),
                TypeParams = doc.TypeParams
                    .Select(p => new ParamDocumentation(p.Name, p.Description))
                    .ToEquatableArray(),
                Exceptions = doc.Exceptions
                    .Select(e => new ExceptionDocumentation(e.Type, e.Description))
                    .ToEquatableArray(),
                SeeAlso = doc.SeeAlso.ToEquatableArray()
            };
        }
    }
}