// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Types;

/// <summary>
/// A type-safe wrapper representing a <c>DbSet&lt;T&gt;</c> type reference.
/// Carries the entity type for typed expression building.
/// </summary>
public readonly record struct DbSetTypeRef
{
    /// <summary>Gets the entity type (e.g., <c>"Customer"</c>).</summary>
    public TypeRef EntityType { get; }

    /// <summary>Creates a <c>DbSetTypeRef</c> for the given entity type.</summary>
    public DbSetTypeRef(TypeRef entityType) => EntityType = entityType;

    /// <summary>Gets the globally qualified <c>DbSet&lt;T&gt;</c> type string.</summary>
    public override string ToString() =>
        $"global::Microsoft.EntityFrameworkCore.DbSet<{EntityType.Value}>";

    /// <summary>Implicitly converts to <see cref="TypeRef"/> for use in type declarations.</summary>
    public static implicit operator TypeRef(DbSetTypeRef self) =>
        TypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(DbSetTypeRef self) =>
        self.ToString();
}

/// <summary>
/// A type-safe wrapper representing an <c>EntityTypeBuilder&lt;T&gt;</c> type reference.
/// Carries the entity type for typed expression building.
/// </summary>
public readonly record struct EntityTypeBuilderTypeRef
{
    /// <summary>Gets the entity type (e.g., <c>"Customer"</c>).</summary>
    public TypeRef EntityType { get; }

    /// <summary>Creates an <c>EntityTypeBuilderTypeRef</c> for the given entity type.</summary>
    public EntityTypeBuilderTypeRef(TypeRef entityType) => EntityType = entityType;

    /// <summary>Gets the globally qualified <c>EntityTypeBuilder&lt;T&gt;</c> type string.</summary>
    public override string ToString() =>
        $"global::Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<{EntityType.Value}>";

    /// <summary>Implicitly converts to <see cref="TypeRef"/> for use in type declarations.</summary>
    public static implicit operator TypeRef(EntityTypeBuilderTypeRef self) =>
        TypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(EntityTypeBuilderTypeRef self) =>
        self.ToString();
}

/// <summary>
/// A type-safe wrapper representing an <c>IEntityTypeConfiguration&lt;T&gt;</c> type reference.
/// Carries the entity type for typed expression building.
/// </summary>
public readonly record struct EntityTypeConfigurationTypeRef
{
    /// <summary>Gets the entity type (e.g., <c>"Customer"</c>).</summary>
    public TypeRef EntityType { get; }

    /// <summary>Creates an <c>EntityTypeConfigurationTypeRef</c> for the given entity type.</summary>
    public EntityTypeConfigurationTypeRef(TypeRef entityType) => EntityType = entityType;

    /// <summary>Gets the globally qualified <c>IEntityTypeConfiguration&lt;T&gt;</c> type string.</summary>
    public override string ToString() =>
        $"global::Microsoft.EntityFrameworkCore.IEntityTypeConfiguration<{EntityType.Value}>";

    /// <summary>Implicitly converts to <see cref="TypeRef"/> for use in type declarations.</summary>
    public static implicit operator TypeRef(EntityTypeConfigurationTypeRef self) =>
        TypeRef.From(self.ToString());

    /// <summary>Implicitly converts to <see cref="string"/> for use in string interpolation.</summary>
    public static implicit operator string(EntityTypeConfigurationTypeRef self) =>
        self.ToString();
}

/// <summary>
/// Convenience <see cref="TypeRef"/> constants for Entity Framework Core types.
/// </summary>
public static class EntityFrameworkTypes
{
    /// <summary>Gets the <c>Microsoft.EntityFrameworkCore</c> namespace.</summary>
    public static NamespaceRef Namespace => NamespaceRef.From("Microsoft.EntityFrameworkCore");

    /// <summary>Gets the <c>Microsoft.EntityFrameworkCore.Metadata.Builders</c> namespace.</summary>
    public static NamespaceRef BuildersNamespace => NamespaceRef.From("Microsoft.EntityFrameworkCore.Metadata.Builders");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>DbContext</c>.</summary>
    public static TypeRef DbContext => Namespace.GlobalType("DbContext");

    /// <summary>Gets a <see cref="TypeRef"/> for <c>ModelBuilder</c>.</summary>
    public static TypeRef ModelBuilder => Namespace.GlobalType("ModelBuilder");

    /// <summary>Creates a <c>DbSet&lt;T&gt;</c> type reference.</summary>
    public static DbSetTypeRef DbSet(TypeRef entityType) => new(entityType);

    /// <summary>Creates an <c>EntityTypeBuilder&lt;T&gt;</c> type reference.</summary>
    public static EntityTypeBuilderTypeRef EntityTypeBuilder(TypeRef entityType) => new(entityType);

    /// <summary>Creates an <c>IEntityTypeConfiguration&lt;T&gt;</c> type reference.</summary>
    public static EntityTypeConfigurationTypeRef EntityTypeConfiguration(TypeRef entityType) => new(entityType);
}

/// <summary>
/// Convenience <see cref="AttributeRef"/> constants for Entity Framework data annotations.
/// </summary>
public static class EntityFrameworkAttributes
{
    private static NamespaceRef DataAnnotationsNamespace => NamespaceRef.From("System.ComponentModel.DataAnnotations");
    private static NamespaceRef SchemaNamespace => NamespaceRef.From("System.ComponentModel.DataAnnotations.Schema");

    /// <summary>Gets an <see cref="AttributeRef"/> for <c>[Key]</c>.</summary>
    public static AttributeRef Key => DataAnnotationsNamespace.GlobalAttribute("KeyAttribute");

    /// <summary>Gets an <see cref="AttributeRef"/> for <c>[Required]</c>.</summary>
    public static AttributeRef Required => DataAnnotationsNamespace.GlobalAttribute("RequiredAttribute");

    /// <summary>Gets an <see cref="AttributeRef"/> for <c>[MaxLength]</c>.</summary>
    public static AttributeRef MaxLength => DataAnnotationsNamespace.GlobalAttribute("MaxLengthAttribute");

    /// <summary>Gets an <see cref="AttributeRef"/> for <c>[StringLength]</c>.</summary>
    public static AttributeRef StringLength => DataAnnotationsNamespace.GlobalAttribute("StringLengthAttribute");

    /// <summary>Gets an <see cref="AttributeRef"/> for <c>[Range]</c>.</summary>
    public static AttributeRef Range => DataAnnotationsNamespace.GlobalAttribute("RangeAttribute");

    /// <summary>Gets an <see cref="AttributeRef"/> for <c>[Table]</c>.</summary>
    public static AttributeRef Table => SchemaNamespace.GlobalAttribute("TableAttribute");

    /// <summary>Gets an <see cref="AttributeRef"/> for <c>[Column]</c>.</summary>
    public static AttributeRef Column => SchemaNamespace.GlobalAttribute("ColumnAttribute");

    /// <summary>Gets an <see cref="AttributeRef"/> for <c>[ForeignKey]</c>.</summary>
    public static AttributeRef ForeignKey => SchemaNamespace.GlobalAttribute("ForeignKeyAttribute");

    /// <summary>Gets an <see cref="AttributeRef"/> for <c>[NotMapped]</c>.</summary>
    public static AttributeRef NotMapped => SchemaNamespace.GlobalAttribute("NotMappedAttribute");

    /// <summary>Gets an <see cref="AttributeRef"/> for <c>[DatabaseGenerated]</c>.</summary>
    public static AttributeRef DatabaseGenerated => SchemaNamespace.GlobalAttribute("DatabaseGeneratedAttribute");
}
