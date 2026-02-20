// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Types;

public class DbSetTypeRefTests
{
    [Test]
    public async Task Creates_globally_qualified_type()
    {
        var typeRef = new DbSetTypeRef("Customer");

        await Assert.That((string)typeRef)
            .IsEqualTo("global::Microsoft.EntityFrameworkCore.DbSet<Customer>");
    }

    [Test]
    public async Task Carries_entity_type()
    {
        var typeRef = new DbSetTypeRef("Customer");

        await Assert.That((string)typeRef.EntityType).IsEqualTo("Customer");
    }
}

public class EntityTypeBuilderTypeRefTests
{
    [Test]
    public async Task Creates_globally_qualified_type()
    {
        var typeRef = new EntityTypeBuilderTypeRef("Customer");

        await Assert.That((string)typeRef)
            .IsEqualTo("global::Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Customer>");
    }

    [Test]
    public async Task Carries_entity_type()
    {
        var typeRef = new EntityTypeBuilderTypeRef("Customer");

        await Assert.That((string)typeRef.EntityType).IsEqualTo("Customer");
    }
}

public class EntityTypeConfigurationTypeRefTests
{
    [Test]
    public async Task Creates_globally_qualified_type()
    {
        var typeRef = new EntityTypeConfigurationTypeRef("Customer");

        await Assert.That((string)typeRef)
            .IsEqualTo("global::Microsoft.EntityFrameworkCore.IEntityTypeConfiguration<Customer>");
    }

    [Test]
    public async Task Carries_entity_type()
    {
        var typeRef = new EntityTypeConfigurationTypeRef("Customer");

        await Assert.That((string)typeRef.EntityType).IsEqualTo("Customer");
    }
}

public class EntityFrameworkTypesTests
{
    [Test]
    public async Task DbContext_produces_correct_type()
    {
        await Assert.That(EntityFrameworkTypes.DbContext.Value)
            .IsEqualTo("global::Microsoft.EntityFrameworkCore.DbContext");
    }

    [Test]
    public async Task ModelBuilder_produces_correct_type()
    {
        await Assert.That(EntityFrameworkTypes.ModelBuilder.Value)
            .IsEqualTo("global::Microsoft.EntityFrameworkCore.ModelBuilder");
    }

    [Test]
    public async Task DbSet_produces_generic_DbSet()
    {
        var typeRef = EntityFrameworkTypes.DbSet("Customer");

        await Assert.That((string)typeRef)
            .IsEqualTo("global::Microsoft.EntityFrameworkCore.DbSet<Customer>");
    }

    [Test]
    public async Task EntityTypeBuilder_produces_generic_EntityTypeBuilder()
    {
        var typeRef = EntityFrameworkTypes.EntityTypeBuilder("Customer");

        await Assert.That((string)typeRef)
            .IsEqualTo("global::Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Customer>");
    }

    [Test]
    public async Task EntityTypeConfiguration_produces_generic_IEntityTypeConfiguration()
    {
        var typeRef = EntityFrameworkTypes.EntityTypeConfiguration("Customer");

        await Assert.That((string)typeRef)
            .IsEqualTo("global::Microsoft.EntityFrameworkCore.IEntityTypeConfiguration<Customer>");
    }
}

public class EntityFrameworkAttributesTests
{
    [Test]
    public async Task Key_produces_valid_attribute()
    {
        await Assert.That(EntityFrameworkAttributes.Key.Value)
            .IsEqualTo("global::System.ComponentModel.DataAnnotations.KeyAttribute");
    }

    [Test]
    public async Task Required_produces_valid_attribute()
    {
        await Assert.That(EntityFrameworkAttributes.Required.Value)
            .IsEqualTo("global::System.ComponentModel.DataAnnotations.RequiredAttribute");
    }

    [Test]
    public async Task MaxLength_produces_valid_attribute()
    {
        await Assert.That(EntityFrameworkAttributes.MaxLength.Value)
            .IsEqualTo("global::System.ComponentModel.DataAnnotations.MaxLengthAttribute");
    }

    [Test]
    public async Task StringLength_produces_valid_attribute()
    {
        await Assert.That(EntityFrameworkAttributes.StringLength.Value)
            .IsEqualTo("global::System.ComponentModel.DataAnnotations.StringLengthAttribute");
    }

    [Test]
    public async Task Range_produces_valid_attribute()
    {
        await Assert.That(EntityFrameworkAttributes.Range.Value)
            .IsEqualTo("global::System.ComponentModel.DataAnnotations.RangeAttribute");
    }

    [Test]
    public async Task Table_produces_valid_attribute()
    {
        await Assert.That(EntityFrameworkAttributes.Table.Value)
            .IsEqualTo("global::System.ComponentModel.DataAnnotations.Schema.TableAttribute");
    }

    [Test]
    public async Task Column_produces_valid_attribute()
    {
        await Assert.That(EntityFrameworkAttributes.Column.Value)
            .IsEqualTo("global::System.ComponentModel.DataAnnotations.Schema.ColumnAttribute");
    }

    [Test]
    public async Task ForeignKey_produces_valid_attribute()
    {
        await Assert.That(EntityFrameworkAttributes.ForeignKey.Value)
            .IsEqualTo("global::System.ComponentModel.DataAnnotations.Schema.ForeignKeyAttribute");
    }

    [Test]
    public async Task NotMapped_produces_valid_attribute()
    {
        await Assert.That(EntityFrameworkAttributes.NotMapped.Value)
            .IsEqualTo("global::System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute");
    }

    [Test]
    public async Task DatabaseGenerated_produces_valid_attribute()
    {
        await Assert.That(EntityFrameworkAttributes.DatabaseGenerated.Value)
            .IsEqualTo("global::System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedAttribute");
    }
}
