// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using NUnit.Framework;
using Vortice.SPIRV;
using static Vortice.SpirvCross.SpirvCrossApi;
using static Vortice.SpirvCross.spvc_backend;
using static Vortice.SpirvCross.spvc_capture_mode;
using static Vortice.SpirvCross.spvc_compiler_option;
using static Vortice.SpirvCross.spvc_resource_type;

namespace Vortice.SpirvCross.Test;

[TestFixture(TestOf = typeof(spvc_context))]
public unsafe class Tests
{
    private static readonly string AssetsPath = Path.Combine(AppContext.BaseDirectory, "Assets");

    [TestCase]
    public void ContextTest()
    {
        spvc_get_version(out uint major, out uint minor, out uint patch);

        spvc_context_create(out spvc_context _handle).CheckResult("Cannot create SPIRV-Cross context");
        //spvc_context_set_error_callback(_handle, &OnErrorCallback, 0);
        Assert.That(spvc_context_get_last_error_string(_handle), Is.Empty);

        spvc_context_release_allocations(_handle);
        spvc_context_destroy(_handle);
    }

    [TestCase]
    public void GLSLCompilerTest()
    {
        byte[] vertexBytecode = GetBytecode("triangle.vert");
        spvc_context_create(out spvc_context context).CheckResult();
        spvc_context_parse_spirv(context, vertexBytecode, out spvc_parsed_ir parsedIr).CheckResult();
        spvc_context_create_compiler(context, SPVC_BACKEND_GLSL, parsedIr, SPVC_CAPTURE_MODE_TAKE_OWNERSHIP, out spvc_compiler compiler).CheckResult();

        spvc_compiler_create_compiler_options(compiler, out spvc_compiler_options options).CheckResult();
        spvc_compiler_options_set_uint(options, SPVC_COMPILER_OPTION_GLSL_VERSION, 330);
        spvc_compiler_options_set_bool(options, SPVC_COMPILER_OPTION_GLSL_ES, SPVC_FALSE);
        spvc_compiler_install_compiler_options(compiler, options);

        spvc_compiler_compile(compiler, out string? glsl);
        Assert.That(glsl, Is.Not.Empty);
        Assert.That(spvc_context_get_last_error_string(context), Is.Empty);

        spvc_context_release_allocations(context);
        spvc_context_destroy(context);
    }

    [TestCase]
    public void HLSLCompilerTest()
    {
        byte[] vertexBytecode = GetBytecode("triangle.frag");

        spvc_context_create(out spvc_context context).CheckResult();
        spvc_context_parse_spirv(context, vertexBytecode, out spvc_parsed_ir parsedIr).CheckResult();
        spvc_context_create_compiler(context, SPVC_BACKEND_HLSL, parsedIr, SPVC_CAPTURE_MODE_TAKE_OWNERSHIP, out spvc_compiler compiler).CheckResult();

        spvc_compiler_create_compiler_options(compiler, out spvc_compiler_options options).CheckResult();
        spvc_compiler_options_set_uint(options, SPVC_COMPILER_OPTION_HLSL_SHADER_MODEL, 50);
        spvc_compiler_install_compiler_options(compiler, options);

        spvc_compiler_compile(compiler, out string? hlsl);
        Assert.That(hlsl, Is.Not.Empty);
        Assert.That(spvc_context_get_last_error_string(context), Is.Empty);

        spvc_context_release_allocations(context);
        spvc_context_destroy(context);
    }

