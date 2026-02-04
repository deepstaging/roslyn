// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;
using Scriban.Runtime;

namespace Deepstaging.Roslyn.Scriban;

/// <summary>
/// Factory for creating Scriban script contexts from .NET objects.
/// Provides a clean API for deep-importing objects into ScriptObject instances.
/// </summary>
public static class DeepstagingTemplateObject
{
    /// <summary>
    /// Creates a new ScriptObject by deeply importing the specified object.
    /// This is a convenience method equivalent to: new ScriptObject().ImportDeep(obj, renamer).
    /// </summary>
    /// <param name="obj">The object to import into a ScriptObject</param>
    /// <param name="renamer">Optional member renamer (defaults to preserving original names)</param>
    /// <returns>A new ScriptObject containing the deeply imported object graph</returns>
    public static ScriptObject From(object? obj, MemberRenamerDelegate? renamer = null)
    {
        var scriptObject = new ScriptObject();
        return scriptObject.ImportDeep(obj, renamer);
    }
}

/// <summary>
/// Extensions for ScriptObject to support deep importing of .NET objects.
/// 
/// Performance Note:
/// Uses ConcurrentDictionary to cache reflection metadata (PropertyInfo[]) per Type.
/// This is critical because ImportDeep runs during source generation, and reflection
/// is expensive. Cache ensures O(1) lookup after first access per type.
/// 
/// Typical performance:
/// - First import of a type: ~1-2ms (reflection + cache)
/// - Subsequent imports: ~0.01ms (cache hit)
/// 
/// This cache is safe because PropertyInfo[] for a type never changes during compilation.
/// </summary>
public static class ScriptObjectExtensions
{
    // Cache reflection data to avoid repeated GetProperties() calls
    // ConcurrentDictionary is thread-safe (generators may run in parallel)
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> PropertyCache = new();

    /// <summary>
    /// Deeply imports a .NET object into a ScriptObject, recursively handling nested objects and collections.
    /// This allows query models with nested structures to be used directly in Scriban templates.
    /// Uses reflection caching for performance during source generation.
    /// </summary>
    /// <param name="scriptObject">The target ScriptObject</param>
    /// <param name="obj">The object to import</param>
    /// <param name="renamer">Optional member renamer (defaults to preserving original names)</param>
    public static ScriptObject ImportDeep(this ScriptObject scriptObject, object? obj, MemberRenamerDelegate? renamer = null)
    {
        if (obj == null)
            return scriptObject;

        renamer ??= member => member.Name;

        // Get cached properties for this type (avoids repeated reflection)
        var properties = PropertyCache.GetOrAdd(obj.GetType(), t => 
            t.GetProperties(BindingFlags.Public | BindingFlags.Instance));
        
        foreach (var property in properties)
        {
            try
            {
                if (!property.CanRead)
                    continue;

                // Skip indexer properties
                if (property.GetIndexParameters().Length > 0)
                    continue;

                var name = renamer(property);
                var value = property.GetValue(obj);
                
                scriptObject[name] = ConvertValue(value, renamer);
            }
            catch
            {
                // Skip properties that can't be accessed
                continue;
            }
        }
        
        return scriptObject;
    }

    /// <summary>
    /// Converts a .NET value to a Scriban-compatible representation.
    /// Handles primitives (pass-through), collections (converts to ScriptArray),
    /// and complex objects (recursively imports to ScriptObject).
    /// </summary>
    /// <param name="value">The value to convert</param>
    /// <param name="renamer">Member renamer to use for nested objects</param>
    /// <returns>A Scriban-compatible value (primitive, ScriptArray, or ScriptObject)</returns>
    private static object? ConvertValue(object? value, MemberRenamerDelegate renamer)
    {
        if (value == null)
            return null;

        var type = value.GetType();

        // Handle primitive types and strings (fast path - no allocation)
        if (type.IsPrimitive || type == typeof(string) || type == typeof(decimal) || type.IsEnum)
            return value;

        // Handle collections (including ImmutableArray)
        if (value is IEnumerable enumerable and not string)
        {
            var array = new ScriptArray();
            foreach (var item in enumerable)
            {
                array.Add(ConvertValue(item, renamer));
            }
            return array;
        }

        // Handle complex objects - import into ScriptObject
        var scriptObject = new ScriptObject();
        scriptObject.ImportDeep(value, renamer);
        return scriptObject;
    }
}
