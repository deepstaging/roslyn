// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5


namespace Deepstaging.Roslyn.Tests;

public class FieldQueryTests : RoslynTestBase
{
    [Test]
    public async Task Can_filter_public_fields()
    {
        var code = """
            public class TestClass
            {
                public int PublicField;
                private int PrivateField;
            }
            """;

        var fields = SymbolsFor(code)
            .RequireNamedType("TestClass")
            .QueryFields()
            .ThatArePublic()
            .GetAll();

        await Assert.That(fields.Any(f => f.Value.Name == "PublicField")).IsTrue();
    }

    [Test]
    public async Task Can_filter_const_fields()
    {
        var code = """
            public class TestClass
            {
                public const int ConstField = 42;
                public readonly int ReadonlyField = 42;
            }
            """;

        var constFields = SymbolsFor(code)
            .RequireNamedType("TestClass")
            .QueryFields()
            .ThatAreConst()
            .GetAll();

        await Assert.That(constFields.Any(f => f.Value.Name == "ConstField")).IsTrue();
    }
}
