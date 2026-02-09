// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.RoslynKit;

namespace RoslynKit.Sample;

/// <summary>
/// A sample Person class demonstrating the [GenerateWith] attribute.
/// This generates immutable With*() methods for each property.
/// </summary>
[GenerateWith]
public partial class Person
{
    /// <summary>
    /// The person's name.
    /// </summary>
    public string Name { get; init; } = "";
    
    /// <summary>
    /// The person's age.
    /// </summary>
    public int Age { get; init; }
    
    /// <summary>
    /// The person's email address.
    /// </summary>
    public string? Email { get; init; }
}

/// <summary>
/// A sample Address struct demonstrating [GenerateWith] with structs.
/// </summary>
[GenerateWith]
public partial struct Address
{
    /// <summary>
    /// The street address.
    /// </summary>
    public string Street { get; init; }
    
    /// <summary>
    /// The city.
    /// </summary>
    public string City { get; init; }
    
    /// <summary>
    /// The postal/ZIP code.
    /// </summary>
    public string PostalCode { get; init; }
}
