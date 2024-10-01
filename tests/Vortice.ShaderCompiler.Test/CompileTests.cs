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
            var options = new CompilerOptions
            {
                ShaderStage = ShaderKind.VertexShader
            };

            CompileResult result = compiler.Compile(shaderSource, shaderSourceFile, options);
            Assert.That(CompilationStatus.Success, Is.EqualTo(result.Status));

            var shaderCode = result.Bytecode.AsSpan();
            Assert.That(shaderCode.Length > 0, Is.True);
        }
    }


    [TestCase]
    public void ErrorTest()
    {
        string shaderSourceFile = Path.Combine(AssetsPath, "TriangleVertexError.glsl");
        string shaderSource = File.ReadAllText(shaderSourceFile);

        using (Compiler compiler = new())
        {
            var options = new CompilerOptions
            {
                ShaderStage = ShaderKind.VertexShader
            };

            CompileResult result = compiler.Compile(shaderSource, shaderSourceFile, options);
            Assert.That(CompilationStatus.CompilationError, Is.EqualTo(result.Status));
            Assert.That(result.ErrorMessage!.Contains("error: 'out_var_ThisIsAnError' : undeclared identifier"), Is.True);
        }
    }

    [TestCase]
    public void IncludeFileTest()
    {
        string shaderSourceFile = Path.Combine(AssetsPath, "TriangleVertexWithInclude.glsl");
        string shaderSource = File.ReadAllText(shaderSourceFile);

        using (var compiler = new Compiler())
        {
            CompilerOptions options = new()
            {
                ShaderStage = ShaderKind.VertexShader,
                IncludeDirectories =
                {
                    Path.GetDirectoryName(shaderSourceFile)!
                }
            };

            CompileResult result = compiler.Compile(shaderSource, shaderSourceFile, options);
            Assert.That(result.Status, Is.EqualTo(CompilationStatus.Success));

            var shaderCode = result.Bytecode.AsSpan();

            Assert.That(shaderCode.Length > 0, Is.True);
        }
    }

    [TestCase]
    public void MultipleIncludeFileTest()
    {
        string vertexShaderSourceFile = Path.Combine(AssetsPath, "TriangleVertexWithInclude.glsl");
        string fragmentShaderSourceFile = Path.Combine(AssetsPath, "TriangleFragmentWithInclude.glsl");

        using (var compiler = new Compiler())
        {
            CompilerOptions options = new()
            {
                ShaderStage = ShaderKind.VertexShader,
                IncludeDirectories =
                {
                    Path.GetDirectoryName(vertexShaderSourceFile)!
                }
            };

            CompileResult vertexResult = compiler.Compile(vertexShaderSourceFile, options);
            Assert.That(vertexResult.Status, Is.EqualTo(CompilationStatus.Success));
            Assert.That(vertexResult.Bytecode.Length > 0, Is.True);

            // Fragment
            options.ShaderStage = ShaderKind.FragmentShader;
            CompileResult fragmentResult = compiler.Compile(fragmentShaderSourceFile, options);
            Assert.That(fragmentResult.Status, Is.EqualTo(CompilationStatus.Success));
            Assert.That(fragmentResult.Bytecode.Length > 0, Is.True);
        }
    }
}
