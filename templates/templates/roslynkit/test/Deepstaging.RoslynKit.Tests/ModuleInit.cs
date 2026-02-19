// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System.ComponentModel;
using System.Runtime.CompilerServices;
using Deepstaging.Roslyn.Testing;
using Deepstaging.RoslynKit;
#if (includeRuntime)
using Deepstaging.RoslynKit.Runtime;
#endif

namespace Deepstaging.RoslynKit.Tests;

internal static class TestInit
{
    [ModuleInitializer]
    public static void Init() =>
        ReferenceConfiguration.AddReferencesFromTypes(
            typeof(AutoNotifyAttribute),
            typeof(AlsoNotifyAttribute),
#if (includeRuntime)
            typeof(ObservableObject),
#endif
            typeof(INotifyPropertyChanged));
}