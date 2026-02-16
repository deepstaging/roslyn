# EntityFrameworkRefs

Entity Framework Core types, data annotation attributes, and well-known API calls.

> **See also:** [Refs Overview](index.md) | [TypeRef & Primitives](../type-ref.md)

---

## Core Types

| Member | Returns | Produces |
|--------|---------|----------|
| `DbContext` | `TypeRef` | `DbContext` |
| `DbSet(entity)` | `TypeRef` | `DbSet<T>` |
| `ModelBuilder` | `TypeRef` | `ModelBuilder` |
| `EntityTypeBuilder(entity)` | `TypeRef` | `EntityTypeBuilder<T>` |
| `IEntityTypeConfiguration(entity)` | `TypeRef` | `IEntityTypeConfiguration<T>` |

## Attributes

All attribute members return `AttributeRef` — they work directly with `.WithAttribute()` on any builder and bridge to `AttributeBuilder` for configuring arguments.

### Data Annotations

| Member | Returns | Produces |
|--------|---------|----------|
| `Key` | `AttributeRef` | `[Key]` |
| `Required` | `AttributeRef` | `[Required]` |
| `MaxLength` | `AttributeRef` | `[MaxLength]` |
| `StringLength` | `AttributeRef` | `[StringLength]` |
| `Range` | `AttributeRef` | `[Range]` |

### Schema

| Member | Returns | Produces |
|--------|---------|----------|
| `Table` | `AttributeRef` | `[Table]` |
| `Column` | `AttributeRef` | `[Column]` |
| `ForeignKey` | `AttributeRef` | `[ForeignKey]` |
| `NotMapped` | `AttributeRef` | `[NotMapped]` |
| `DatabaseGenerated` | `AttributeRef` | `[DatabaseGenerated]` |

## API Call Helpers

| Method | Returns | Produces |
|--------|---------|----------|
| `Set(context, entity)` | `ExpressionRef` | `context.Set<T>()` |
| `SaveChangesAsync(context, ct)` | `ExpressionRef` | `context.SaveChangesAsync(ct)` |
| `SaveChangesAsync(context)` | `ExpressionRef` | `context.SaveChangesAsync()` |
| `FindAsync(dbSet, keys...)` | `ExpressionRef` | `dbSet.FindAsync(keys...)` |
| `AddAsync(dbSet, entity)` | `ExpressionRef` | `dbSet.AddAsync(entity)` |
| `Remove(dbSet, entity)` | `ExpressionRef` | `dbSet.Remove(entity)` |

---

## Examples

### Attributes — Simple

```csharp
// [Key] — no arguments needed
property.WithAttribute(EntityFrameworkRefs.Key)

// [Required]
property.WithAttribute(EntityFrameworkRefs.Required)

// [NotMapped]
property.WithAttribute(EntityFrameworkRefs.NotMapped)
```

### Attributes — With Arguments

`AttributeRef` bridges to `AttributeBuilder` via `.WithArgument()`:

```csharp
// [MaxLength(100)]
property.WithAttribute(EntityFrameworkRefs.MaxLength.WithArgument("100"))

// [StringLength(50, MinimumLength = 1)]
property.WithAttribute(EntityFrameworkRefs.StringLength
    .WithArgument("50")
    .WithNamedArgument("MinimumLength", "1"))

// [Range(0, 100)]
property.WithAttribute(EntityFrameworkRefs.Range.WithArguments("0", "100"))

// [Table("Orders")]
type.WithAttribute(EntityFrameworkRefs.Table.WithArgument("\"Orders\""))

// [Column("order_date", TypeName = "date")]
property.WithAttribute(EntityFrameworkRefs.Column
    .WithArgument("\"order_date\"")
    .WithNamedArgument("TypeName", "\"date\""))

// [ForeignKey("CustomerId")]
property.WithAttribute(EntityFrameworkRefs.ForeignKey.WithArgument("\"CustomerId\""))

// [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
property.WithAttribute(EntityFrameworkRefs.DatabaseGenerated
    .WithArgument("DatabaseGeneratedOption.Identity"))
```

### Attributes — With Configure Callback

Attributes also work with the builder's callback pattern via implicit `string` conversion:

```csharp
property.WithAttribute(EntityFrameworkRefs.Column, a => a
    .WithArgument("\"order_date\"")
    .WithNamedArgument("TypeName", "\"date\""))
```

### Entity Type Building

```csharp
// DbContext with DbSet property
builder
    .Extends(EntityFrameworkRefs.DbContext)
    .AddProperty("Orders", EntityFrameworkRefs.DbSet("Order"))
    .AddProperty("Customers", EntityFrameworkRefs.DbSet("Customer"))

// IEntityTypeConfiguration implementation
TypeBuilder.Class("OrderConfiguration")
    .Implements(EntityFrameworkRefs.IEntityTypeConfiguration("Order"))
    .AddMethod("Configure", m => m
        .AddParameter("builder", EntityFrameworkRefs.EntityTypeBuilder("Order"))
        .WithBody(b => b
            .AddStatement("builder.HasKey(e => e.Id);")))
```

### CRUD Operations

```csharp
// context.Set<Order>()
body.AddStatement($"var orders = {EntityFrameworkRefs.Set("_context", "Order")};")
// → var orders = _context.Set<Order>();

// dbSet.FindAsync(id)
body.AddReturn(EntityFrameworkRefs.FindAsync("_db.Orders", "id").Await())
// → return await _db.Orders.FindAsync(id)

// dbSet.AddAsync(entity)
body.AddStatement($"await {EntityFrameworkRefs.AddAsync("_db.Orders", "order")};")
// → await _db.Orders.AddAsync(order);

// context.SaveChangesAsync(ct)
body.AddStatement(
    $"await {EntityFrameworkRefs.SaveChangesAsync("_context", "cancellationToken")};")
// → await _context.SaveChangesAsync(cancellationToken);

// dbSet.Remove(entity)
body.AddStatement($"{EntityFrameworkRefs.Remove("_db.Orders", "order")};")
// → _db.Orders.Remove(order);
```
