// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System;
using System.Diagnostics;

namespace Deepstaging.Roslyn.TypeScript.Emit;

/// <summary>
/// Formats emitted TypeScript code using an external formatter.
/// Supports <c>dprint</c> (preferred, Rust-based, fast) and <c>prettier</c> (fallback via npx).
/// </summary>
internal static class TsFormatter
{
    /// <summary>
    /// Formats TypeScript source code using the first available formatter.
    /// Returns the original code unchanged if no formatter is available.
    /// </summary>
    /// <param name="code">The TypeScript source code to format.</param>
    /// <param name="options">Emit options controlling formatting preferences.</param>
    /// <returns>The formatted code, or the original code if no formatter is available.</returns>
    internal static string Format(string code, TsEmitOptions options)
    {
        // Try dprint first (fast, no Node.js needed)
        var dprintResult = TryFormatWithDprint(code, options);
        if (dprintResult != null)
            return dprintResult;

        // Try prettier via npx
        var prettierResult = TryFormatWithPrettier(code, options);
        if (prettierResult != null)
            return prettierResult;

        // No formatter available — return as-is
        return code;
    }

    /// <summary>
    /// Checks whether any supported formatter is available on the system.
    /// </summary>
    internal static bool IsAvailable()
    {
        return IsDprintAvailable() || IsPrettierAvailable();
    }

    private static string? TryFormatWithDprint(string code, TsEmitOptions options)
    {
        try
        {
            var tabWidth = options.Indentation.Length;
            var useTabs = options.Indentation.Contains("\t");
            var semi = options.UseSemicolons;
            var singleQuote = options.SingleQuotes;
            var trailingComma = options.UseTrailingCommas;

            var args = "fmt --stdin ts" +
                " --plugins https://plugins.dprint.dev/typescript-0.95.4.wasm" +
                $" --config-inline \"indentWidth={tabWidth}\"" +
                $" --config-inline \"useTabs={useTabs.ToString().ToLowerInvariant()}\"" +
                $" --config-inline \"semiColons={(semi ? "always" : "asi")}\"" +
                $" --config-inline \"quoteStyle={(singleQuote ? "preferSingle" : "preferDouble")}\"" +
                $" --config-inline \"trailingCommas={(trailingComma ? "always" : "never")}\"";

            return RunStdinFormatter("dprint", args, code);
        }
        catch
        {
            return null;
        }
    }

    private static string? TryFormatWithPrettier(string code, TsEmitOptions options)
    {
        try
        {
            var tabWidth = options.Indentation.Length;
            var useTabs = options.Indentation.Contains("\t");
            var semi = options.UseSemicolons;
            var singleQuote = options.SingleQuotes;
            var trailingComma = options.UseTrailingCommas;

            var args = $"--yes prettier --parser typescript" +
                $" --tab-width {tabWidth}" +
                $" {(useTabs ? "--use-tabs" : "--no-use-tabs")}" +
                $" {(semi ? "--semi" : "--no-semi")}" +
                $" {(singleQuote ? "--single-quote" : "--no-single-quote")}" +
                $" --trailing-comma {(trailingComma ? "all" : "none")}";

            return RunStdinFormatter("npx", args, code);
        }
        catch
        {
            return null;
        }
    }

    private static string? RunStdinFormatter(string command, string arguments, string input)
    {
        var psi = new ProcessStartInfo
        {
            FileName = command,
            Arguments = arguments,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        using var process = Process.Start(psi);
        if (process == null)
            return null;

        // Write code to stdin and close it
        process.StandardInput.Write(input);
        process.StandardInput.Close();

        var stdout = process.StandardOutput.ReadToEnd();
        process.WaitForExit(30_000);

        if (process.ExitCode != 0)
            return null;

        return string.IsNullOrWhiteSpace(stdout) ? null : stdout;
    }

    private static bool IsDprintAvailable()
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "dprint",
                Arguments = "--version",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using var process = Process.Start(psi);
            if (process == null) return false;
            process.WaitForExit(5_000);
            return process.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsPrettierAvailable()
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "npx",
                Arguments = "--yes prettier --version",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using var process = Process.Start(psi);
            if (process == null) return false;
            process.WaitForExit(15_000);
            return process.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }
}
