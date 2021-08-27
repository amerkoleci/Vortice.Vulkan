// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.IO;
using Vortice.ShaderCompiler;
using Xunit;

namespace Vortice.Dxc.Test
{
    public class CompileTests
    {
        private string AssetsPath = Path.Combine(AppContext.BaseDirectory, "Assets");

        [Fact]
        public void SingleFileTest()
        {
            string shaderSourceFile = Path.Combine(AssetsPath, "TriangleVertex.glsl");
            string shaderSource = File.ReadAllText(shaderSourceFile);

            using (var compiler = new Compiler())
            {
                using (var result = compiler.Compile(shaderSource, shaderSourceFile, ShaderKind.VertexShader))
                {
                    Assert.Equal(CompilationStatus.Success, result.Status);

                    var shaderCode = result.GetBytecode().ToArray();

                    Assert.True(shaderCode.Length > 0);
                }
            }
        }

        [Fact]
        public void ErrorTest()
        {
            string shaderSourceFile = Path.Combine(AssetsPath, "TriangleVertexError.glsl");
            string shaderSource = File.ReadAllText(shaderSourceFile);

            using (var compiler = new Compiler())
            {
                using (var result = compiler.Compile(shaderSource, shaderSourceFile, ShaderKind.VertexShader))
                {
                    Assert.Equal(CompilationStatus.compilationError, result.Status);

                    var shaderCode = result.GetBytecode().ToArray();

                    Assert.Contains("error: 'out_var_ThisIsAnError' : undeclared identifier", result.ErrorMessage);
                }
            }
        }
    }
}
