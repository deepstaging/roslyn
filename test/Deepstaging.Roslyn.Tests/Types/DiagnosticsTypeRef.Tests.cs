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
}
