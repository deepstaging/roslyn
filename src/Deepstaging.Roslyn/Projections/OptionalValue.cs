// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn;

/// <summary>
/// An optional value that may or may not contain data.
/// Provides a fluent API for mapping and transforming values with null-safety.
/// </summary>
/// <typeparam name="TSource">The source type of the value.</typeparam>
public readonly struct OptionalValue<TSource> : IProjection<TSource?>, IEquatable<OptionalValue<TSource>>
{
    private readonly TSource? _value;
    private readonly bool _hasValue;

    private OptionalValue(TSource? value, bool hasValue)
    {
        _value = value;
        _hasValue = hasValue;
    }

    /// <summary>
    /// Creates a projection with a value.
    /// </summary>
    public static OptionalValue<TSource> WithValue(TSource value)
    {
        return new OptionalValue<TSource>(value, true);
    }

    /// <summary>
    /// Creates a projection without a value.
    /// </summary>
    public static OptionalValue<TSource> Empty()
    {
        return new OptionalValue<TSource>(default, false);
    }

    /// <summary>
    /// Maps the source value to a different type using the provided function.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="mapper">The mapping function.</param>
    /// <returns>A new projected value with the result type.</returns>
    public OptionalValue<TResult> Map<TResult>(Func<TSource, TResult> mapper)
    {
        if (_hasValue && _value != null)
            return OptionalValue<TResult>.WithValue(mapper(_value));

        return OptionalValue<TResult>.Empty();
    }

    /// <summary>
    /// Maps the source value to a different type using the provided function (alias for Map).
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="selector">The selection function.</param>
    /// <returns>A new projected value with the result type.</returns>
    public OptionalValue<TResult> Select<TResult>(Func<TSource, TResult> selector)
    {
        return Map(selector);
    }

    /// <summary>
    /// Maps an integer value to an enum type.
    /// Useful for converting Roslyn attribute enum values (stored as int) to actual enum types.
    /// </summary>
    /// <typeparam name="TEnum">The enum type to convert to.</typeparam>
    /// <returns>A new projected value with the enum type.</returns>
    public OptionalValue<TEnum> ToEnum<TEnum>() where TEnum : struct, Enum
    {
        if (!_hasValue || _value == null)
            return OptionalValue<TEnum>.Empty();

        // Handle int → enum conversion (Roslyn stores enums as int)
        if (_value is int intValue && Enum.IsDefined(typeof(TEnum), intValue))
            return OptionalValue<TEnum>.WithValue((TEnum)Enum.ToObject(typeof(TEnum), intValue));

        // Handle string → enum conversion
        if (_value is string strValue && Enum.TryParse<TEnum>(strValue, out var enumValue))
            return OptionalValue<TEnum>.WithValue(enumValue);

        return OptionalValue<TEnum>.Empty();
    }

    /// <summary>
    /// Returns the value or throws an exception if not present.
    /// </summary>
    /// <param name="message">Optional error message.</param>
    /// <returns>The value.</returns>
    /// <exception cref="InvalidOperationException">Thrown when value is not present.</exception>
    public TSource OrThrow(string? message = null)
    {
        return _hasValue && _value != null
            ? _value
            : throw new InvalidOperationException(message ?? "Value not found");
    }

    /// <summary>
    /// Returns the value or throws an exception with a lazily-computed message if not present.
    /// </summary>
    /// <param name="messageFactory">Factory function to create the error message.</param>
    /// <returns>The value.</returns>
    /// <exception cref="InvalidOperationException">Thrown when value is not present.</exception>
    public TSource OrThrow(Func<string> messageFactory)
    {
        return _hasValue && _value != null
            ? _value
            : throw new InvalidOperationException(messageFactory());
    }

    /// <summary>
    /// Returns the value or throws a custom exception if not present.
    /// </summary>
    /// <param name="exceptionFactory">Factory function to create the exception.</param>
    /// <returns>The value.</returns>
    public TSource OrThrow(Func<Exception> exceptionFactory)
    {
        return _hasValue && _value != null ? _value : throw exceptionFactory();
    }

    /// <summary>
    /// Returns the value or a default value if not present.
    /// </summary>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>The value or default.</returns>
    public TSource OrDefault(TSource defaultValue)
    {
        return _hasValue && _value != null ? _value : defaultValue;
    }

    /// <summary>
    /// Returns the value or computes a default using the provided function.
    /// </summary>
    /// <param name="defaultFactory">Function to compute the default value.</param>
    /// <returns>The value or computed default.</returns>
    public TSource OrDefault(Func<TSource> defaultFactory)
    {
        return _hasValue && _value != null ? _value : defaultFactory();
    }

    /// <summary>
    /// Returns the value or null if not present.
    /// </summary>
    /// <returns>The value or null.</returns>
    public TSource? OrNull()
    {
        return _hasValue ? _value : default;
    }

    /// <summary>
    /// Executes an action if the value is present.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns>This projected value for chaining.</returns>
    public OptionalValue<TSource> Do(Action<TSource> action)
    {
        if (_hasValue && _value != null)
            action(_value);

        return this;
    }

    /// <summary>
    /// Executes an action if the value is not present.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns>This projected value for chaining.</returns>
    public OptionalValue<TSource> OrElse(Action action)
    {
        if (!_hasValue)
            action();

        return this;
    }

    /// <summary>
    /// Returns true if a value is present.
    /// </summary>
    public bool HasValue => _hasValue;

    /// <summary>
    /// Returns true if no value is present (negation of HasValue).
    /// </summary>
    public bool IsEmpty => !_hasValue;

    /// <summary>
    /// Gets the underlying value. Throws if value is not present.
    /// Use this when you've already verified HasValue or within a Match/Map callback.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when value is not present.</exception>
    public TSource Value => _hasValue && _value != null ? _value : throw new InvalidOperationException("Value is empty");

    /// <summary>
    /// Pattern matching with discriminated union semantics.
    /// Executes the appropriate function based on whether the value is present.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="whenPresent">Function to execute when value is present (receives non-null value).</param>
    /// <param name="whenEmpty">Function to execute when value is empty.</param>
    /// <returns>The result from the executed function.</returns>
    /// <example>
    /// <code>
    /// var result = optionalValue.Match(
    ///     whenPresent: v => v.ToString(),
    ///     whenEmpty: () => "NoValue");
    /// </code>
    /// </example>
    public TResult Match<TResult>(
        Func<TSource, TResult> whenPresent,
        Func<TResult> whenEmpty)
    {
        return _hasValue && _value != null ? whenPresent(_value) : whenEmpty();
    }

    /// <summary>
    /// Pattern matching with void return (for side effects).
    /// Executes the appropriate action based on whether the value is present.
    /// </summary>
    /// <param name="whenPresent">Action to execute when value is present (receives non-null value).</param>
    /// <param name="whenEmpty">Action to execute when value is empty.</param>
    public void Match(
        Action<TSource> whenPresent,
        Action whenEmpty)
    {
        if (_hasValue && _value != null)
            whenPresent(_value);
        else
            whenEmpty();
    }

    /// <summary>
    /// Attempts to get the value. Returns true if value is present.
    /// Standard .NET pattern for extracting values from optional types.
    /// </summary>
    /// <param name="value">The value if present, default if not.</param>
    /// <returns>True if value is present, false otherwise.</returns>
    public bool TryGetValue(out TSource? value)
    {
        value = _value;
        return _hasValue && _value != null;
    }

    /// <summary>
    /// Checks if no value is present. Returns true if value is absent.
    /// Use for early-exit scenarios: if (optional.IsMissing(out var x)) return;
    /// </summary>
    /// <param name="value">Always set to default when this returns true.</param>
    /// <returns>True if value is absent, false otherwise.</returns>
    public bool IsMissing(out TSource? value)
    {
        value = default;
        return !_hasValue || _value == null;
    }

    /// <summary>
    /// Checks if the projected value equals the specified value.
    /// Returns false if no value is present.
    /// </summary>
    /// <param name="other">The value to compare against.</param>
    /// <returns>True if values are equal, false otherwise.</returns>
    public bool Equals(TSource? other)
    {
        return _hasValue &&
               _value != null &&
               other != null &&
               EqualityComparer<TSource>.Default.Equals(_value, other);
    }

    /// <summary>
    /// Enables equality checks: projected == value
    /// </summary>
    public static bool operator ==(OptionalValue<TSource> left, TSource? right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Enables inequality checks: projected != value
    /// </summary>
    public static bool operator !=(OptionalValue<TSource> left, TSource? right)
    {
        return !left.Equals(right);
    }

    /// <summary>
    /// Determines whether the current instance equals the specified object (always returns false).
    /// </summary>
    public override bool Equals(object? obj)
    {
        return false;
    }

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    public override int GetHashCode()
    {
        return _hasValue ? _value?.GetHashCode() ?? 0 : 0;
    }

    /// <summary>
    /// Determines whether the current instance equals another OptionalValue instance.
    /// </summary>
    public bool Equals(OptionalValue<TSource> other)
    {
        return EqualityComparer<TSource?>.Default.Equals(_value, other._value);
    }
}