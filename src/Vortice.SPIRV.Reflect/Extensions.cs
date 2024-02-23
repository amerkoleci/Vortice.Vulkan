// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using static Vortice.SPIRV.Reflect.Utils;

namespace Vortice.SPIRV.Reflect;

unsafe partial struct SpvReflectTypeDescription
{
    public readonly string? TypeName => GetUtf8Span(type_name).GetString();
    public readonly string? StructMemberName => GetUtf8Span(struct_member_name).GetString();
}

unsafe partial struct SpvReflectInterfaceVariable
{
    public readonly string Name => GetUtf8Span(name).GetString()!;
    public readonly string Semantic => GetUtf8Span(semantic).GetString()!;
}

unsafe partial struct SpvReflectBlockVariable
{
    public readonly string Name => GetUtf8Span(name).GetString()!;
}

unsafe partial struct SpvReflectDescriptorBinding
{
    public readonly string Name => GetUtf8Span(name).GetString()!;
}

unsafe partial struct SpvReflectEntryPoint
{
    public readonly string Name => GetUtf8Span(name).GetString()!;
}

unsafe partial struct SpvReflectSpecializationConstant
{
    public readonly string Name => GetUtf8Span(name).GetString()!;
}

unsafe partial struct SpvReflectShaderModule
{
    public readonly string EntryPointName => GetUtf8Span(entry_point_name).GetString()!;
    public readonly string SourceFile => GetUtf8Span(source_file).GetString()!;
    public readonly string SourceSource => GetUtf8Span(source_source).GetString()!;
}
