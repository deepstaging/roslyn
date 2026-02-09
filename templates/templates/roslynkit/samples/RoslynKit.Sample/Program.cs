// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using RoslynKit.Sample;

Console.WriteLine("=== [GenerateWith] Demo ===");
Console.WriteLine();

// Create an initial person
var person = new Person
{
    Name = "Alice",
    Age = 30,
    Email = "alice@example.com"
};

Console.WriteLine($"Original: {person.Name}, {person.Age}, {person.Email}");

// Use generated With* methods to create modified copies
var olderPerson = person.WithAge(31);
Console.WriteLine($"After birthday: {olderPerson.Name}, {olderPerson.Age}, {olderPerson.Email}");

var renamedPerson = person.WithName("Alice Smith");
Console.WriteLine($"After marriage: {renamedPerson.Name}, {renamedPerson.Age}, {renamedPerson.Email}");

// Chain With* calls for multiple changes
var movedPerson = person
    .WithName("Alice Johnson")
    .WithEmail("alice.johnson@newcompany.com");
Console.WriteLine($"After job change: {movedPerson.Name}, {movedPerson.Age}, {movedPerson.Email}");

// Works with structs too!
var address = new Address
{
    Street = "123 Main St",
    City = "Seattle",
    PostalCode = "98101"
};

Console.WriteLine($"\nOriginal address: {address.Street}, {address.City} {address.PostalCode}");

var newAddress = address.WithCity("Portland").WithPostalCode("97201");
Console.WriteLine($"After move: {newAddress.Street}, {newAddress.City} {newAddress.PostalCode}");

Console.WriteLine();
Console.WriteLine("=== [AutoNotify] Demo ===");
Console.WriteLine();

// Create a ViewModel with INotifyPropertyChanged
var vm = new PersonViewModel();

// Subscribe to PropertyChanged
vm.PropertyChanged += (sender, e) =>
{
    Console.WriteLine($"  PropertyChanged: {e.PropertyName}");
};

Console.WriteLine("Setting FirstName to 'John':");
vm.FirstName = "John";

Console.WriteLine("\nSetting LastName to 'Doe':");
vm.LastName = "Doe";

Console.WriteLine($"\nFullName is: {vm.FullName}");

Console.WriteLine("\nSetting Age to 25:");
vm.Age = 25;

Console.WriteLine();
Console.WriteLine("=== Settings ViewModel Demo ===");
Console.WriteLine();

var settings = new SettingsViewModel();
settings.PropertyChanged += (sender, e) =>
{
    Console.WriteLine($"  PropertyChanged: {e.PropertyName}");
};

Console.WriteLine($"Initial IsValid: {settings.IsValid}");

Console.WriteLine("\nSetting ServerUrl to 'localhost':");
settings.ServerUrl = "localhost";
Console.WriteLine($"IsValid: {settings.IsValid}");

Console.WriteLine("\nSetting Port to 443:");
settings.Port = 443;
Console.WriteLine($"IsValid: {settings.IsValid}");
