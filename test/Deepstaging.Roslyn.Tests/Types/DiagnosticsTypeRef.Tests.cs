// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.Tests.Types;

public class DiagnosticsTypesTests
{
    [Test]
    public async Task Activity_produces_correct_type()
    {
        await Assert.That(DiagnosticsTypes.Activity.Value)
            .IsEqualTo("global::System.Diagnostics.Activity");
    }

    [Test]
    public async Task ActivitySource_produces_correct_type()
    {
        await Assert.That(DiagnosticsTypes.ActivitySource.Value)
            .IsEqualTo("global::System.Diagnostics.ActivitySource");
    }

    [Test]
    public async Task Stopwatch_produces_correct_type()
    {
        await Assert.That(DiagnosticsTypes.Stopwatch.Value)
            .IsEqualTo("global::System.Diagnostics.Stopwatch");
    }

    [Test]
    public async Task Debug_produces_correct_type()
    {
        await Assert.That(DiagnosticsTypes.Debug.Value)
            .IsEqualTo("global::System.Diagnostics.Debug");
    }

    [Test]
    public async Task ActivityKind_produces_correct_type()
    {
        await Assert.That(DiagnosticsTypes.ActivityKind.Value)
            .IsEqualTo("global::System.Diagnostics.ActivityKind");
    }

    [Test]
    public async Task ActivityStatusCode_produces_correct_type()
    {
        await Assert.That(DiagnosticsTypes.ActivityStatusCode.Value)
            .IsEqualTo("global::System.Diagnostics.ActivityStatusCode");
    }

    [Test]
    public async Task DiagnosticSource_produces_correct_type()
    {
        await Assert.That(DiagnosticsTypes.DiagnosticSource.Value)
            .IsEqualTo("global::System.Diagnostics.DiagnosticSource");
    }

    [Test]
    public async Task Process_produces_correct_type()
    {
        await Assert.That(DiagnosticsTypes.Process.Value)
            .IsEqualTo("global::System.Diagnostics.Process");
    }

    [Test]
    public async Task Trace_produces_correct_type()
    {
        await Assert.That(DiagnosticsTypes.Trace.Value)
            .IsEqualTo("global::System.Diagnostics.Trace");
    }

    [Test]
    public async Task Debugger_produces_correct_type()
    {
        await Assert.That(DiagnosticsTypes.Debugger.Value)
            .IsEqualTo("global::System.Diagnostics.Debugger");
    }
}
