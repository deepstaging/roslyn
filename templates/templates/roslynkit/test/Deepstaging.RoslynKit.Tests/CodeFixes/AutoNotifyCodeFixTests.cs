// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Testing;
using Deepstaging.RoslynKit.Analyzers;
using Deepstaging.RoslynKit.CodeFixes;

namespace Deepstaging.RoslynKit.Tests.CodeFixes;

public class AutoNotifyCodeFixTests : RoslynTestBase
{
    [Test]
    public async Task MakePartial_adds_partial_modifier() =>
        await AnalyzeAndFixWith<MustBePartialAnalyzer, MakePartialCodeFix>(
                """
                using Deepstaging.RoslynKit;

                namespace TestApp;

                [AutoNotify]
                public class Person
                {
                    private string _name;
                }
                """)
            .ForDiagnostic("RK001")
            .ShouldProduce(
                """
                using Deepstaging.RoslynKit;

                namespace TestApp;

                [AutoNotify]
                public partial class Person
                {
                    private string _name;
                }
                """);

    [Test]
    public async Task MakePrivate_changes_field_to_private() =>
        await AnalyzeAndFixWith<FieldMustBePrivateAnalyzer, MakePrivateCodeFix>(
                """
                using Deepstaging.RoslynKit;

                namespace TestApp;

                [AutoNotify]
                public partial class Person
                {
                    public string _name;
                }
                """)
            .ForDiagnostic("RK002")
            .ShouldOfferFix("Make field private");
}