// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Testing;
using Deepstaging.RoslynKit.Generators;

namespace Deepstaging.RoslynKit.Tests;

/// <summary>
/// Tests for the WithMethodsGenerator source generator.
/// </summary>
public class WithMethodsGeneratorTests : RoslynTestBase
{
    [Test]
    public async Task GeneratesWithMethods_ForClassWithInitProperties()
    {
        const string source = """
                              using Deepstaging.RoslynKit;

                              namespace TestApp;

                              [GenerateWith]
                              public partial class Person
                              {
                                  public string Name { get; init; }
                                  public int Age { get; init; }
                              }
                              """;

        await GenerateWith<WithMethodsGenerator>(source)
            .ShouldGenerate()
            .WithFileCount(1)
            .WithFileContaining("WithName")
            .WithFileContaining("WithAge")
            .WithNoDiagnostics()
            .VerifySnapshot();
    }

    [Test]
    public async Task GeneratesWithMethods_ForStruct()
    {
        const string source = """
                              using Deepstaging.RoslynKit;

                              namespace TestApp;

                              [GenerateWith]
                              public partial struct Point
                              {
                                  public int X { get; init; }
                                  public int Y { get; init; }
                              }
                              """;

        await GenerateWith<WithMethodsGenerator>(source)
            .ShouldGenerate()
            .WithFileCount(1)
            .WithFileContaining("partial struct Point")
            .WithFileContaining("WithX")
            .WithFileContaining("WithY")
            .WithNoDiagnostics();
    }

    [Test]
    public async Task SkipsStaticProperties()
    {
        const string source = """
                              using Deepstaging.RoslynKit;

                              namespace TestApp;

                              [GenerateWith]
                              public partial class Config
                              {
                                  public static string Default { get; set; }
                                  public string Value { get; init; }
                              }
                              """;

        await GenerateWith<WithMethodsGenerator>(source)
            .ShouldGenerate()
            .WithFileCount(1)
            .WithFileContaining("WithValue")
            .WithNoDiagnostics();
    }

    [Test]
    public async Task HandlesNullableTypes()
    {
        const string source = """
                              using Deepstaging.RoslynKit;

                              namespace TestApp;

                              [GenerateWith]
                              public partial class Contact
                              {
                                  public string Name { get; init; }
                                  public string? Email { get; init; }
                                  public int? PhoneExtension { get; init; }
                              }
                              """;

        await GenerateWith<WithMethodsGenerator>(source)
            .ShouldGenerate()
            .WithFileCount(1)
            .WithFileContaining("WithEmail(string? value)")
            .WithFileContaining("WithPhoneExtension(int? value)")
            .WithNoDiagnostics()
            .VerifySnapshot();
    }

    [Test]
    public async Task GeneratesNoOutput_ForTypeWithoutProperties()
    {
        const string source = """
                              using Deepstaging.RoslynKit;

                              namespace TestApp;

                              [GenerateWith]
                              public partial class Empty
                              {
                              }
                              """;

        await GenerateWith<WithMethodsGenerator>(source)
            .ShouldNotGenerate();
    }
}