// Copyright (c) Amer Koleci and Contributors.
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
                Assert.That(CompilationStatus.Success, Is.EqualTo(result.Status));

                var shaderCode = result.GetBytecode().ToArray();

                Assert.That(shaderCode.Length > 0, Is.True);
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
                Assert.That(CompilationStatus.compilationError, Is.EqualTo(result.Status));

                var shaderCode = result.GetBytecode().ToArray();

                Assert.That(result.ErrorMessage.Contains("error: 'out_var_ThisIsAnError' : undeclared identifier"), Is.True);
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
                Assert.That(CompilationStatus.Success, Is.EqualTo(result.Status));

                var shaderCode = result.GetBytecode().ToArray();

                Assert.That(shaderCode.Length > 0, Is.True);
            }
        }
    }
}
