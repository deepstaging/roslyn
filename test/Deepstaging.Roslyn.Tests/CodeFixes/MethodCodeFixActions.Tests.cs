// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.CodeFixes;

public class MethodCodeFixActionsTests : RoslynTestBase
{
    private const string Id = NeedsMethodFixAnalyzer.DiagnosticId;

    private const string ClassWithMethod = """
                                           using Deepstaging.Roslyn.Tests.CodeFixes;

                                           namespace TestApp;

                                           public class Foo
                                           {
                                               [NeedsMethodFix]
                                               public void DoWork() { }
                                           }
                                           """;

    #region Modifier Tests

    [Test]
    public async Task AddPartial_AddsPartialModifier() =>
        await AnalyzeAndFixWith<NeedsMethodFixAnalyzer, AddPartialToMethodFix>(ClassWithMethod)
            .ForDiagnostic(Id)
            .ShouldProduce(
                """
                using Deepstaging.Roslyn.Tests.CodeFixes;

                namespace TestApp;

                public class Foo
                {
                    [NeedsMethodFix]
                    public partial void DoWork() { }
                }
                """);

    [Test]
    public async Task AddAsync_AddsAsyncModifier() =>
        await AnalyzeAndFixWith<NeedsMethodFixAnalyzer, AddAsyncToMethodFix>(ClassWithMethod)
            .ForDiagnostic(Id)
            .ShouldProduce(
                """
                using Deepstaging.Roslyn.Tests.CodeFixes;

                namespace TestApp;

                public class Foo
                {
                    [NeedsMethodFix]
                    public async void DoWork() { }
                }
                """);

    [Test]
    public async Task AddVirtual_AddsVirtualModifier() =>
        await AnalyzeAndFixWith<NeedsMethodFixAnalyzer, AddVirtualToMethodFix>(ClassWithMethod)
            .ForDiagnostic(Id)
            .ShouldProduce(
                """
                using Deepstaging.Roslyn.Tests.CodeFixes;

                namespace TestApp;

                public class Foo
                {
                    [NeedsMethodFix]
                    public virtual void DoWork() { }
                }
                """);

    [Test]
    public async Task AddOverride_AddsOverrideModifier() =>
        await AnalyzeAndFixWith<NeedsMethodFixAnalyzer, AddOverrideToMethodFix>(ClassWithMethod)
            .ForDiagnostic(Id)
            .ShouldProduce(
                """
                using Deepstaging.Roslyn.Tests.CodeFixes;

                namespace TestApp;

                public class Foo
                {
                    [NeedsMethodFix]
                    public override void DoWork() { }
                }
                """);

    [Test]
    public async Task AddStatic_AddsStaticModifier() =>
        await AnalyzeAndFixWith<NeedsMethodFixAnalyzer, AddStaticToMethodFix>(ClassWithMethod)
            .ForDiagnostic(Id)
            .ShouldProduce(
                """
                using Deepstaging.Roslyn.Tests.CodeFixes;

                namespace TestApp;

                public class Foo
                {
                    [NeedsMethodFix]
                    public static void DoWork() { }
                }
                """);

    [Test]
    public async Task RemoveModifier_RemovesStaticModifier() =>
        await AnalyzeAndFixWith<NeedsMethodFixAnalyzer, RemoveStaticFromMethodFix>(
                """
                using Deepstaging.Roslyn.Tests.CodeFixes;

                namespace TestApp;

                public class Foo
                {
                    [NeedsMethodFix]
                    public static void DoWork() { }
                }
                """)
            .ForDiagnostic(Id)
            .ShouldProduce(
                """
                using Deepstaging.Roslyn.Tests.CodeFixes;

                namespace TestApp;

                public class Foo
                {
                    [NeedsMethodFix]
                    public void DoWork() { }
                }
                """);

