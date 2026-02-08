// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Interfaces.Disposal;

/// <summary>
/// TypeBuilder extensions for implementing IDisposable with the full dispose pattern.
/// </summary>
public static class TypeBuilderDisposableExtensions
{
    /// <summary>
    /// Implements IDisposable with the standard dispose pattern.
    /// Generates: Dispose(), Dispose(bool), disposed field.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="disposeStatements">Statements to execute when disposing managed resources.</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder ImplementsIDisposable(
        this TypeBuilder builder,
        params string[] disposeStatements)
    {
        return builder
            .Implements("global::System.IDisposable")
            .AddField(FieldBuilder.Parse("private bool _disposed"))
            .AddMethod(BuildDisposeMethod())
            .AddMethod(BuildDisposePatternMethod(disposeStatements));
    }

    /// <summary>
    /// Implements IDisposable by disposing a single field.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="fieldName">The disposable field to dispose.</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder ImplementsIDisposableForField(
        this TypeBuilder builder,
        string fieldName) =>
        builder.ImplementsIDisposable($"{fieldName}?.Dispose();");

    /// <summary>
    /// Implements IDisposable by disposing multiple fields.
    /// </summary>
    /// <param name="builder">The type builder.</param>
    /// <param name="fieldNames">The disposable fields to dispose.</param>
    /// <returns>The modified type builder.</returns>
    public static TypeBuilder ImplementsIDisposableForFields(
        this TypeBuilder builder,
        params string[] fieldNames) =>
        builder.ImplementsIDisposable(fieldNames.Select(f => $"{f}?.Dispose();").ToArray());

    private static MethodBuilder BuildDisposeMethod() =>
        MethodBuilder
            .Parse("public void Dispose()")
            .WithBody(b => b
                .AddStatement("Dispose(true);")
                .AddStatement("global::System.GC.SuppressFinalize(this);"));

    private static MethodBuilder BuildDisposePatternMethod(string[] userStatements)
    {
        return MethodBuilder
            .Parse("protected virtual void Dispose(bool disposing)")
            .WithBody(b =>
            {
                b = b.AddStatement("if (_disposed) return;");
                b = b.AddStatement("if (disposing)");
                b = b.AddStatement("{");
                foreach (var statement in userStatements)
                {
                    b = b.AddStatement($"    {statement}");
                }
                b = b.AddStatement("}");
                b = b.AddStatement("");
                b = b.AddStatement("_disposed = true;");
                return b;
            });
    }
}
