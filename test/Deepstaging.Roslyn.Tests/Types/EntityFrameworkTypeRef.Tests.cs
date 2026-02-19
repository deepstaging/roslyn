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
}
