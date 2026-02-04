// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Tests.Emit;

public class TypeBuilderTests : RoslynTestBase
{
    [Test]
    public async Task Can_emit_simple_class()
    {
        var result = TypeBuilder
            .Class("Customer")
            .InNamespace("MyApp.Domain")
            .WithAccessibility(Accessibility.Public)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.IsValid(out var validEmit)).IsTrue();
        await Assert.That(validEmit.Code).Contains("public class Customer");
        await Assert.That(validEmit.Code).Contains("namespace MyApp.Domain");
    }

    [Test]
    public async Task Can_emit_interface()
    {
        var result = TypeBuilder
            .Interface("IRepository")
            .InNamespace("MyApp.Core")
            .WithAccessibility(Accessibility.Public)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public interface IRepository");
    }

    [Test]
    public async Task Can_emit_struct()
    {
        var result = TypeBuilder
            .Struct("Point")
            .InNamespace("MyApp.Types")
            .WithAccessibility(Accessibility.Public)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public struct Point");
    }

    [Test]
    public async Task Emits_valid_compilable_code()
    {
        var result = TypeBuilder
            .Class("Customer")
            .InNamespace("MyApp.Domain")
            .AddUsing("System")
            .Emit();

        await Assert.That(result.Success).IsTrue();

        // Try to compile the generated code
        var compilation = CompilationFor(result.Code!);
        var diagnostics = compilation.GetDiagnostics()
            .Where(d => d.Severity == DiagnosticSeverity.Error);

        await Assert.That(diagnostics).IsEmpty();
    }

    [Test]
    public async Task Can_emit_class_with_attribute()
    {
        var result = TypeBuilder
            .Class("Customer")
            .InNamespace("MyApp.Domain")
            .AddUsing("System")
            .WithAttribute("Serializable")
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("[Serializable]");
        await Assert.That(result.Code).Contains("public class Customer");
    }

    [Test]
    public async Task Can_emit_class_with_attribute_with_arguments()
    {
        var result = TypeBuilder
            .Class("Customer")
            .InNamespace("MyApp.Domain")
            .AddUsing("System")
            .WithAttribute("Obsolete", attr => attr.WithArgument("\"Use CustomerV2 instead\""))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("[Obsolete(\"Use CustomerV2 instead\")]");
    }

    [Test]
    public async Task Can_emit_class_with_multiple_attributes()
    {
        var result = TypeBuilder
            .Class("Customer")
            .InNamespace("MyApp.Domain")
            .AddUsing("System")
            .WithAttribute("Serializable")
            .WithAttribute("Obsolete", attr => attr.WithArgument("\"Deprecated\""))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("[Serializable]");
        await Assert.That(result.Code).Contains("[Obsolete(\"Deprecated\")]");
    }

    [Test]
    public async Task Can_emit_property_with_attribute()
    {
        var result = TypeBuilder
            .Class("Customer")
            .InNamespace("MyApp.Domain")
            .AddProperty("Name", "string", p => p
                .WithAutoPropertyAccessors()
                .WithAttribute("Required"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("[Required]");
        await Assert.That(result.Code).Contains("public string Name");
    }

    [Test]
    public async Task Can_emit_method_with_attribute()
    {
        var result = TypeBuilder
            .Class("Customer")
            .InNamespace("MyApp.Domain")
            .AddMethod("GetName", m => m
                .WithReturnType("string")
                .WithAttribute("Obsolete")
                .WithExpressionBody("\"\""))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("[Obsolete]");
        await Assert.That(result.Code).Contains("public string GetName()");
    }

    #region Nested Types

    [Test]
    public async Task Can_emit_nested_class()
    {
        var result = TypeBuilder
            .Class("Outer")
            .InNamespace("MyApp.Domain")
            .AddNestedType("Inner", n => n
                .WithAccessibility(Accessibility.Private)
                .AddProperty("Value", "int", p => p.WithAutoPropertyAccessors()))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public class Outer");
        await Assert.That(result.Code).Contains("private class Inner");
        await Assert.That(result.Code).Contains("public int Value");
    }

    [Test]
    public async Task Can_emit_multiple_nested_classes()
    {
        var result = TypeBuilder
            .Class("Container")
            .InNamespace("MyApp")
            .AddNestedType("First", n => n.WithAccessibility(Accessibility.Public))
            .AddNestedType("Second", n => n.WithAccessibility(Accessibility.Internal))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public class First");
        await Assert.That(result.Code).Contains("internal class Second");
    }

    [Test]
    public async Task Can_emit_deeply_nested_classes()
    {
        var result = TypeBuilder
            .Class("Level1")
            .InNamespace("MyApp")
            .AddNestedType("Level2", n => n
                .AddNestedType("Level3", n2 => n2
                    .AddProperty("DeepValue", "string", p => p.WithAutoPropertyAccessors())))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public class Level1");
        await Assert.That(result.Code).Contains("public class Level2");
        await Assert.That(result.Code).Contains("public class Level3");
        await Assert.That(result.Code).Contains("public string DeepValue");
    }

    [Test]
    public async Task Can_emit_nested_struct()
    {
        var nestedStruct = TypeBuilder.Struct("InnerStruct")
            .WithAccessibility(Accessibility.Private)
            .AddField("_value", "int", f => f);

        var result = TypeBuilder
            .Class("Outer")
            .InNamespace("MyApp")
            .AddNestedType(nestedStruct)
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("private struct InnerStruct");
    }

    [Test]
    public async Task Can_emit_nested_interface()
    {
        var result = TypeBuilder
            .Class("Service")
            .InNamespace("MyApp")
            .AddNestedType("ICallback", n => TypeBuilder.Interface("ICallback")
                .WithAccessibility(Accessibility.Public)
                .AddMethod("OnComplete", m => m.WithReturnType("void")))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("public interface ICallback");
    }

    [Test]
    public async Task Nested_class_compiles()
    {
        var result = TypeBuilder
            .Class("Outer")
            .InNamespace("MyApp")
            .AddUsing("System")
            .AddNestedType("Inner", n => n
                .WithAccessibility(Accessibility.Private)
                .AddProperty("Name", "string", p => p.WithAutoPropertyAccessors()))
            .Emit();

        await Assert.That(result.Success).IsTrue();

        var compilation = CompilationFor(result.Code!);
        var diagnostics = compilation.GetDiagnostics()
            .Where(d => d.Severity == DiagnosticSeverity.Error);
        await Assert.That(diagnostics).IsEmpty();
    }

    [Test]
    public async Task Nested_type_usings_are_hoisted_to_compilation_unit()
    {
        var result = TypeBuilder
            .Class("Outer")
            .InNamespace("MyApp")
            .AddNestedType("Inner", n => n
                .AddUsing("System.Collections.Generic")
                .AddProperty("Items", "List<int>", p => p.WithAutoPropertyAccessors()))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("using System.Collections.Generic;");
        
        // Verify the using is at the top, not inside the class
        var code = result.Code!;
        var usingIndex = code.IndexOf("using System.Collections.Generic;");
        var classIndex = code.IndexOf("public class Outer");
        await Assert.That(usingIndex).IsLessThan(classIndex);
    }

    [Test]
    public async Task Deeply_nested_usings_are_hoisted()
    {
        var result = TypeBuilder
            .Class("Level1")
            .InNamespace("MyApp")
            .AddUsing("System")
            .AddNestedType("Level2", n => n
                .AddUsing("System.Text")
                .AddNestedType("Level3", n2 => n2
                    .AddUsing("System.IO")
                    .AddProperty("Stream", "MemoryStream", p => p.WithAutoPropertyAccessors())))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("using System;");
        await Assert.That(result.Code).Contains("using System.Text;");
        await Assert.That(result.Code).Contains("using System.IO;");
    }

    [Test]
    public async Task Collects_usings_from_method_builders()
    {
        var result = TypeBuilder
            .Class("Service")
            .InNamespace("MyApp")
            .AddMethod("Process", m => m
                .WithReturnType("Task")
                .AddUsing("System.Threading.Tasks"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("using System.Threading.Tasks;");
    }

    [Test]
    public async Task Collects_usings_from_property_builders()
    {
        var result = TypeBuilder
            .Class("Entity")
            .InNamespace("MyApp")
            .AddProperty("Items", "List<string>", p => p
                .WithAutoPropertyAccessors()
                .AddUsing("System.Collections.Generic"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("using System.Collections.Generic;");
    }

    [Test]
    public async Task Collects_usings_from_field_builders()
    {
        var result = TypeBuilder
            .Class("Repository")
            .InNamespace("MyApp")
            .AddField("_logger", "ILogger", f => f
                .AsReadonly()
                .AddUsing("Microsoft.Extensions.Logging"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("using Microsoft.Extensions.Logging;");
    }

    [Test]
    public async Task Collects_usings_from_constructor_builders()
    {
        var result = TypeBuilder
            .Class("Handler")
            .InNamespace("MyApp")
            .AddConstructor(c => c
                .AddParameter("options", "IOptions<Config>")
                .AddUsing("Microsoft.Extensions.Options"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("using Microsoft.Extensions.Options;");
    }

    [Test]
    public async Task Collects_usings_from_multiple_member_types()
    {
        var result = TypeBuilder
            .Class("ComplexService")
            .InNamespace("MyApp")
            .AddUsing("System")
            .AddField("_cache", "IMemoryCache", f => f
                .AsReadonly()
                .AddUsing("Microsoft.Extensions.Caching.Memory"))
            .AddProperty("Logger", "ILogger", p => p
                .WithAutoPropertyAccessors()
                .AddUsing("Microsoft.Extensions.Logging"))
            .AddMethod("ProcessAsync", m => m
                .WithReturnType("Task<Result>")
                .AddUsing("System.Threading.Tasks"))
            .AddConstructor(c => c
                .AddParameter("options", "IOptions<Config>")
                .AddUsing("Microsoft.Extensions.Options"))
            .Emit();

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Code).Contains("using Microsoft.Extensions.Caching.Memory;");
        await Assert.That(result.Code).Contains("using Microsoft.Extensions.Logging;");
        await Assert.That(result.Code).Contains("using Microsoft.Extensions.Options;");
        await Assert.That(result.Code).Contains("using System;");
        await Assert.That(result.Code).Contains("using System.Threading.Tasks;");
    }

    #endregion
}
