// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Refs;

/// <summary>
/// Factory members for common <c>System</c> types.
/// </summary>
/// <remarks>
/// For exception types see <see cref="ExceptionRefs"/>.
/// For <c>Func</c> and <c>Action</c> delegates see <see cref="DelegateRefs"/>.
/// </remarks>
public static class SystemRefs
{
    /// <summary>Gets the <c>System</c> namespace.</summary>
    public static NamespaceRef Namespace => NamespaceRef.From("System");

    // ── Value Types ─────────────────────────────────────────────────────

    /// <summary>Gets a <c>Guid</c> type reference.</summary>
    public static TypeRef Guid => Namespace.GlobalType("Guid");

    /// <summary>Gets a <c>DateTime</c> type reference.</summary>
    public static TypeRef DateTime => Namespace.GlobalType("DateTime");

    /// <summary>Gets a <c>DateTimeOffset</c> type reference.</summary>
    public static TypeRef DateTimeOffset => Namespace.GlobalType("DateTimeOffset");

    /// <summary>Gets a <c>TimeSpan</c> type reference.</summary>
    public static TypeRef TimeSpan => Namespace.GlobalType("TimeSpan");

    /// <summary>Gets a <c>DateOnly</c> type reference.</summary>
    public static TypeRef DateOnly => Namespace.GlobalType("DateOnly");

    /// <summary>Gets a <c>TimeOnly</c> type reference.</summary>
    public static TypeRef TimeOnly => Namespace.GlobalType("TimeOnly");

    // ── Utility Types ───────────────────────────────────────────────────

    /// <summary>Gets a <c>Uri</c> type reference.</summary>
    public static TypeRef Uri => Namespace.GlobalType("Uri");

    /// <summary>Gets a <c>Version</c> type reference.</summary>
    public static TypeRef Version => Namespace.GlobalType("Version");

    /// <summary>Gets a <c>Convert</c> type reference.</summary>
    public static TypeRef Convert => Namespace.GlobalType("Convert");

    /// <summary>Gets a <c>Lazy&lt;T&gt;</c> type reference.</summary>
    public static TypeRef Lazy(TypeRef valueType) =>
        Namespace.GlobalType($"Lazy<{valueType.Value}>");

    /// <summary>Gets a <c>Nullable&lt;T&gt;</c> type reference.</summary>
    public static TypeRef Nullable(TypeRef valueType) =>
        Namespace.GlobalType($"Nullable<{valueType.Value}>");

    // ── Well-Known API Calls ────────────────────────────────────────────

    /// <summary>Produces a <c>Guid.NewGuid()</c> expression.</summary>
    public static ExpressionRef NewGuid() => Guid.Call("NewGuid");

    /// <summary>Produces a <c>Guid.Parse(value)</c> expression.</summary>
    public static ExpressionRef GuidParse(ExpressionRef value) => Guid.Call("Parse", value);

    /// <summary>Gets a <c>Guid.Empty</c> expression.</summary>
    public static ExpressionRef GuidEmpty => Guid.Member("Empty");

    /// <summary>Gets a <c>DateTime.UtcNow</c> expression.</summary>
    public static ExpressionRef UtcNow => DateTime.Member("UtcNow");

    /// <summary>Gets a <c>DateTime.Now</c> expression.</summary>
    public static ExpressionRef Now => DateTime.Member("Now");

    /// <summary>Gets a <c>DateTimeOffset.UtcNow</c> expression.</summary>
    public static ExpressionRef DateTimeOffsetUtcNow => DateTimeOffset.Member("UtcNow");
}
