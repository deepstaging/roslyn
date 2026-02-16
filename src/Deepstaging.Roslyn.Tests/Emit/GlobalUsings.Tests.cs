// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Emit;

public class GlobalUsingsTests
{
    [Test]
    public async Task Emit_single_namespace()
    {
        var result = GlobalUsings.Emit("System.Text.Json");

        await Assert.That(result).IsEqualTo("global using System.Text.Json;\n");
    }

    [Test]
    public async Task Emit_multiple_namespaces()
    {
        var result = GlobalUsings.Emit(
            "System.Text.Json",
            "System.Collections.Generic",
            "Microsoft.Extensions.Logging");

        await Assert.That(result).Contains("global using System.Text.Json;");
        await Assert.That(result).Contains("global using System.Collections.Generic;");
        await Assert.That(result).Contains("global using Microsoft.Extensions.Logging;");
    }

    [Test]
    public async Task Emit_static_using()
    {
        var result = GlobalUsings.Emit("static System.Math");

        await Assert.That(result).IsEqualTo("global using static System.Math;\n");
    }

    [Test]
    public async Task Emit_mixed_regular_and_static()
    {
        var result = GlobalUsings.Emit(
            "System.Text.Json",
            "static System.Math");

        await Assert.That(result).Contains("global using System.Text.Json;");
        await Assert.That(result).Contains("global using static System.Math;");
    }

    [Test]
    public async Task Emit_from_NamespaceRef()
    {
        var result = GlobalUsings.Emit(
            JsonRefs.Namespace,
            CollectionRefs.Namespace);

        await Assert.That(result).Contains("global using System.Text.Json;");
        await Assert.That(result).Contains("global using System.Collections.Generic;");
    }

    [Test]
    public async Task Emit_with_AsStatic()
    {
        var result = GlobalUsings.Emit(
            NamespaceRef.From("System.Math").AsStatic());

        await Assert.That(result).IsEqualTo("global using static System.Math;\n");
    }

    [Test]
    public async Task Emit_empty_returns_empty()
    {
        var result = GlobalUsings.Emit(Array.Empty<string>());

        await Assert.That(result).IsEqualTo(string.Empty);
    }

    [Test]
    public async Task Emit_skips_whitespace_entries()
    {
        var result = GlobalUsings.Emit("System", "  ", "System.Linq");

        await Assert.That(result).Contains("global using System;");
        await Assert.That(result).Contains("global using System.Linq;");
        await Assert.That(result).DoesNotContain("global using  ");
    }
}