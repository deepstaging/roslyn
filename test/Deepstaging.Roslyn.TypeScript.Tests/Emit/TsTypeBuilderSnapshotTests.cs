// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.TypeScript;
using Deepstaging.Roslyn.TypeScript.Emit;
using Deepstaging.Roslyn.TypeScript.Testing;
using Deepstaging.Roslyn.TypeScript.Types;

namespace Deepstaging.Roslyn.TypeScript.Tests.Emit;

public class TsTypeBuilderSnapshotTests : TsTestBase
{
    [Test]
    public Task UserService_Class() =>
        VerifyEmit(
            TsTypeBuilder.Class("UserService")
                .Exported()
                .AddField("baseUrl", "string", f => f.WithAccessibility(TsAccessibility.Private).AsReadonly())
                .AddConstructor(c => c
                    .AddParameter("http", "HttpClient", p => p.AsParameterProperty(TsAccessibility.Private))
                    .AddParameter("baseUrl", "string", p => p)
                    .WithBody(b => b
                        .AddStatement("this.baseUrl = baseUrl")))
                .AddMethod("getUsers", m => m
                    .Async()
                    .WithReturnType(new TsPromiseTypeRef(new TsArrayTypeRef("User")))
                    .WithBody(b => b
                        .AddConst("response", "await this.http.get(`${this.baseUrl}/users`)")
                        .AddReturn("response.data as User[]")))
                .AddMethod("getUserById", m => m
                    .Async()
                    .AddParameter("id", "string", p => p)
                    .WithReturnType(new TsPromiseTypeRef(TsTypeRef.Union(TsTypeRef.From("User"), TsTypeRef.From("undefined"))))
                    .WithBody(b => b
                        .AddTryCatch(
                            t => t
                                .AddConst("response", "await this.http.get(`${this.baseUrl}/users/${id}`)")
                                .AddReturn("response.data as User"),
                            "error",
                            c => c
                                .AddReturn("undefined"))))
                .Emit(DefaultOptions));
}
