// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using NUnit.Framework;

namespace Vortice.ShaderCompiler.Test;

[TestFixture(TestOf = typeof(Compiler))]
public class CompileTests
{
    private string AssetsPath = Path.Combine(AppContext.BaseDirectory, "Assets");

    [TestCase]
    public void SingleFileTest()
    {
        string shaderSourceFile = Path.Combine(AssetsPath, "TriangleVertex.glsl");
        string shaderSource = File.ReadAllText(shaderSourceFile);

        using (var compiler = new Compiler())
        {
            using (var result = compiler.Compile(shaderSource, shaderSourceFile, ShaderKind.VertexShader))
            {
                Assert.AreEqual(CompilationStatus.Success, result.Status);

                var shaderCode = result.GetBytecode().ToArray();

                Assert.True(shaderCode.Length > 0);
            }
        }
    }

    [TestCase]
    public void ErrorTest()
    {
        string shaderSourceFile = Path.Combine(AssetsPath, "TriangleVertexError.glsl");
        string shaderSource = File.ReadAllText(shaderSourceFile);

        using (var compiler = new Compiler())
        {
            using (var result = compiler.Compile(shaderSource, shaderSourceFile, ShaderKind.VertexShader))
            {
                Assert.AreEqual(CompilationStatus.compilationError, result.Status);

                var shaderCode = result.GetBytecode().ToArray();

                Assert.IsTrue(result.ErrorMessage.Contains("error: 'out_var_ThisIsAnError' : undeclared identifier"));
            }
        }
    }

    [TestCase]
    public void IncludeFileTest()
    {
        string shaderSourceFile = Path.Combine(AssetsPath, "TriangleVertexWithInclude.glsl");
        string shaderSource = File.ReadAllText(shaderSourceFile);

        using (var compiler = new Compiler())
        {
            compiler.Includer = new Includer(Path.GetDirectoryName(shaderSourceFile)!);
            using (Result result = compiler.Compile(shaderSource, shaderSourceFile, ShaderKind.VertexShader))
            {
                Assert.AreEqual(CompilationStatus.Success, result.Status);

                var shaderCode = result.GetBytecode().ToArray();

                Assert.True(shaderCode.Length > 0);
            }
        }
    }
}
