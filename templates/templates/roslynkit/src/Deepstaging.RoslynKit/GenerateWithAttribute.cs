// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.RoslynKit;

/// <summary>
/// Marks a class for source generation of immutable With*() methods.
/// The class must be declared as partial.
/// </summary>
/// <example>
/// <code>
/// [GenerateWith]
/// public partial class Person
/// {
///     public string Name { get; init; }
///     public int Age { get; init; }
/// }
/// 
/// // Generated:
/// // public Person WithName(string name) => new Person { Name = name, Age = this.Age };
/// // public Person WithAge(int age) => new Person { Name = this.Name, Age = age };
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class GenerateWithAttribute : Attribute
{
}