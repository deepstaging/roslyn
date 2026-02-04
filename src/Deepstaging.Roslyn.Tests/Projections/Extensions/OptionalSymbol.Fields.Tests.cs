// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Tests.Projections.Extensions;

public class OptionalSymbolFieldsTests : RoslynTestBase
{
    [Test]
    public async Task GetFieldType_returns_field_type()
    {
        var code = """
            public class TestClass
            {
                public int NumberField;
                public string TextField;
            }
            """;

        var fieldType = SymbolsFor(code)
            .GetNamedType("TestClass")
            .QueryFields()
            .WithName("NumberField")
            .FirstOrDefault()
            .ReturnType;

        await Assert.That(fieldType.HasValue).IsTrue();
        await Assert.That(fieldType.Name).IsEqualTo("Int32");
    }

    [Test]
    public async Task IsConstField_detects_const_fields()
    {
        var code = """
            public class TestClass
            {
                public const int ConstField = 42;
                public readonly int ReadOnlyField = 42;
                public int NormalField;
            }
            """;

        var fields = SymbolsFor(code)
            .GetNamedType("TestClass")
            .QueryFields()
            .GetAll();

        await Assert.That(fields.ByName["ConstField"].IsConst()).IsTrue();
        await Assert.That(fields.ByName["ReadOnlyField"].IsConst()).IsFalse();
        await Assert.That(fields.ByName["NormalField"].IsConst()).IsFalse();
    }

    [Test]
    public async Task IsReadOnlyField_detects_readonly_fields()
    {
        var code = """
            public class TestClass
            {
                public readonly int ReadOnlyField = 42;
                public int NormalField;
            }
            """;

        var context = SymbolsFor(code);

        var readonlyField = context.GetNamedType("TestClass").QueryFields().WithName("ReadOnlyField").FirstOrDefault();
        var normalField = context.GetNamedType("TestClass").QueryFields().WithName("NormalField").FirstOrDefault();

        await Assert.That(readonlyField.IsReadOnly).IsTrue();
        await Assert.That(normalField.IsReadOnly).IsFalse();
    }

    [Test]
    public async Task IsStaticField_detects_static_fields()
    {
        var code = """
            public class TestClass
            {
                public static int StaticField;
                public int InstanceField;
            }
            """;

        var context = SymbolsFor(code);

        var staticField = context.GetNamedType("TestClass").QueryFields().WithName("StaticField").FirstOrDefault();
        var instanceField = context.GetNamedType("TestClass").QueryFields().WithName("InstanceField").FirstOrDefault();

        await Assert.That(staticField.IsStatic).IsTrue();
        await Assert.That(instanceField.IsStatic).IsFalse();
    }
}
