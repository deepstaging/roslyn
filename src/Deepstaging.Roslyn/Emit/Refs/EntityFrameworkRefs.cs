// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Emit.Refs;

/// <summary>
/// Factory members for common <c>Microsoft.EntityFrameworkCore</c> types.
/// </summary>
public static class EntityFrameworkRefs
{
    /// <summary>Gets the <c>Microsoft.EntityFrameworkCore</c> namespace.</summary>
    public static NamespaceRef Namespace => NamespaceRef.From("Microsoft.EntityFrameworkCore");

    /// <summary>Gets the <c>Microsoft.EntityFrameworkCore.Metadata.Builders</c> namespace.</summary>
    public static NamespaceRef BuildersNamespace => NamespaceRef.From("Microsoft.EntityFrameworkCore.Metadata.Builders");

    // ── Core Types ──────────────────────────────────────────────────────

    /// <summary>Gets a <c>DbContext</c> type reference.</summary>
    public static TypeRef DbContext => Namespace.GlobalType("DbContext");

    /// <summary>Gets a <c>DbSet&lt;T&gt;</c> type reference.</summary>
    public static TypeRef DbSet(TypeRef entityType) =>
        Namespace.GlobalType($"DbSet<{entityType.Value}>");

    /// <summary>Gets a <c>ModelBuilder</c> type reference.</summary>
    public static TypeRef ModelBuilder => Namespace.GlobalType("ModelBuilder");

    /// <summary>Gets an <c>EntityTypeBuilder&lt;T&gt;</c> type reference.</summary>
    public static TypeRef EntityTypeBuilder(TypeRef entityType) =>
        BuildersNamespace.GlobalType($"EntityTypeBuilder<{entityType.Value}>");

    /// <summary>Gets an <c>IEntityTypeConfiguration&lt;T&gt;</c> type reference.</summary>
    public static TypeRef IEntityTypeConfiguration(TypeRef entityType) =>
        Namespace.GlobalType($"IEntityTypeConfiguration<{entityType.Value}>");

    // ── Attributes ──────────────────────────────────────────────────────

    /// <summary>Gets the <c>System.ComponentModel.DataAnnotations</c> namespace.</summary>
    public static NamespaceRef DataAnnotationsNamespace => NamespaceRef.From("System.ComponentModel.DataAnnotations");

    /// <summary>Gets the <c>System.ComponentModel.DataAnnotations.Schema</c> namespace.</summary>
    public static NamespaceRef SchemaNamespace => NamespaceRef.From("System.ComponentModel.DataAnnotations.Schema");

    /// <summary>Gets a <c>KeyAttribute</c> attribute reference.</summary>
    public static AttributeRef Key => DataAnnotationsNamespace.GlobalAttribute("KeyAttribute");

    /// <summary>Gets a <c>RequiredAttribute</c> attribute reference.</summary>
    public static AttributeRef Required => DataAnnotationsNamespace.GlobalAttribute("RequiredAttribute");

    /// <summary>Gets a <c>MaxLengthAttribute</c> attribute reference.</summary>
    public static AttributeRef MaxLength => DataAnnotationsNamespace.GlobalAttribute("MaxLengthAttribute");

    /// <summary>Gets a <c>StringLengthAttribute</c> attribute reference.</summary>
    public static AttributeRef StringLength => DataAnnotationsNamespace.GlobalAttribute("StringLengthAttribute");

    /// <summary>Gets a <c>RangeAttribute</c> attribute reference.</summary>
    public static AttributeRef Range => DataAnnotationsNamespace.GlobalAttribute("RangeAttribute");

    /// <summary>Gets a <c>TableAttribute</c> attribute reference.</summary>
    public static AttributeRef Table => SchemaNamespace.GlobalAttribute("TableAttribute");

    /// <summary>Gets a <c>ColumnAttribute</c> attribute reference.</summary>
    public static AttributeRef Column => SchemaNamespace.GlobalAttribute("ColumnAttribute");

    /// <summary>Gets a <c>ForeignKeyAttribute</c> attribute reference.</summary>
    public static AttributeRef ForeignKey => SchemaNamespace.GlobalAttribute("ForeignKeyAttribute");

    /// <summary>Gets a <c>NotMappedAttribute</c> attribute reference.</summary>
    public static AttributeRef NotMapped => SchemaNamespace.GlobalAttribute("NotMappedAttribute");

    /// <summary>Gets a <c>DatabaseGeneratedAttribute</c> attribute reference.</summary>
    public static AttributeRef DatabaseGenerated => SchemaNamespace.GlobalAttribute("DatabaseGeneratedAttribute");

    // ── Well-Known API Calls ────────────────────────────────────────────

    /// <summary>Produces a <c>context.Set&lt;T&gt;()</c> expression.</summary>
    public static ExpressionRef Set(ExpressionRef context, TypeRef entityType) =>
        ExpressionRef.From(context).Call($"Set<{entityType.Value}>");

    /// <summary>Produces a <c>context.SaveChangesAsync(cancellationToken)</c> expression.</summary>
    public static ExpressionRef SaveChangesAsync(ExpressionRef context, ExpressionRef cancellationToken) =>
        ExpressionRef.From(context).Call("SaveChangesAsync", cancellationToken);

    /// <summary>Produces a <c>context.SaveChangesAsync()</c> expression.</summary>
    public static ExpressionRef SaveChangesAsync(ExpressionRef context) =>
        ExpressionRef.From(context).Call("SaveChangesAsync");

    /// <summary>Produces a <c>dbSet.FindAsync(keys)</c> expression.</summary>
    public static ExpressionRef FindAsync(ExpressionRef dbSet, params ExpressionRef[] keys) =>
        ExpressionRef.From(dbSet).Call("FindAsync", keys);

    /// <summary>Produces a <c>dbSet.AddAsync(entity)</c> expression.</summary>
    public static ExpressionRef AddAsync(ExpressionRef dbSet, ExpressionRef entity) =>
        ExpressionRef.From(dbSet).Call("AddAsync", entity);

    /// <summary>Produces a <c>dbSet.Remove(entity)</c> expression.</summary>
    public static ExpressionRef Remove(ExpressionRef dbSet, ExpressionRef entity) =>
        ExpressionRef.From(dbSet).Call("Remove", entity);
}
