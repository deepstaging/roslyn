// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.CodeFixes;

public class TypeCodeFixActionsTests : RoslynTestBase
{
    private const string Id = NeedsTypeFixAnalyzer.DiagnosticId;

    #region Modifier Tests

    [Test]
    public async Task AddPartial_AddsPartialModifier() =>
        await AnalyzeAndFixWith<NeedsTypeFixAnalyzer, AddPartialToClassFix>(
                """
                using Deepstaging.Roslyn.Tests.CodeFixes;

                namespace TestApp;

                [NeedsTypeFix]
                public class Foo { }
                """)
            .ForDiagnostic(Id)
            .ShouldProduce(
                """
                using Deepstaging.Roslyn.Tests.CodeFixes;

                namespace TestApp;

                [NeedsTypeFix]
                public partial class Foo { }
                """);

    [Test]
    public async Task AddSealed_AddsSealedModifier() =>
        await AnalyzeAndFixWith<NeedsTypeFixAnalyzer, AddSealedToClassFix>(
                """
                using Deepstaging.Roslyn.Tests.CodeFixes;

                namespace TestApp;

                [NeedsTypeFix]
                public class Foo { }
                """)
            .ForDiagnostic(Id)
            .ShouldProduce(
                """
                using Deepstaging.Roslyn.Tests.CodeFixes;

                namespace TestApp;

                [NeedsTypeFix]
                public sealed class Foo { }
                """);

    [Test]
    public async Task AddStatic_AddsStaticModifier() =>
        await AnalyzeAndFixWith<NeedsTypeFixAnalyzer, AddStaticToClassFix>(
                """
                using Deepstaging.Roslyn.Tests.CodeFixes;

                namespace TestApp;

                [NeedsTypeFix]
                public class Foo { }
                """)
            .ForDiagnostic(Id)
            .ShouldProduce(
                """
                using Deepstaging.Roslyn.Tests.CodeFixes;

                namespace TestApp;

                [NeedsTypeFix]
                public static class Foo { }
                """);

    [Test]
    public async Task AddAbstract_AddsAbstractModifier() =>
        await AnalyzeAndFixWith<NeedsTypeFixAnalyzer, AddAbstractToClassFix>(
                """
                using Deepstaging.Roslyn.Tests.CodeFixes;

                namespace TestApp;

                [NeedsTypeFix]
                public class Foo { }
                """)
            .ForDiagnostic(Id)
            .ShouldProduce(
                """
                using Deepstaging.Roslyn.Tests.CodeFixes;

                namespace TestApp;

                [NeedsTypeFix]
                public abstract class Foo { }
                """);

    [Test]
    public async Task AddReadonly_AddsReadonlyToStruct() =>
        await AnalyzeAndFixWith<NeedsTypeFixAnalyzer, AddReadonlyToStructFix>(
                """
                using Deepstaging.Roslyn.Tests.CodeFixes;

                namespace TestApp;

                [NeedsTypeFix]
                public struct Foo { }
                """)
            .ForDiagnostic(Id)
            .ShouldProduce(
                """
                using Deepstaging.Roslyn.Tests.CodeFixes;

                namespace TestApp;

                [NeedsTypeFix]
                public readonly struct Foo { }
                """);

    [Test]
    public async Task RemoveModifier_RemovesSealedModifier() =>
        await AnalyzeAndFixWith<NeedsTypeFixAnalyzer, RemoveSealedFromClassFix>(
                """
                using Deepstaging.Roslyn.Tests.CodeFixes;

                namespace TestApp;

                [NeedsTypeFix]
                public sealed class Foo { }
                """)
            .ForDiagnostic(Id)
            .ShouldProduce(
                """
                using Deepstaging.Roslyn.Tests.CodeFixes;

                namespace TestApp;

                [NeedsTypeFix]
                public class Foo { }
                """);

    #endregion

    #region Member Insertion Tests

    [Test]
    public async Task AddMembers_AddsMemberToClass() =>
        await AnalyzeAndFixWith<NeedsTypeFixAnalyzer, AddMembersToClassFix>(
                """
                using Deepstaging.Roslyn.Tests.CodeFixes;

                namespace TestApp;

                [NeedsTypeFix]
                public class Foo { }
                """)
            .ForDiagnostic(Id)
            .ShouldOfferFix("Add DoWork method");

    [Test]
    public async Task AddMembersFromSource_AddsParsedMember() =>
        await AnalyzeAndFixWith<NeedsTypeFixAnalyzer, AddMembersFromSourceToClassFix>(
                """
                using Deepstaging.Roslyn.Tests.CodeFixes;

                namespace TestApp;

                [NeedsTypeFix]
                public class Foo { }
                """)
            .ForDiagnostic(Id)
            .ShouldOfferFix("Add members from source");

    #endregion

    #region Rename Tests

    [Test]
    public async Task RenameType_RenamesClass() =>
        await AnalyzeAndFixWith<NeedsTypeFixAnalyzer, RenameClassFix>(
                """
                using Deepstaging.Roslyn.Tests.CodeFixes;

                namespace TestApp;

                [NeedsTypeFix]
                public class Foo { }
                """)
            .ForDiagnostic(Id)
            .ShouldProduce(
                """
                using Deepstaging.Roslyn.Tests.CodeFixes;

                namespace TestApp;

                [NeedsTypeFix]
                public class Renamed { }
                """);

    #endregion

    #region Base Type Tests

    [Test]
    public async Task AddBaseType_AddsBaseClass() =>
        await AnalyzeAndFixWith<NeedsTypeFixAnalyzer, AddBaseTypeToClassFix>(
                """
                using Deepstaging.Roslyn.Tests.CodeFixes;

                namespace TestApp;

                [NeedsTypeFix]
                public class Foo { }
                """)
            .ForDiagnostic(Id)
            .ShouldOfferFix("Add 'BaseClass'");

    [Test]
    public async Task AddInterface_AddsInterface() =>
        await AnalyzeAndFixWith<NeedsTypeFixAnalyzer, AddInterfaceToClassFix>(
                """
                using Deepstaging.Roslyn.Tests.CodeFixes;

                namespace TestApp;

                [NeedsTypeFix]
                public class Foo { }
                """)
            .ForDiagnostic(Id)
            .ShouldOfferFix("Implement 'IDisposable'");

    #endregion
}