// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;

namespace Deepstaging.Roslyn.TypeScript.Emit;

/// <summary>
/// Validates emitted TypeScript code by invoking the TypeScript compiler (<c>tsc</c>).
/// </summary>
internal static class TsValidator
{
    /// <summary>
    /// Validates TypeScript source code using <c>tsc --noEmit</c>.
    /// Returns diagnostics from the TypeScript compiler. An empty array means the code is valid.
    /// </summary>
    /// <param name="code">The TypeScript source code to validate.</param>
    /// <param name="tscPath">Optional path to the <c>tsc</c> executable. Defaults to <c>"tsc"</c> (resolved via PATH) if <c>npx</c> is also unavailable.</param>
    /// <returns>An array of diagnostic strings from the TypeScript compiler.</returns>
    internal static ImmutableArray<string> Validate(string code, string? tscPath = null)
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"ds-roslyn-ts-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDir);

        try
        {
            var tsFile = Path.Combine(tempDir, "emit.ts");
            var tsconfigFile = Path.Combine(tempDir, "tsconfig.json");

            File.WriteAllText(tsFile, code);
            File.WriteAllText(tsconfigFile, TsConfigContent);

            var (exitCode, output) = RunProcess(
                tscPath ?? ResolveTsc(),
                $"--noEmit --pretty false -p \"{tsconfigFile}\"",
                tempDir);

            if (exitCode == 0)
                return ImmutableArray<string>.Empty;

            return ParseDiagnostics(output, "emit.ts");
        }
        finally
        {
            TryDeleteDirectory(tempDir);
        }
    }

    /// <summary>
    /// Checks whether the TypeScript compiler is available on the system.
    /// </summary>
    internal static bool IsAvailable(string? tscPath = null)
    {
        try
        {
            var (exitCode, _) = RunProcess(tscPath ?? ResolveTsc(), "--version", null);
            return exitCode == 0;
        }
        catch
        {
            return false;
        }
    }

    private static string ResolveTsc()
    {
        // Try bare tsc on PATH first
        try
        {
            var (exitCode, _) = RunProcess("tsc", "--version", null);
            if (exitCode == 0)
                return "tsc";
        }
        catch
        {
            // Not on PATH, fall through
        }

        // Try npx as fallback (works if typescript is in a parent node_modules)
        try
        {
            var (exitCode, _) = RunProcess("npx", "tsc --version", null);
            if (exitCode == 0)
                return "npx tsc";
        }
        catch
        {
            // npx not available
        }

        // Last resort — return tsc and let it fail with a clear error
        return "tsc";
    }

    private static (int ExitCode, string Output) RunProcess(string command, string arguments, string? workingDirectory)
    {
        // Split command if it contains spaces (e.g., "npx --yes tsc")
        string fileName;
        string fullArgs;

        if (command.Contains(" "))
        {
            var parts = command.Split(new[] { ' ' }, 2);
            fileName = parts[0];
            fullArgs = parts[1] + " " + arguments;
        }
        else
        {
            fileName = command;
            fullArgs = arguments;
        }

        var psi = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = fullArgs,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        if (workingDirectory != null)
            psi.WorkingDirectory = workingDirectory;

        using var process = Process.Start(psi)
            ?? throw new InvalidOperationException($"Failed to start process: {fileName}");

        var stdout = process.StandardOutput.ReadToEnd();
        var stderr = process.StandardError.ReadToEnd();

        process.WaitForExit(30_000);

        var output = string.IsNullOrEmpty(stdout) ? stderr : stdout;
        return (process.ExitCode, output.Trim());
    }

    private static ImmutableArray<string> ParseDiagnostics(string output, string fileName)
    {
        if (string.IsNullOrWhiteSpace(output))
            return ImmutableArray.Create("error: tsc exited with errors but produced no output.");

        var builder = ImmutableArray.CreateBuilder<string>();
        var lines = output.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var rawLine in lines)
        {
            var line = rawLine.Trim();
            if (string.IsNullOrEmpty(line))
                continue;

            // tsc outputs lines like: emit.ts(3,10): error TS2322: Type 'string' is not assignable...
            // Normalize to our diagnostic format
            if (line.Contains(": error TS"))
                builder.Add($"error: {line}");
            else if (line.Contains(": warning TS"))
                builder.Add($"warning: {line}");
            else if (line.StartsWith("error TS", StringComparison.Ordinal))
                builder.Add($"error: {line}");
            else
                builder.Add(line);
        }

        return builder.ToImmutable();
    }

    private static void TryDeleteDirectory(string path)
    {
        try { Directory.Delete(path, recursive: true); }
        catch { /* best effort cleanup */ }
    }

    private const string TsConfigContent = @"{
  ""compilerOptions"": {
    ""strict"": true,
    ""target"": ""ES2022"",
    ""module"": ""ES2022"",
    ""moduleResolution"": ""bundler"",
    ""noEmit"": true,
    ""skipLibCheck"": true,
    ""lib"": [""ES2022"", ""DOM""]
  },
  ""include"": [""emit.ts""]
}";
}