    [Test]
    public async Task AddAsync_OrderedAfterStatic() =>
        await AnalyzeAndFixWith<NeedsMethodFixAnalyzer, AddAsyncToMethodFix>(
                """
                using Deepstaging.Roslyn.Tests.CodeFixes;

                namespace TestApp;

                public class Foo
                {
                    [NeedsMethodFix]
                    public static void DoWork() { }
                }
                """)
            .ForDiagnostic(Id)
            .ShouldProduce(
                """
                using Deepstaging.Roslyn.Tests.CodeFixes;

                namespace TestApp;

                public class Foo
                {
                    [NeedsMethodFix]
                    public static async void DoWork() { }
                }
                """);

    #endregion

    #region Rename Tests

    [Test]
    public async Task RenameMethod_RenamesMethod() =>
        await AnalyzeAndFixWith<NeedsMethodFixAnalyzer, RenameMethodFix>(ClassWithMethod)
            .ForDiagnostic(Id)
            .ShouldProduce(
                """
                using Deepstaging.Roslyn.Tests.CodeFixes;

                namespace TestApp;

                public class Foo
                {
                    [NeedsMethodFix]
                    public void NewName() { }
                }
                """);

    [Test]
    public async Task AddAsyncSuffix_AppendsSuffix() =>
        await AnalyzeAndFixWith<NeedsMethodFixAnalyzer, AddAsyncSuffixFix>(ClassWithMethod)
            .ForDiagnostic(Id)
            .ShouldProduce(
                """
                using Deepstaging.Roslyn.Tests.CodeFixes;

                namespace TestApp;

                public class Foo
                {
                    [NeedsMethodFix]
                    public void DoWorkAsync() { }
                }
                """);

    [Test]
    public async Task RemoveAsyncSuffix_RemovesSuffix() =>
        await AnalyzeAndFixWith<NeedsMethodFixAnalyzer, RemoveAsyncSuffixFix>(
                """
                using Deepstaging.Roslyn.Tests.CodeFixes;

                namespace TestApp;

                public class Foo
                {
                    [NeedsMethodFix]
                    public void DoWorkAsync() { }
                }
                """)
            .ForDiagnostic(Id)
            .ShouldProduce(
                """
                using Deepstaging.Roslyn.Tests.CodeFixes;

                namespace TestApp;

                public class Foo
                {
                    [NeedsMethodFix]
                    public void DoWork() { }
                }
                """);

    #endregion

    #region Return Type Tests

    [Test]
    public async Task ChangeReturnType_ChangesReturnType() =>
        await AnalyzeAndFixWith<NeedsMethodFixAnalyzer, ChangeReturnTypeFix>(ClassWithMethod)
            .ForDiagnostic(Id)
            .ShouldProduce(
                """
                using Deepstaging.Roslyn.Tests.CodeFixes;

                namespace TestApp;

                public class Foo
                {
                    [NeedsMethodFix]
                    public string DoWork() { }
                }
                """);

    [Test]
    public async Task WrapReturnTypeInTask_VoidBecomesTask() =>
        await AnalyzeAndFixWith<NeedsMethodFixAnalyzer, WrapReturnTypeInTaskFix>(ClassWithMethod)
            .ForDiagnostic(Id)
            .ShouldProduce(
                """
                using Deepstaging.Roslyn.Tests.CodeFixes;

                namespace TestApp;

                public class Foo
                {
                    [NeedsMethodFix]
                    public Task DoWork() { }
                }
                """);

    [Test]
    public async Task WrapReturnTypeInTask_IntBecomesTaskOfInt() =>
        await AnalyzeAndFixWith<NeedsMethodFixAnalyzer, WrapReturnTypeInTaskFix>(
                """
                using Deepstaging.Roslyn.Tests.CodeFixes;

                namespace TestApp;

                public class Foo
                {
                    [NeedsMethodFix]
                    public int DoWork() { return 0; }
                }
                """)
            .ForDiagnostic(Id)
            .ShouldProduce(
                """
                using Deepstaging.Roslyn.Tests.CodeFixes;

                namespace TestApp;

                public class Foo
                {
                    [NeedsMethodFix]
                    public Task<int> DoWork() { return 0; }
                }
                """);

    #endregion
}