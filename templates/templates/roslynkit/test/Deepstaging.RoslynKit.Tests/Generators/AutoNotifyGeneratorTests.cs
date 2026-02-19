// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.Testing;
using Deepstaging.RoslynKit.Generators;

namespace Deepstaging.RoslynKit.Tests.Generators;

public class AutoNotifyGeneratorTests : RoslynTestBase
{
    private const string SingleField = """
                                       using Deepstaging.RoslynKit;

                                       namespace TestApp;

                                       [AutoNotify]
                                       public partial class Person
                                       {  
                                           [AlsoNotify("FullName")]
                                           private string _name;
                                       }
                                       """;

    [Test]
    public async Task Generates_property_for_private_field() =>
        await GenerateWith<AutoNotifyGenerator>(SingleField)
            .ShouldGenerate()
            .WithFileContaining("public string Name")
            .CompilesSuccessfully();

    [Test]
    public async Task Generates_INotifyPropertyChanged_implementation() =>
        await GenerateWith<AutoNotifyGenerator>(SingleField)
            .ShouldGenerate()
#if (includeRuntime)
            .WithFileContaining("ObservableObject")
            .WithFileContaining("SetField");
#else
            .WithFileContaining("INotifyPropertyChanged")
            .WithFileContaining("PropertyChanged")
            .WithFileContaining("OnPropertyChanged")
            .WithFileContaining("SetProperty");
#endif

    [Test]
    public async Task Generates_multiple_properties()
    {
        const string source = """
                              using Deepstaging.RoslynKit;

                              namespace TestApp;

                              [AutoNotify]
                              public partial class Person
                              {
                                  private string _name;
                                  private int _age;
                              }
                              """;

        await GenerateWith<AutoNotifyGenerator>(source)
            .ShouldGenerate()
            .WithFileContaining("public string Name")
            .WithFileContaining("public int Age")
            .CompilesSuccessfully();
    }

    [Test]
    public async Task Includes_AlsoNotify_calls()
    {
        const string source = """
                              using Deepstaging.RoslynKit;

                              namespace TestApp;

                              [AutoNotify]
                              public partial class Person
                              {
                                  private string _firstName;

                                  [AlsoNotify("FullName")]
                                  private string _lastName;
                              }
                              """;

        await GenerateWith<AutoNotifyGenerator>(source)
            .ShouldGenerate()
            .WithFileContaining("""OnPropertyChanged("FullName")""")
            .CompilesSuccessfully();
    }

    [Test]
    public async Task Skips_class_without_attribute()
    {
        const string source = """
                              namespace TestApp;

                              public partial class Person
                              {
                                  private string _name;
                              }
                              """;

        await GenerateWith<AutoNotifyGenerator>(source)
            .ShouldNotGenerate();
    }

    [Test]
    public async Task Skips_non_underscore_fields()
    {
        const string source = """
                              using Deepstaging.RoslynKit;

                              namespace TestApp;

                              [AutoNotify]
                              public partial class Person
                              {
                                  private string name;
                              }
                              """;

        await GenerateWith<AutoNotifyGenerator>(source)
            .ShouldGenerate()
            .WithoutFileContaining("public string Name");
    }

    [Test]
#if (includeRuntime)
    public async Task SnapshotRuntime() =>
#else
    public async Task Snapshot() =>
#endif
        await GenerateWith<AutoNotifyGenerator>(SingleField)
            .ShouldGenerate()
            .VerifySnapshot();
}