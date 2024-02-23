// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using NUnit.Framework;
using Vortice.SPIRV.Reflect;
using static Vortice.SPIRV.Reflect.SPIRVReflectApi;

namespace Vortice.SpirvCross.Test;

[TestFixture(TestOf = typeof(SPIRVReflectApi))]
public unsafe class Tests
{
    private static readonly string AssetsPath = Path.Combine(AppContext.BaseDirectory, "Assets");

    [TestCase]
    public void ReflectShaderModuleCreation()
    {
        byte[] vertexBytecode = GetBytecode("triangle.vert");
        SpvReflectShaderModule module = default;
        spvReflectCreateShaderModule(vertexBytecode, &module).CheckResult();
        spvReflectDestroyShaderModule(&module);
    }

    [TestCase]
    public void ParseVertexResources()
    {
        byte[] vertexBytecode = GetBytecode("texture.vert");

        SpvReflectShaderModule module = default;
        spvReflectCreateShaderModule(vertexBytecode, &module).CheckResult();

        ReadOnlySpan<SpvReflectDescriptorBinding> bindings = spvReflectEnumerateDescriptorBindings(&module);

        spvReflectDestroyShaderModule(&module);
    }

    private static byte[] GetBytecode(string name)
    {
        return File.ReadAllBytes(Path.Combine(AssetsPath, $"{name}.spv"));
    }
}
