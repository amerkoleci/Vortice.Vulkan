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

        uint binding_count = 0;
        spvReflectEnumerateDescriptorBindings(&module, &binding_count, null).CheckResult();

        SpvReflectDescriptorBinding** bindings = stackalloc SpvReflectDescriptorBinding*[(int)binding_count];
        spvReflectEnumerateDescriptorBindings(&module, &binding_count, bindings).CheckResult();

        Assert.That(binding_count, Is.EqualTo(1u));
        for (int i = 0; i < binding_count; i++)
        {
            ref SpvReflectDescriptorBinding binding = ref *bindings[i];

            Assert.That(binding.spirv_id, Is.EqualTo(19));
            Assert.That(binding.type_description->id, Is.EqualTo(17));
            //Assert.That(binding.type_description->.type_id, Is.EqualTo(18));
            Assert.That(binding.Name, Is.EqualTo("ubo"));
            Assert.That(binding.type_description->TypeName, Is.EqualTo("UBO"));

            Assert.That(binding.set, Is.EqualTo(0u));
            Assert.That(binding.binding, Is.EqualTo(0u));
        }

        uint input_count = 0;
        spvReflectEnumerateInputVariables(&module, &input_count, null).CheckResult();
        SpvReflectInterfaceVariable** variables = stackalloc SpvReflectInterfaceVariable*[(int)binding_count];
        spvReflectEnumerateInputVariables(&module, &input_count, variables).CheckResult();

        Assert.That(input_count, Is.EqualTo(3u));
        Assert.That(variables[0]->Name, Is.EqualTo("inUV"));
        Assert.That(variables[1]->Name, Is.EqualTo("inPos"));
        Assert.That(variables[2]->Name, Is.EqualTo("inNormal"));

        spvReflectDestroyShaderModule(&module);
    }

    [TestCase]
    public void ParseFragmentResources()
    {
        byte[] bytecode = GetBytecode("texture.frag");

        SpvReflectShaderModule module = default;
        spvReflectCreateShaderModule(bytecode, &module).CheckResult();

        uint binding_count = 0;
        spvReflectEnumerateDescriptorBindings(&module, &binding_count, null).CheckResult();

        SpvReflectDescriptorBinding** bindings = stackalloc SpvReflectDescriptorBinding*[(int)binding_count];
        spvReflectEnumerateDescriptorBindings(&module, &binding_count, bindings).CheckResult();

        Assert.That(binding_count, Is.EqualTo(1u));
        for (int i = 0; i < binding_count; i++)
        {
            ref SpvReflectDescriptorBinding binding = ref *bindings[i];

            Assert.That(binding.spirv_id, Is.EqualTo(13));
            Assert.That(binding.type_description->id, Is.EqualTo(11));
            //Assert.That(binding.type_description->.type_id, Is.EqualTo(18));
            Assert.That(binding.descriptor_type, Is.EqualTo(SpvReflectDescriptorType.CombinedImageSampler));
            Assert.That(binding.Name, Is.EqualTo("samplerColor"));

            Assert.That(binding.set, Is.EqualTo(0u));
            Assert.That(binding.binding, Is.EqualTo(1u));
        }

        spvReflectDestroyShaderModule(&module);
    }

    private static byte[] GetBytecode(string name) => File.ReadAllBytes(Path.Combine(AssetsPath, $"{name}.spv"));
}
