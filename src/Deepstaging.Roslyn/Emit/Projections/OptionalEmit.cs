// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Emit;

/// <summary>
/// An optional emit result that may contain diagnostics or errors.
/// Similar to OptionalSymbol&lt;T&gt; - check before accessing.
/// Use Validate() or IsValid(out var valid) to safely access the result.
/// </summary>
public readonly struct OptionalEmit : IValidatableProjection<CompilationUnitSyntax, ValidEmit>
{
    private readonly CompilationUnitSyntax? _syntax;
    private readonly string? _code;
    private readonly ImmutableArray<Diagnostic> _diagnostics;

    private OptionalEmit(
        CompilationUnitSyntax? syntax,
        string? code,
        ImmutableArray<Diagnostic> diagnostics)
    {
        _syntax = syntax;
        _code = code;
        _diagnostics = diagnostics.IsDefault ? ImmutableArray<Diagnostic>.Empty : diagnostics;
    }

    #region Factory Methods

    /// <summary>
    /// Creates a successful emit result with no diagnostics.
    /// </summary>
    internal static OptionalEmit FromSuccess(CompilationUnitSyntax syntax, string code)
        => new(syntax, code, ImmutableArray<Diagnostic>.Empty);

    /// <summary>
    /// Creates an emit result with warnings or informational diagnostics.
    /// The result is considered successful if there are no errors.
    /// </summary>
    internal static OptionalEmit FromDiagnostics(
        CompilationUnitSyntax syntax,
        string code,
        ImmutableArray<Diagnostic> diagnostics)
        => new(syntax, code, diagnostics);

    /// <summary>
    /// Creates a failed emit result with error diagnostics.
    /// </summary>
    internal static OptionalEmit FromFailure(ImmutableArray<Diagnostic> diagnostics)
        => new(null, null, diagnostics);

    #endregion

    #region Result Properties

    /// <summary>
    /// Gets a value indicating whether the emit was successful.
    /// True if syntax is present and no error diagnostics exist.
    /// </summary>
    public bool Success =>
        _syntax != null && !_diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error);

    /// <summary>
    /// Gets all diagnostics produced during emit (errors, warnings, info).
    /// </summary>
    public ImmutableArray<Diagnostic> Diagnostics => _diagnostics;

    /// <summary>
    /// Gets the generated compilation unit syntax tree, or null if emit failed.
    /// </summary>
    public CompilationUnitSyntax? Syntax => _syntax;

    /// <summary>
    /// Gets the formatted C# code as a string, or null if emit failed.
    /// </summary>
    public string? Code => _code;

    /// <summary>
    /// Gets only the error diagnostics.
    /// </summary>
    public IEnumerable<Diagnostic> Errors =>
        _diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error);

    /// <summary>
    /// Gets only the warning diagnostics.
    /// </summary>
    public IEnumerable<Diagnostic> Warnings =>
        _diagnostics.Where(d => d.Severity == DiagnosticSeverity.Warning);

    #endregion

    #region IValidatableProjection Implementation

    /// <summary>
    /// Gets a value indicating whether the emit result contains a value.
    /// </summary>
    public bool HasValue => _syntax != null;

    /// <summary>
    /// Gets a value indicating whether the emit result is empty (failed).
    /// </summary>
    public bool IsEmpty => _syntax == null;

    /// <summary>
    /// Validates the optional emit result to a ValidEmit with guaranteed non-null access.
    /// Returns null if validation fails (emit was unsuccessful).
    /// </summary>
    public ValidEmit? Validate() =>
        Success ? ValidEmit.From(_syntax!, _code!) : null;

    /// <summary>
    /// Validates the optional emit result or throws if unsuccessful.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when emit failed.</exception>
    public ValidEmit ValidateOrThrow(string? message = null)
    {
        if (!Success)
        {
            var errorCount = Errors.Count();
            var errorMessages = string.Join("\n  ", Errors.Select(d => d.ToString()));
            throw new InvalidOperationException(
                message ?? $"Emit failed with {errorCount} error(s):\n  {errorMessages}");
        }
        return ValidEmit.From(_syntax!, _code!);
    }

    /// <summary>
    /// Attempts to validate the optional emit result. Returns true if successful.
    /// </summary>
    public bool TryValidate(out ValidEmit validated)
    {
        if (Success)
        {
            validated = ValidEmit.From(_syntax!, _code!);
            return true;
        }
        validated = default;
        return false;
    }

    /// <summary>
    /// Checks if the emit result is not valid (unsuccessful). Returns true if invalid.
    /// Enables fast-exit pattern: if (result.IsNotValid(out var valid)) return;
    /// </summary>
    public bool IsNotValid(out ValidEmit validated) => !TryValidate(out validated);

    /// <summary>
    /// Checks if the emit result is valid (successful). Returns true if valid.
    /// </summary>
    public bool IsValid(out ValidEmit validated) => TryValidate(out validated);

    /// <summary>
    /// Returns the syntax or throws if emit failed.
    /// </summary>
    public CompilationUnitSyntax OrThrow(string? message = null) =>
        ValidateOrThrow(message).Syntax;

    /// <summary>
    /// Returns the syntax or throws with a lazily-computed message if emit failed.
    /// </summary>
    /// <param name="messageFactory">Factory function to create the error message.</param>
    public CompilationUnitSyntax OrThrow(Func<string> messageFactory) =>
        ValidateOrThrow(messageFactory()).Syntax;

    /// <summary>
    /// Returns the syntax or null if emit failed.
    /// </summary>
    public CompilationUnitSyntax? OrNull() => _syntax;

    #endregion

    #region Combination

    /// <summary>
    /// Combines multiple optional emits into a single optional emit.
    /// If any emit failed, returns a failed result with all aggregated diagnostics.
    /// If all succeed, combines using ValidEmit.Combine and returns a successful result.
    /// </summary>
    /// <param name="emits">The optional emits to combine.</param>
    /// <param name="options">Optional emit options for formatting. Defaults to no validation.</param>
    public static OptionalEmit Combine(IEnumerable<OptionalEmit> emits, EmitOptions? options = null)
    {
        var emitList = emits.ToList();
        if (emitList.Count == 0)
            throw new ArgumentException("At least one emit is required.", nameof(emits));

        // Collect all diagnostics
        var allDiagnostics = emitList
            .SelectMany(e => e.Diagnostics)
            .ToImmutableArray();

        // Check if any emit failed
        var failedEmits = emitList.Where(e => !e.Success).ToList();
        if (failedEmits.Count > 0)
        {
            return FromFailure(allDiagnostics);
        }

        // All succeeded - combine the validated emits
        var validEmits = emitList.Select(e => e.ValidateOrThrow());
        var combined = ValidEmit.Combine(validEmits, options);

        return allDiagnostics.Length > 0
            ? FromDiagnostics(combined.Syntax, combined.Code, allDiagnostics)
            : FromSuccess(combined.Syntax, combined.Code);
    }

    /// <summary>
    /// Combines multiple optional emits into a single optional emit.
    /// If any emit failed, returns a failed result with all aggregated diagnostics.
    /// If all succeed, combines using ValidEmit.Combine and returns a successful result.
    /// </summary>
    /// <param name="emits">The optional emits to combine.</param>
    public static OptionalEmit Combine(params OptionalEmit[] emits)
        => Combine((IEnumerable<OptionalEmit>)emits);

    #endregion

    #region Convenience Methods

    /// <summary>
    /// Returns diagnostic information or the code if successful.
    /// </summary>
    public override string ToString()
    {
        if (Success)
            return _code!;

        var errorCount = Errors.Count();
        return $"Emit failed with {errorCount} error(s)";
    }

    #endregion
}
