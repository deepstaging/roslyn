// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Roslyn.TypeScript.Expressions;

/// <summary>Factory methods that produce <see cref="TsExpressionRef"/> values for TypeScript <c>console</c> operations.</summary>
public static class TsConsoleExpression
{
    private static readonly TsExpressionRef Console = TsExpressionRef.From("console");

    /// <summary>Produces <c>console.log(...args)</c>.</summary>
    /// <param name="args">The values to log.</param>
    public static TsExpressionRef Log(params TsExpressionRef[] args) =>
        Console.Call("log", args);

    /// <summary>Produces <c>console.error(...args)</c>.</summary>
    /// <param name="args">The values to log as errors.</param>
    public static TsExpressionRef Error(params TsExpressionRef[] args) =>
        Console.Call("error", args);

    /// <summary>Produces <c>console.warn(...args)</c>.</summary>
    /// <param name="args">The values to log as warnings.</param>
    public static TsExpressionRef Warn(params TsExpressionRef[] args) =>
        Console.Call("warn", args);

    /// <summary>Produces <c>console.info(...args)</c>.</summary>
    /// <param name="args">The values to log as info.</param>
    public static TsExpressionRef Info(params TsExpressionRef[] args) =>
        Console.Call("info", args);

    /// <summary>Produces <c>console.debug(...args)</c>.</summary>
    /// <param name="args">The values to log as debug.</param>
    public static TsExpressionRef Debug(params TsExpressionRef[] args) =>
        Console.Call("debug", args);

    /// <summary>Produces <c>console.table(data)</c>.</summary>
    /// <param name="data">The data to display as a table.</param>
    public static TsExpressionRef Table(TsExpressionRef data) =>
        Console.Call("table", data);

    /// <summary>Produces <c>console.time(label)</c>.</summary>
    /// <param name="label">The timer label.</param>
    public static TsExpressionRef Time(TsExpressionRef label) =>
        Console.Call("time", label);

    /// <summary>Produces <c>console.timeEnd(label)</c>.</summary>
    /// <param name="label">The timer label.</param>
    public static TsExpressionRef TimeEnd(TsExpressionRef label) =>
        Console.Call("timeEnd", label);
}
