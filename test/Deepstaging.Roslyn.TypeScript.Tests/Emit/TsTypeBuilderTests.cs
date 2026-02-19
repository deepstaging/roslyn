// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using Deepstaging.Roslyn.TypeScript.Emit;

namespace Deepstaging.Roslyn.TypeScript.Tests.Emit;

public class TsTypeBuilderTests
{
    [Test]
    public async Task EmitsExportedInterface()
    {
        var result = TsTypeBuilder.Interface("User")
            .Exported()
            .AddProperty("id", "number", p => p.AsReadonly())
            .AddProperty("name", "string", p => p)
            .AddProperty("email", "string", p => p.AsOptional())
            .Emit();

        var code = result.ValidateOrThrow().Code;
        await Assert.That(code).Contains("export interface User");
        await Assert.That(code).Contains("readonly id: number;");
        await Assert.That(code).Contains("name: string;");
        await Assert.That(code).Contains("email?: string;");
    }

    [Test]
    public async Task EmitsClassWithConstructorAndMethods()
    {
        var result = TsTypeBuilder.Class("UserService")
            .Exported()
            .Extends("BaseService")
            .Implements("IUserService")
            .AddField("users", "Map<string, User>", f => f
                .WithAccessibility(TsAccessibility.Private)
                .AsReadonly()
                .WithInitializer("new Map()"))
            .AddConstructor(c => c
                .AddParameter("config", "ServiceConfig")
                .CallsSuper("config")
                .WithBody(b => b
                    .AddStatement("this.users = new Map()")))
            .AddMethod("getUser", m => m
                .Async()
                .AddParameter("id", "string")
                .WithReturnType("Promise<User | undefined>")
                .WithBody(b => b
                    .AddReturn("this.users.get(id)")))
            .AddMethod("deleteUser", m => m
                .AddParameter("id", "string")
                .WithReturnType("void")
                .WithExpressionBody("this.users.delete(id)"))
            .Emit();

        var code = result.ValidateOrThrow().Code;
        await Assert.That(code).Contains("export class UserService extends BaseService implements IUserService");
        await Assert.That(code).Contains("private readonly users: Map<string, User> = new Map();");
        await Assert.That(code).Contains("constructor(config: ServiceConfig)");
        await Assert.That(code).Contains("super(config);");
        await Assert.That(code).Contains("async getUser(id: string): Promise<User | undefined>");
        await Assert.That(code).Contains("return this.users.get(id);");
        await Assert.That(code).Contains("deleteUser(id: string): void");
        await Assert.That(code).Contains("return this.users.delete(id);");
    }

    [Test]
    public async Task EmitsTypeAlias()
    {
        var result = TsTypeBuilder.TypeAlias("UserId", "string | number")
            .Exported()
            .Emit();

        var code = result.ValidateOrThrow().Code;
        await Assert.That(code).Contains("export type UserId = string | number;");
    }

    [Test]
    public async Task EmitsEnum()
    {
        var result = TsTypeBuilder.Enum("Status")
            .Exported()
            .AddEnumMember("Active", "'active'")
            .AddEnumMember("Inactive", "'inactive'")
            .AddEnumMember("Pending", "'pending'")
            .Emit();

        var code = result.ValidateOrThrow().Code;
        await Assert.That(code).Contains("export enum Status");
        await Assert.That(code).Contains("Active = 'active',");
        await Assert.That(code).Contains("Inactive = 'inactive',");
    }

    [Test]
    public async Task EmitsAbstractClass()
    {
        var result = TsTypeBuilder.Class("Repository")
            .Exported()
            .AsAbstract()
            .AddTypeParameter("T")
            .AddMethod("findById", m => m
                .AsAbstract()
                .AddParameter("id", "string")
                .WithReturnType("Promise<T | null>"))
            .AddMethod("save", m => m
                .AsAbstract()
                .AddParameter("entity", "T")
                .WithReturnType("Promise<void>"))
            .Emit();

        var code = result.ValidateOrThrow().Code;
        await Assert.That(code).Contains("export abstract class Repository<T>");
        await Assert.That(code).Contains("abstract findById(id: string): Promise<T | null>;");
        await Assert.That(code).Contains("abstract save(entity: T): Promise<void>;");
    }

    [Test]
    public async Task EmitsGenericInterfaceWithIndexSignature()
    {
        var result = TsTypeBuilder.Interface("Dictionary")
            .Exported()
            .AddTypeParameter("T")
            .AddIndexSignature("key", "string", "T")
            .AddProperty("length", "number", p => p.AsReadonly())
            .Emit();

        var code = result.ValidateOrThrow().Code;
        await Assert.That(code).Contains("export interface Dictionary<T>");
        await Assert.That(code).Contains("[key: string]: T;");
        await Assert.That(code).Contains("readonly length: number;");
    }

    [Test]
    public async Task EmitsClassWithDecorators()
    {
        var result = TsTypeBuilder.Class("UserController")
            .Exported()
            .WithDecorator("@Controller('/users')")
            .AddMethod("getAll", m => m
                .WithDecorator("@Get('/')")
                .Async()
                .WithReturnType("Promise<User[]>")
                .WithBody(b => b
                    .AddReturn("this.userService.findAll()")))
            .Emit();

        var code = result.ValidateOrThrow().Code;
        await Assert.That(code).Contains("@Controller('/users')");
        await Assert.That(code).Contains("@Get('/')");
        await Assert.That(code).Contains("export class UserController");
    }

    [Test]
    public async Task EmitsConstructorParameterProperties()
    {
        var result = TsTypeBuilder.Class("Point")
            .Exported()
            .AddConstructor(c => c
                .AddParameter("x", "number", p => p.AsParameterProperty(TsAccessibility.Public).AsReadonlyParameterProperty())
                .AddParameter("y", "number", p => p.AsParameterProperty(TsAccessibility.Public).AsReadonlyParameterProperty()))
            .Emit();

        var code = result.ValidateOrThrow().Code;
        await Assert.That(code).Contains("public readonly x: number");
        await Assert.That(code).Contains("public readonly y: number");
    }
}
