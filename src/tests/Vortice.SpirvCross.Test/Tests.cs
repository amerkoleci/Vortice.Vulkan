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
        compiler.Options.SetUInt(CompilerOption.HLSLShaderModel, 50);
        compiler.Apply();

        string hlsl = compiler.Compile();
        Assert.IsNotEmpty(hlsl);
        Assert.IsEmpty(context.GetLastErrorString());
    }

    private  static byte[] GetBytecode(string name)
    {
        return File.ReadAllBytes(Path.Combine(AssetsPath, $"{name}.spv"));
    }
}