    [TestCase]
    public void ParseVertexResources()
    {
        byte[] vertexBytecode = GetBytecode("texture.vert");

        spvc_context_create(out spvc_context context).CheckResult();
        spvc_context_parse_spirv(context, vertexBytecode, out spvc_parsed_ir parsedIr).CheckResult();
        spvc_context_create_compiler(context, SPVC_BACKEND_GLSL, parsedIr, SPVC_CAPTURE_MODE_TAKE_OWNERSHIP, out spvc_compiler compiler_glsl).CheckResult();

        spvc_compiler_create_shader_resources(compiler_glsl, out spvc_resources resources);

        spvc_reflected_resource* resourceList;
        spvc_resources_get_resource_list_for_type(resources, SPVC_RESOURCE_TYPE_UNIFORM_BUFFER, out resourceList, out nuint resourceSize).CheckResult();

        Assert.That(resourceSize, Is.EqualTo((nuint)1));

        for (uint i = 0; i < (uint)resourceSize; i++)
        {
            Assert.That(resourceList[i].id, Is.EqualTo(19));
            Assert.That(resourceList[i].base_type_id, Is.EqualTo(17));
            Assert.That(resourceList[i].type_id, Is.EqualTo(18));
            Assert.That(resourceList[i].GetName(), Is.EqualTo("UBO"));

            uint descriptorSet = spvc_compiler_get_decoration(compiler_glsl, resourceList[i].id, SpvDecoration.DescriptorSet);
            uint binding = spvc_compiler_get_decoration(compiler_glsl, resourceList[i].id, SpvDecoration.Binding);
            Assert.That(descriptorSet, Is.EqualTo(0u));
            Assert.That(binding, Is.EqualTo(0u));
        }

        Assert.That(spvc_context_get_last_error_string(context), Is.Empty);

        spvc_resources_get_resource_list_for_type(resources, SPVC_RESOURCE_TYPE_STAGE_INPUT, out resourceList, out resourceSize).CheckResult();
        Assert.That(resourceSize, Is.EqualTo((nuint)3));
        Assert.That(resourceList[0].GetName(), Is.EqualTo("inUV"));
        Assert.That(resourceList[1].GetName(), Is.EqualTo("inPos"));
        Assert.That(resourceList[2].GetName(), Is.EqualTo("inNormal"));

        Assert.That(spvc_context_get_last_error_string(context), Is.Empty);
        spvc_context_release_allocations(context);
        spvc_context_destroy(context);
    }

    [TestCase]
    public void ParseFragmentResources()
    {
        byte[] vertexBytecode = GetBytecode("texture.frag");

        spvc_context_create(out spvc_context context).CheckResult();
        spvc_context_parse_spirv(context, vertexBytecode, out spvc_parsed_ir parsedIr).CheckResult();
        spvc_context_create_compiler(context, SPVC_BACKEND_GLSL, parsedIr, SPVC_CAPTURE_MODE_TAKE_OWNERSHIP, out spvc_compiler compiler_glsl).CheckResult();

        spvc_compiler_create_shader_resources(compiler_glsl, out spvc_resources resources);
        spvc_resources_get_resource_list_for_type(resources, SPVC_RESOURCE_TYPE_SAMPLED_IMAGE, out spvc_reflected_resource* resourceList, out nuint resourceSize).CheckResult();
        ReadOnlySpan<spvc_combined_image_sampler> samplers = spvc_compiler_get_combined_image_samplers(compiler_glsl);

        Assert.That(resourceSize, Is.EqualTo((nuint)1));

        for (uint i = 0; i < (uint)resourceSize; i++)
        {
            Assert.That(resourceList[i].id, Is.EqualTo(13));
            Assert.That(resourceList[i].base_type_id, Is.EqualTo(11));
            Assert.That(resourceList[i].type_id, Is.EqualTo(12));
            Assert.That(resourceList[i].GetName(), Is.EqualTo("samplerColor"));

            uint descriptorSet = spvc_compiler_get_decoration(compiler_glsl, resourceList[i].id, SpvDecoration.DescriptorSet);
            uint binding = spvc_compiler_get_decoration(compiler_glsl, resourceList[i].id, SpvDecoration.Binding);
            Assert.That(descriptorSet, Is.EqualTo(0u));
            Assert.That(binding, Is.EqualTo(1u));
        }

        Assert.That(spvc_context_get_last_error_string(context), Is.Empty);
        spvc_context_release_allocations(context);
        spvc_context_destroy(context);
    }

    private  static byte[] GetBytecode(string name)
    {
        return File.ReadAllBytes(Path.Combine(AssetsPath, $"{name}.spv"));
    }
}
