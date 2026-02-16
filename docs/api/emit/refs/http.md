# HttpRefs

HTTP types, method constants, and API call helpers for `System.Net.Http`.

> **See also:** [Refs Overview](index.md) | [TypeRef & Primitives](../type-ref.md)

---

## Types

| Member | Returns | Produces |
|--------|---------|----------|
| `Client` | `TypeRef` | `HttpClient` |
| `RequestMessage` | `TypeRef` | `HttpRequestMessage` |
| `ResponseMessage` | `TypeRef` | `HttpResponseMessage` |
| `Method` | `TypeRef` | `HttpMethod` |
| `Content` | `TypeRef` | `HttpContent` |
| `StringContent` | `TypeRef` | `StringContent` |
| `ByteArrayContent` | `TypeRef` | `ByteArrayContent` |
| `StreamContent` | `TypeRef` | `StreamContent` |

## HTTP Method Expressions

| Member | Returns | Produces |
|--------|---------|----------|
| `Get` | `ExpressionRef` | `HttpMethod.Get` |
| `Post` | `ExpressionRef` | `HttpMethod.Post` |
| `Put` | `ExpressionRef` | `HttpMethod.Put` |
| `Patch` | `ExpressionRef` | `HttpMethod.Patch` |
| `Delete` | `ExpressionRef` | `HttpMethod.Delete` |
| `Verb(string)` | `ExpressionRef` | `HttpMethod.{verb}` |

!!! note "These are ExpressionRef, not TypeRef"
    `HttpMethod.Get` is a static property (a value), not a type. The `Verb()` method validates against known HTTP verbs: Get, Post, Put, Patch, Delete, Head, Options, Trace.

## API Call Helpers

| Method | Returns | Produces |
|--------|---------|----------|
| `GetAsync(client, url)` | `ExpressionRef` | `client.GetAsync(url)` |
| `PostAsync(client, url, content)` | `ExpressionRef` | `client.PostAsync(url, content)` |
| `PutAsync(client, url, content)` | `ExpressionRef` | `client.PutAsync(url, content)` |
| `DeleteAsync(client, url)` | `ExpressionRef` | `client.DeleteAsync(url)` |
| `SendAsync(client, request)` | `ExpressionRef` | `client.SendAsync(request)` |
| `SendAsync(client, request, ct)` | `ExpressionRef` | `client.SendAsync(request, ct)` |
| `EnsureSuccessStatusCode(response)` | `ExpressionRef` | `response.EnsureSuccessStatusCode()` |
| `ReadAsStringAsync(response)` | `ExpressionRef` | `response.Content.ReadAsStringAsync()` |

---

## Examples

### Types and Parameters

```csharp
// HttpClient field
builder.AddField("_client", HttpRefs.Client, f => f.AsReadonly())

// HttpRequestMessage parameter
method.AddParameter("request", HttpRefs.RequestMessage)

// Return type
method.WithReturnType(TaskRefs.Task(HttpRefs.ResponseMessage))
```

### HTTP Method Constants

```csharp
// Construct a request with a specific method
body.AddStatement($"var request = new {HttpRefs.RequestMessage}({HttpRefs.Post}, url);")
// → var request = new global::...HttpRequestMessage(global::...HttpMethod.Post, url);

// Dynamic verb
body.AddStatement($"var method = {HttpRefs.Verb("Patch")};")
// → var method = global::System.Net.Http.HttpMethod.Patch;
```

### API Call Chains

```csharp
// GET request with response handling
body
    .AddStatement($"var response = await {HttpRefs.GetAsync("_client", "url")};")
    .AddStatement($"{HttpRefs.EnsureSuccessStatusCode("response")};")
    .AddStatement($"var content = await {HttpRefs.ReadAsStringAsync("response")};")
    .AddReturn(JsonRefs.Deserialize("OrderDto", "content"))

// POST request
body.AddStatement(
    $"var response = await {HttpRefs.PostAsync("_client", "url", "payload")};")

// SendAsync with cancellation
body.AddStatement(
    $"var response = await {HttpRefs.SendAsync("_client", "request", "ct")};")
```
