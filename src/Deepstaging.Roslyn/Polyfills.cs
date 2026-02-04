// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
// Polyfills for netstandard2.0 to enable modern C# language features.
// These types are normally provided by the runtime in .NET 5+ but must be
// defined for netstandard2.0 targeting.

// ReSharper disable once CheckNamespace
namespace System
{
    using ComponentModel;
    using Runtime.CompilerServices;

    /// <summary>Represent a type can be used to index a collection either from the start or the end.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal readonly struct Index : IEquatable<Index>
    {
        private readonly int _value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Index(int value, bool fromEnd = false)
        {
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(value));
            _value = fromEnd ? ~value : value;
        }

        private Index(int value)
        {
            _value = value;
        }

        public static Index Start => new(0);
        public static Index End => new(~0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Index FromStart(int value)
        {
            return value < 0
                ? throw new ArgumentOutOfRangeException(nameof(value))
                : new Index(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Index FromEnd(int value)
        {
            return value < 0
                ? throw new ArgumentOutOfRangeException(nameof(value))
                : new Index(~value);
        }

        public int Value => _value < 0 ? ~_value : _value;
        public bool IsFromEnd => _value < 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetOffset(int length)
        {
            return IsFromEnd ? length - ~_value : _value;
        }

        public override bool Equals(object? value)
        {
            return value is Index index && _value == index._value;
        }

        public bool Equals(Index other)
        {
            return _value == other._value;
        }

        public override int GetHashCode()
        {
            return _value;
        }

        public static implicit operator Index(int value)
        {
            return FromStart(value);
        }

        public override string ToString()
        {
            return IsFromEnd ? $"^{(uint)Value}" : ((uint)Value).ToString();
        }
    }

    /// <summary>Represent a range has start and end indexes.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal readonly struct Range : IEquatable<Range>
    {
        public Index Start { get; }
        public Index End { get; }

        public Range(Index start, Index end)
        {
            Start = start;
            End = end;
        }

        public override bool Equals(object? value)
        {
            return value is Range r && r.Start.Equals(Start) && r.End.Equals(End);
        }

        public bool Equals(Range other)
        {
            return other.Start.Equals(Start) && other.End.Equals(End);
        }

        public override int GetHashCode()
        {
            return Start.GetHashCode() * 31 + End.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Start}..{End}";
        }

        public static Range StartAt(Index start)
        {
            return new Range(start, Index.End);
        }

        public static Range EndAt(Index end)
        {
            return new Range(Index.Start, end);
        }

        public static Range All => new(Index.Start, Index.End);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (int Offset, int Length) GetOffsetAndLength(int length)
        {
            var start = Start.GetOffset(length);
            var end = End.GetOffset(length);
            if ((uint)end > (uint)length || (uint)start > (uint)end)
                throw new ArgumentOutOfRangeException(nameof(length));
            return (start, end - start);
        }
    }
}

// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices
{
    using ComponentModel;

    /// <summary>
    /// Reserved to be used by the compiler for tracking metadata.
    /// This class should not be used by developers in source code.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static class IsExternalInit;

    /// <summary>
    /// Indicates that a type has required members.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property,
        AllowMultiple = false, Inherited = false)]
    internal sealed class RequiredMemberAttribute : Attribute;

    /// <summary>
    /// Specifies that this constructor sets all required members.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    internal sealed class CompilerFeatureRequiredAttribute : Attribute
    {
        public CompilerFeatureRequiredAttribute(string featureName)
        {
            FeatureName = featureName;
        }

        public string FeatureName { get; }
        public bool IsOptional { get; init; }
        public const string RefStructs = nameof(RefStructs);
        public const string RequiredMembers = nameof(RequiredMembers);
    }
}

// ReSharper disable once CheckNamespace
namespace System.Diagnostics.CodeAnalysis
{
    using ComponentModel;

    /// <summary>
    /// Specifies that this constructor sets all required members.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    internal sealed class SetsRequiredMembersAttribute : Attribute;
}