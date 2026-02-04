// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.Roslyn.Tests.Emit;

public class CompleteExampleTests : RoslynTestBase
{
    [Test]
    public async Task Can_emit_complete_service_class()
    {
        var repositoryField = FieldBuilder
            .For("_repository", "ICustomerRepository")
            .WithAccessibility(Accessibility.Private)
            .AsReadonly();

        var constructor = ConstructorBuilder
            .For("CustomerService")
            .WithAccessibility(Accessibility.Public)
            .AddParameter("repository", "ICustomerRepository")
            .WithBody(b => b.AddStatement("_repository = repository"));

        var getByIdMethod = MethodBuilder
            .For("GetByIdAsync")
            .Async()
            .WithAccessibility(Accessibility.Public)
            .WithReturnType("Task<Customer>")
            .AddParameter("id", "Guid")
            .WithBody(b => b.AddStatements("""
                if (id == Guid.Empty)
                    throw new ArgumentException("Invalid ID");
                
                var customer = await _repository.FindAsync(id);
                return customer;
                """));

        var result = TypeBuilder
            .Class("CustomerService")
            .InNamespace("MyApp.Services")
            .WithAccessibility(Accessibility.Public)
            .AddUsing("System")
            .AddUsing("System.Threading.Tasks")
            .AddField(repositoryField)
            .AddConstructor(constructor)
            .AddMethod(getByIdMethod)
            .Emit();

        // Validate success
        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.IsValid(out var validEmit)).IsTrue();

        // Validate structure
        await Assert.That(validEmit.Code).Contains("namespace MyApp.Services");
        await Assert.That(validEmit.Code).Contains("public class CustomerService");
        await Assert.That(validEmit.Code).Contains("private readonly ICustomerRepository _repository");
        await Assert.That(validEmit.Code).Contains("public CustomerService(ICustomerRepository repository)");
        await Assert.That(validEmit.Code).Contains("public async Task<Customer> GetByIdAsync(Guid id)");
        
        // Note: We don't validate compilation here because Customer and ICustomerRepository
        // are not defined - this test validates structure generation, not semantic correctness
    }
    
    [Test]
    public async Task Can_emit_complete_entity_that_compiles()
    {
        var result = TypeBuilder
            .Class("Customer")
            .InNamespace("MyApp.Domain")
            .AddUsing("System")
            .AddProperty("Id", "Guid", prop => prop
                .WithAccessibility(Accessibility.Public)
                .WithAutoPropertyAccessors())
            .AddProperty("Name", "string", prop => prop
                .WithAccessibility(Accessibility.Public)
                .WithAutoPropertyAccessors())
            .AddProperty("Email", "string", prop => prop
                .WithAccessibility(Accessibility.Public)
                .WithAutoPropertyAccessors())
            .AddMethod("Validate", method => method
                .WithAccessibility(Accessibility.Public)
                .WithReturnType("bool")
                .WithBody(b => b.AddStatements("""
                    if (string.IsNullOrEmpty(Name))
                        return false;
                    if (string.IsNullOrEmpty(Email))
                        return false;
                    return true;
                    """)))
            .Emit();

        await Assert.That(result.Success).IsTrue();

        // Verify structure
        await Assert.That(result.Code).Contains("public Guid Id { get; set; }");
        await Assert.That(result.Code).Contains("public string Name { get; set; }");
        await Assert.That(result.Code).Contains("public string Email { get; set; }");
        await Assert.That(result.Code).Contains("public bool Validate()");

        // Verify it actually compiles!
        var compilation = CompilationFor(result.Code!);
        var errors = compilation.GetDiagnostics()
            .Where(d => d.Severity == DiagnosticSeverity.Error);

        await Assert.That(errors).IsEmpty();
    }
}
