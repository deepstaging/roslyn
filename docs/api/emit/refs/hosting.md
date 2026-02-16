# DependencyInjectionRefs, ConfigurationRefs & LoggingRefs

Microsoft.Extensions hosting infrastructure — DI, configuration, and logging.

> **See also:** [Refs Overview](index.md) | [TypeRef & Primitives](../type-ref.md)

---

## DependencyInjectionRefs

`Microsoft.Extensions.DependencyInjection` types and service registration helpers.

### Types

| Member | Returns | Produces |
|--------|---------|----------|
| `IServiceCollection` | `TypeRef` | `IServiceCollection` |
| `IServiceProvider` | `TypeRef` | `System.IServiceProvider` |
| `IServiceScopeFactory` | `TypeRef` | `IServiceScopeFactory` |
| `IServiceScope` | `TypeRef` | `IServiceScope` |
| `ServiceDescriptor` | `TypeRef` | `ServiceDescriptor` |

### API Call Helpers

Service registration (called on an `IServiceCollection` instance):

| Method | Returns | Produces |
|--------|---------|----------|
| `AddSingleton(services, service, impl)` | `ExpressionRef` | `services.AddSingleton<TService, TImpl>()` |
| `AddSingleton(services, service)` | `ExpressionRef` | `services.AddSingleton<TService>()` |
| `AddScoped(services, service, impl)` | `ExpressionRef` | `services.AddScoped<TService, TImpl>()` |
| `AddScoped(services, service)` | `ExpressionRef` | `services.AddScoped<TService>()` |
| `AddTransient(services, service, impl)` | `ExpressionRef` | `services.AddTransient<TService, TImpl>()` |
| `AddTransient(services, service)` | `ExpressionRef` | `services.AddTransient<TService>()` |

Service resolution (called on an `IServiceProvider` instance):

| Method | Returns | Produces |
|--------|---------|----------|
| `GetRequiredService(provider, service)` | `ExpressionRef` | `provider.GetRequiredService<T>()` |
| `GetService(provider, service)` | `ExpressionRef` | `provider.GetService<T>()` |

### Examples

```csharp
// Extension method parameter
method.AddParameter("services", DependencyInjectionRefs.IServiceCollection)

// Service registration
body.AddStatement(
    $"{DependencyInjectionRefs.AddSingleton("services", "IOrderService", "OrderService")};")
// → services.AddSingleton<IOrderService, OrderService>();

body.AddStatement(
    $"{DependencyInjectionRefs.AddScoped("services", "IDbContext")};")
// → services.AddScoped<IDbContext>();

// Service resolution
body.AddStatement(
    $"var logger = {DependencyInjectionRefs.GetRequiredService("provider", LoggingRefs.ILogger)};")
// → var logger = provider.GetRequiredService<global::...ILogger>();

// Return for chaining
body.AddReturn("services")
```

---

## ConfigurationRefs

`Microsoft.Extensions.Configuration` types and accessor helpers.

### Types

| Member | Returns | Produces |
|--------|---------|----------|
| `IConfiguration` | `TypeRef` | `IConfiguration` |
| `IConfigurationSection` | `TypeRef` | `IConfigurationSection` |
| `IConfigurationRoot` | `TypeRef` | `IConfigurationRoot` |
| `IConfigurationBuilder` | `TypeRef` | `IConfigurationBuilder` |

### API Call Helpers

| Method | Returns | Produces |
|--------|---------|----------|
| `GetSection(config, key)` | `ExpressionRef` | `config.GetSection(key)` |
| `GetValue(config, type, key)` | `ExpressionRef` | `config.GetValue<T>(key)` |
| `GetConnectionString(config, name)` | `ExpressionRef` | `config.GetConnectionString(name)` |
| `Bind(config, key, instance)` | `ExpressionRef` | `config.Bind(key, instance)` |

### Examples

```csharp
// Constructor parameter
ctor.AddParameter("configuration", ConfigurationRefs.IConfiguration)

// Read a section
body.AddStatement(
    $"var section = {ConfigurationRefs.GetSection("_config", "\"Logging\"")};")
// → var section = _config.GetSection("Logging");

// Read a typed value
body.AddStatement(
    $"var timeout = {ConfigurationRefs.GetValue("_config", "int", "\"Timeout\"")};")
// → var timeout = _config.GetValue<int>("Timeout");

// Connection string
body.AddStatement(
    $"var cs = {ConfigurationRefs.GetConnectionString("_config", "\"Default\"")};")
// → var cs = _config.GetConnectionString("Default");

// Bind to options
body.AddStatement(
    $"{ConfigurationRefs.Bind("_config", "\"MyOptions\"", "options")};")
// → _config.Bind("MyOptions", options);
```

---

## LoggingRefs

`Microsoft.Extensions.Logging` types and log-level helpers.

### Types

| Member | Returns | Produces |
|--------|---------|----------|
| `ILogger` | `TypeRef` | `ILogger` |
| `ILoggerOf(category)` | `TypeRef` | `ILogger<T>` |
| `ILoggerFactory` | `TypeRef` | `ILoggerFactory` |
| `LogLevel` | `TypeRef` | `LogLevel` |

### API Call Helpers

All logging helpers take a logger instance and variadic arguments (message + format args):

| Method | Returns | Produces |
|--------|---------|----------|
| `LogTrace(logger, args...)` | `ExpressionRef` | `logger.LogTrace(args...)` |
| `LogDebug(logger, args...)` | `ExpressionRef` | `logger.LogDebug(args...)` |
| `LogInformation(logger, args...)` | `ExpressionRef` | `logger.LogInformation(args...)` |
| `LogWarning(logger, args...)` | `ExpressionRef` | `logger.LogWarning(args...)` |
| `LogError(logger, args...)` | `ExpressionRef` | `logger.LogError(args...)` |
| `LogCritical(logger, args...)` | `ExpressionRef` | `logger.LogCritical(args...)` |

### Examples

```csharp
// Typed logger field
builder.AddField("_logger", LoggingRefs.ILoggerOf("CustomerService"), f => f
    .AsReadonly())

// Logger factory parameter
method.AddParameter("loggerFactory", LoggingRefs.ILoggerFactory)

// Log statements
body.AddStatement(
    $"{LoggingRefs.LogInformation("_logger", "\"Processing order {OrderId}\"", "order.Id")};")
// → _logger.LogInformation("Processing order {OrderId}", order.Id);

body.AddStatement(
    $"{LoggingRefs.LogError("_logger", "\"Failed to process: {Error}\"", "ex.Message")};")
// → _logger.LogError("Failed to process: {Error}", ex.Message);

body.AddStatement(
    $"{LoggingRefs.LogDebug("_logger", "\"Cache hit for key {Key}\"", "key")};")
```
