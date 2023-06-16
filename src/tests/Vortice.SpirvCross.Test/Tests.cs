// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using NUnit.Framework;

namespace Vortice.SpirvCross.Test;

[TestFixture]
public class Tests
{
    private static readonly string AssetsPath = Path.Combine(AppContext.BaseDirectory, "Assets");

    [TestCase]
    public void ContextTest()
    {
        Context.GetVersion(out uint major, out uint minor, out uint patch);
        using Context context = new();
        Assert.IsEmpty(context.GetLastErrorString());
    }

    [TestCase]
    public void GLSLCompilerTest()
    {
        byte[] vertexBytecode = GetBytecode("triangle.vert");
        using Context context = new();
        context.ParseSpirv(vertexBytecode, out SpvcParsedIr parsedIr).CheckResult();
        Compiler compiler = context.CreateCompiler(Backend.GLSL, parsedIr, CaptureMode.TakeOwnership);
        compiler.Options.SetUInt(CompilerOption.GLSL_Version, 330);
        compiler.Options.SetBool(CompilerOption.GLSL_ES, false);
        compiler.Apply();

        string glsl = compiler.Compile();
        Assert.IsNotEmpty(glsl);
        Assert.IsEmpty(context.GetLastErrorString());
    }

    [TestCase]
    public void HLSLCompilerTest()
    {
        byte[] vertexBytecode = GetBytecode("triangle.frag");
        using Context context = new();
        context.ParseSpirv(vertexBytecode, out SpvcParsedIr parsedIr).CheckResult();
        Compiler compiler = context.CreateCompiler(Backend.HLSL, parsedIr, CaptureMode.TakeOwnership);
        compiler.Options.SetUInt(CompilerOption.HLSL_ShaderModel, 50);
        compiler.Apply();

        string hlsl = compiler.Compile();
        Assert.IsNotEmpty(hlsl);
        Assert.IsEmpty(context.GetLastErrorString());
    }

    [TestCase]
    public void ParseVertexResources()
    {
        byte[] vertexBytecode = GetBytecode("texture.vert");
        using Context context = new();
        context.ParseSpirv(vertexBytecode, out SpvcParsedIr parsedIr).CheckResult();
        Compiler compiler = context.CreateCompiler(Backend.GLSL, parsedIr, CaptureMode.TakeOwnership);

        compiler.CreateShaderResources(out Resources resources).CheckResult();
        SpvReflectedResource[] list = resources.GetResourceListForType(SpvResourceType.UniformBuffer);

        Assert.AreEqual(list.Length, 1);

        for (int i = 0; i < list.Length; i++)
        {
            Assert.AreEqual(list[i].Id, 19);
            Assert.AreEqual(list[i].BaseTypeId, 17);
            Assert.AreEqual(list[i].TypeId, 18);
            Assert.AreEqual(list[i].Name, "UBO");

            int descriptorSet = compiler.GetDecoration(list[i].Id, SpvDecoration.DescriptorSet);
            int binding = compiler.GetDecoration(list[i].Id, SpvDecoration.Binding);
            Assert.AreEqual(descriptorSet, 0);
            Assert.AreEqual(binding, 0);
        }

        Assert.IsEmpty(context.GetLastErrorString());

        list = resources.GetResourceListForType(SpvResourceType.StageInput);
        Assert.AreEqual(list.Length, 3);
        Assert.AreEqual(list[0].Name, "inUV");
        Assert.AreEqual(list[1].Name, "inPos");
        Assert.AreEqual(list[2].Name, "inNormal");
    }

    [TestCase]
    public void ParseFragmentResources()
    {
        byte[] vertexBytecode = GetBytecode("texture.frag");
        using Context context = new();
        context.ParseSpirv(vertexBytecode, out SpvcParsedIr parsedIr).CheckResult();
        Compiler compiler = context.CreateCompiler(Backend.GLSL, parsedIr, CaptureMode.TakeOwnership);

        compiler.CreateShaderResources(out Resources resources).CheckResult();
        SpvReflectedResource[] list = resources.GetResourceListForType(SpvResourceType.SampledImage);

        Assert.AreEqual(list.Length, 1);

        for (int i = 0; i < list.Length; i++)
        {
            Assert.AreEqual(list[i].Id, 13);
            Assert.AreEqual(list[i].BaseTypeId, 11);
            Assert.AreEqual(list[i].TypeId, 12);
            Assert.AreEqual(list[i].Name, "samplerColor");

            int descriptorSet = compiler.GetDecoration(list[i].Id, SpvDecoration.DescriptorSet);
            int binding = compiler.GetDecoration(list[i].Id, SpvDecoration.Binding);
            Assert.AreEqual(descriptorSet, 0);
            Assert.AreEqual(binding, 1);
        }

        Assert.IsEmpty(context.GetLastErrorString());
    }

    private  static byte[] GetBytecode(string name)
    {
        return File.ReadAllBytes(Path.Combine(AssetsPath, $"{name}.spv"));
    }
}
