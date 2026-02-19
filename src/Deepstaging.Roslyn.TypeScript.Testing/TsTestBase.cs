// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Deepstaging.Roslyn.TypeScript.Emit;
using Verify = VerifyTUnit.Verifier;

namespace Deepstaging.Roslyn.TypeScript.Testing;

/// <summary>
/// Base class for TypeScript emit tests. Provides auto-discovered <c>tsc</c> path,
/// pre-configured <see cref="TsEmitOptions"/> presets, and a fluent emit entry point.
/// </summary>
/// <remarks>
/// <para>
/// Mirrors <c>RoslynTestBase</c> from <c>Deepstaging.Roslyn.Testing</c>.
/// Inherit from this class and use the protected members to test TypeScript code generation.
/// </para>
/// <para>
/// The <c>tsc</c> path is resolved by walking up from <see cref="AppContext.BaseDirectory"/>
/// looking for <c>node_modules/.bin/tsc</c>. If not found, validation-enabled options will
/// throw at emit time. Install TypeScript locally in your test project:
/// <c>npm init -y &amp;&amp; npm install typescript --save-dev</c>.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// public class MyGeneratorTests : TsTestBase
/// {
///     [Test]
///     public async Task Emits_Valid_Interface()
///     {
///         var result = TsTypeBuilder.Interface("User")
///             .Exported()
///             .AddProperty("id", "number", p => p.AsReadonly())
///             .Emit(ValidatedOptions);
///
///         await Assert.That(result.Success).IsTrue();
///     }
/// }
/// </code>
/// </example>
public abstract class TsTestBase
{
    private static readonly Lazy<string?> LazyTscPath = new(DiscoverTscPath);

    /// <summary>
    /// Gets the auto-discovered path to the <c>tsc</c> binary, or <c>null</c> if not found.
    /// </summary>
    /// <remarks>
    /// Walks up from <see cref="AppContext.BaseDirectory"/> (typically <c>bin/Release/net10.0/</c>)
    /// looking for <c>node_modules/.bin/tsc</c> in each ancestor directory.
    /// </remarks>
    protected static string? TscPath => LazyTscPath.Value;

    /// <summary>
    /// Gets emit options with no validation or formatting. The fastest option for unit tests
    /// that only need to verify the emitted string content.
    /// </summary>
    protected static TsEmitOptions DefaultOptions => new();

    /// <summary>
    /// Gets emit options with <c>tsc</c> syntax validation enabled.
    /// Requires TypeScript to be installed in the test project's <c>node_modules</c>.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown at emit time if <c>tsc</c> was not found during discovery.
    /// </exception>
    protected static TsEmitOptions ValidatedOptions => new()
    {
        ValidationLevel = TsValidationLevel.Syntax,
        TscPath = TscPath ?? throw new InvalidOperationException(
            "tsc not found. Install TypeScript in your test project: npm init -y && npm install typescript --save-dev"),
    };

    /// <summary>
    /// Gets emit options with output formatting enabled (via dprint or prettier).
    /// </summary>
    protected static TsEmitOptions FormattedOptions => new()
    {
        FormatOutput = true,
    };

    /// <summary>
    /// Gets emit options with both <c>tsc</c> validation and output formatting enabled.
    /// </summary>
    protected static TsEmitOptions FormattedAndValidatedOptions => new()
    {
        ValidationLevel = TsValidationLevel.Syntax,
        TscPath = TscPath ?? throw new InvalidOperationException(
            "tsc not found. Install TypeScript in your test project: npm init -y && npm install typescript --save-dev"),
        FormatOutput = true,
    };

    /// <summary>
    /// Snapshot-verifies the emitted TypeScript code using Verify.
    /// Asserts the emit succeeded, then compares the code against the <c>.verified.txt</c> file
    /// stored next to the calling test file.
    /// </summary>
    /// <param name="result">The emit result to verify.</param>
    /// <param name="sourceFile">Auto-populated by the compiler; do not pass explicitly.</param>
    /// <returns>A task that completes when the snapshot comparison is done.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the emit result indicates failure.</exception>
    protected static Task VerifyEmit(TsOptionalEmit result, [CallerFilePath] string sourceFile = "")
    {
        if (!result.TryValidate(out var valid))
            throw new InvalidOperationException(
                $"Emit failed with diagnostics:\n{string.Join("\n", result.Diagnostics)}");

        return Verify.Verify(valid.Code, sourceFile: sourceFile);
    }

    private static string? DiscoverTscPath()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory != null)
        {
            var candidate = Path.Combine(directory.FullName, "node_modules", ".bin", "tsc");
            if (File.Exists(candidate))
                return candidate;

            directory = directory.Parent;
        }

        return null;
    }
}
