// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Emit;

public class RegionTests : RoslynTestBase
{
    #region Auto-Regions (UseRegions)

    [Test]
    public async Task Auto_regions_wraps_each_category()
    {
        var result = TypeBuilder
            .Class("Customer")
            .WithAccessibility(Accessibility.Public)
            .AddField("_name", "string", f => f)
            .AddProperty("Name", "string", p => p.WithAutoPropertyAccessors())
            .AddMethod("GetName", m => m.WithExpressionBody("_name"))
            .Emit(new EmitOptions { UseRegions = true, ValidationLevel = ValidationLevel.None });

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("#region Fields");
        await Assert.That(result.Code).Contains("#region Properties");
        await Assert.That(result.Code).Contains("#region Methods");
        await Assert.That(result.Code).Contains("#endregion");
    }

    [Test]
    public async Task Auto_regions_omits_empty_categories()
    {
        var result = TypeBuilder
            .Class("Customer")
            .WithAccessibility(Accessibility.Public)
            .AddProperty("Name", "string", p => p.WithAutoPropertyAccessors())
            .Emit(new EmitOptions { UseRegions = true, ValidationLevel = ValidationLevel.None });

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("#region Properties");
        await Assert.That(result.Code).DoesNotContain("#region Fields");
        await Assert.That(result.Code).DoesNotContain("#region Methods");
    }

    [Test]
    public async Task No_regions_by_default()
    {
        var result = TypeBuilder
            .Class("Customer")
            .WithAccessibility(Accessibility.Public)
            .AddField("_name", "string", f => f)
            .AddProperty("Name", "string", p => p.WithAutoPropertyAccessors())
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).DoesNotContain("#region");
        await Assert.That(result.Code).DoesNotContain("#endregion");
    }

    #endregion

    #region Tag-Per-Member (InRegion)

    [Test]
    public async Task InRegion_tags_property()
    {
        var result = TypeBuilder
            .Class("Customer")
            .WithAccessibility(Accessibility.Public)
            .AddProperty("Name", "string", p => p
                .WithAutoPropertyAccessors()
                .InRegion("Data"))
            .Emit(new EmitOptions { ValidationLevel = ValidationLevel.None });

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("#region Data");
        await Assert.That(result.Code).Contains("#endregion");
    }

    [Test]
    public async Task InRegion_groups_multiple_tagged_members()
    {
        var result = TypeBuilder
            .Class("Customer")
            .WithAccessibility(Accessibility.Public)
            .AddProperty("Name", "string", p => p
                .WithAutoPropertyAccessors()
                .InRegion("Data"))
            .AddProperty("Age", "int", p => p
                .WithAutoPropertyAccessors()
                .InRegion("Data"))
            .Emit(new EmitOptions { ValidationLevel = ValidationLevel.None });

        await Assert.That(result.Success).IsTrue();

        var code = result.Code!;
        var regionStart = code.IndexOf("#region Data", StringComparison.Ordinal);
        var endRegion = code.IndexOf("#endregion", regionStart, StringComparison.Ordinal);
        var namePos = code.IndexOf("Name", regionStart, StringComparison.Ordinal);
        var agePos = code.IndexOf("Age", regionStart, StringComparison.Ordinal);

        // Both properties should be between #region and #endregion
        await Assert.That(namePos).IsGreaterThan(regionStart);
        await Assert.That(agePos).IsGreaterThan(regionStart);
        await Assert.That(namePos).IsLessThan(endRegion);
        await Assert.That(agePos).IsLessThan(endRegion);
    }

    [Test]
    public async Task InRegion_creates_separate_regions_for_different_tags()
    {
        var result = TypeBuilder
            .Class("Customer")
            .WithAccessibility(Accessibility.Public)
            .AddProperty("Name", "string", p => p
                .WithAutoPropertyAccessors()
                .InRegion("Identity"))
            .AddProperty("Age", "int", p => p
                .WithAutoPropertyAccessors()
                .InRegion("Demographics"))
            .Emit(new EmitOptions { ValidationLevel = ValidationLevel.None });

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("#region Identity");
        await Assert.That(result.Code).Contains("#region Demographics");
    }

    [Test]
    public async Task InRegion_on_method()
    {
        var result = TypeBuilder
            .Class("Service")
            .WithAccessibility(Accessibility.Public)
            .AddMethod("Execute", m => m
                .WithExpressionBody("42")
                .InRegion("Actions"))
            .Emit(new EmitOptions { ValidationLevel = ValidationLevel.None });

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("#region Actions");
    }

    [Test]
    public async Task InRegion_on_field()
    {
        var result = TypeBuilder
            .Class("Service")
            .WithAccessibility(Accessibility.Public)
            .AddField("_count", "int", f => f.InRegion("Backing Fields"))
            .Emit(new EmitOptions { ValidationLevel = ValidationLevel.None });

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("#region Backing Fields");
    }

    #endregion

    #region Lambda Grouping (AddRegion)

    [Test]
    public async Task AddRegion_groups_members()
    {
        var result = TypeBuilder
            .Class("Customer")
            .WithAccessibility(Accessibility.Public)
            .AddRegion("Data Properties", r => r
                .AddProperty("Name", "string", p => p.WithAutoPropertyAccessors())
                .AddProperty("Age", "int", p => p.WithAutoPropertyAccessors()))
            .Emit(new EmitOptions { ValidationLevel = ValidationLevel.None });

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("#region Data Properties");
    }

    [Test]
    public async Task AddRegion_with_mixed_member_types()
    {
        var result = TypeBuilder
            .Class("Service")
            .WithAccessibility(Accessibility.Public)
            .AddRegion("Setup", r => r
                .AddField("_initialized", "bool", f => f)
                .AddConstructor(c => c
                    .WithBody(b => b.AddStatement("_initialized = true;"))))
            .Emit(new EmitOptions { ValidationLevel = ValidationLevel.None });

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("#region Setup");
    }

    #endregion

    #region Mixed Scenarios

    [Test]
    public async Task Auto_regions_with_explicit_tags_uses_explicit_over_auto()
    {
        var result = TypeBuilder
            .Class("Customer")
            .WithAccessibility(Accessibility.Public)
            .AddField("_name", "string", f => f)
            .AddProperty("Name", "string", p => p
                .WithAutoPropertyAccessors()
                .InRegion("Custom Region"))
            .AddMethod("GetName", m => m.WithExpressionBody("_name"))
            .Emit(new EmitOptions { UseRegions = true, ValidationLevel = ValidationLevel.None });

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("#region Fields");
        await Assert.That(result.Code).Contains("#region Custom Region");
        await Assert.That(result.Code).Contains("#region Methods");
    }

    [Test]
    public async Task Multiple_auto_region_categories_with_constructors()
    {
        var result = TypeBuilder
            .Class("Service")
            .WithAccessibility(Accessibility.Public)
            .AddField("_repo", "IRepository", f => f)
            .AddConstructor(c => c
                .AddParameter("repo", "IRepository")
                .WithBody(b => b.AddStatement("_repo = repo;")))
            .AddProperty("Name", "string", p => p.WithAutoPropertyAccessors())
            .AddMethod("Run", m => m.WithExpressionBody("42"))
            .Emit(new EmitOptions { UseRegions = true, ValidationLevel = ValidationLevel.None });

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("#region Fields");
        await Assert.That(result.Code).Contains("#region Constructors");
        await Assert.That(result.Code).Contains("#region Properties");
        await Assert.That(result.Code).Contains("#region Methods");
    }

    #endregion
}
