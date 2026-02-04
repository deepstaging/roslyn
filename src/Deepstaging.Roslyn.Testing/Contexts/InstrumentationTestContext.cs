// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using System.Diagnostics;
using LanguageExt.Common;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Deepstaging.Roslyn.Testing;

/// <summary>
/// Contains captured instrumentation data from an effect execution for testing assertions.
/// </summary>
public sealed class InstrumentationTestContext : IDisposable
{
    private readonly ActivityListener _listener;

    internal InstrumentationTestContext(
        Activity? activity,
        ActivityStatusCode? status,
        string? statusDescription,
        IReadOnlyDictionary<string, object?> tags,
        long? durationMs,
        bool wasActivityStarted,
        object? result,
        Error? error,
        ActivityListener listener)
    {
        Activity = activity;
        Status = status;
        StatusDescription = statusDescription;
        Tags = tags;
        DurationMs = durationMs;
        WasActivityStarted = wasActivityStarted;
        Result = result;
        Error = error;
        _listener = listener;
    }

    /// <summary>
    /// The captured Activity, or null if no activity was started.
    /// </summary>
    public Activity? Activity { get; }

    /// <summary>
    /// The activity status code (Ok, Error, Unset), or null if no activity was captured.
    /// </summary>
    public ActivityStatusCode? Status { get; }

    /// <summary>
    /// The status description message, typically set on error.
    /// </summary>
    public string? StatusDescription { get; }

    /// <summary>
    /// All tags set on the activity during execution.
    /// </summary>
    public IReadOnlyDictionary<string, object?> Tags { get; }

    /// <summary>
    /// Duration of the operation in milliseconds, if captured as a tag.
    /// </summary>
    public long? DurationMs { get; }

    /// <summary>
    /// Whether an activity was started at all during effect execution.
    /// </summary>
    public bool WasActivityStarted { get; }

    /// <summary>
    /// The original result value from the effect, or null if the effect failed.
    /// </summary>
    public object? Result { get; }

    /// <summary>
    /// The error from a failed effect, or null if the effect succeeded.
    /// </summary>
    public Error? Error { get; }

    /// <summary>
    /// Whether the effect succeeded.
    /// </summary>
    public bool IsSuccess => Error == null;

    /// <summary>
    /// Whether the effect failed.
    /// </summary>
    public bool IsFailure => Error != null;

    /// <summary>
    /// The activity's operation name.
    /// </summary>
    public string? OperationName => Activity?.OperationName;

    /// <summary>
    /// The activity's display name.
    /// </summary>
    public string? DisplayName => Activity?.DisplayName;

    /// <summary>
    /// The activity's trace ID.
    /// </summary>
    public ActivityTraceId? TraceId => Activity?.TraceId;

    /// <summary>
    /// The activity's span ID.
    /// </summary>
    public ActivitySpanId? SpanId => Activity?.SpanId;

    /// <summary>
    /// The parent activity's span ID, if present.
    /// </summary>
    public ActivitySpanId? ParentSpanId => Activity?.ParentSpanId;

    /// <summary>
    /// The activity's trace state.
    /// </summary>
    public string? TraceState => Activity?.TraceStateString;

    /// <summary>
    /// Events recorded on the activity.
    /// </summary>
    public IEnumerable<ActivityEvent> Events => Activity?.Events ?? [];

    /// <summary>
    /// Links to other activities.
    /// </summary>
    public IEnumerable<ActivityLink> Links => Activity?.Links ?? [];

    /// <summary>
    /// Baggage items attached to the activity.
    /// </summary>
    public IEnumerable<KeyValuePair<string, string?>> Baggage => Activity?.Baggage ?? [];

    /// <summary>
    /// The activity kind (Internal, Server, Client, Producer, Consumer).
    /// </summary>
    public ActivityKind? Kind => Activity?.Kind;

    /// <summary>
    /// Gets the typed result value, throwing if the effect failed or type doesn't match.
    /// </summary>
    public TResult GetResult<TResult>()
    {
        if (Error != null)
            throw new InvalidOperationException($"Cannot get result from failed effect: {Error.Message}");
        
        if (Result is TResult typedResult)
            return typedResult;
        
        throw new InvalidCastException($"Result is of type {Result?.GetType().Name ?? "null"}, not {typeof(TResult).Name}");
    }

    /// <summary>
    /// Tries to get the typed result value.
    /// </summary>
    public bool TryGetResult<TResult>(out TResult? result)
    {
        if (Error == null && Result is TResult typedResult)
        {
            result = typedResult;
            return true;
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Gets a tag value by key.
    /// </summary>
    public object? GetTag(string key) => Tags.TryGetValue(key, out var value) ? value : null;

    /// <summary>
    /// Tries to get a tag value by key.
    /// </summary>
    public bool TryGetTag(string key, out object? value) => Tags.TryGetValue(key, out value);

    public void Dispose()
    {
        _listener.Dispose();
        Activity?.Dispose();
    }
}
