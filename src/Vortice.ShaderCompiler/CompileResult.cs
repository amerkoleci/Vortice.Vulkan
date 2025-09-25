// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.
// Idea and code based on: https://github.com/XenoAtom/XenoAtom.ShaderCompiler/blob/main/src/XenoAtom.ShaderCompiler/ShaderCompilerContext.cs

using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using static Vortice.ShaderCompiler.Native;

namespace Vortice.ShaderCompiler;

public sealed partial class CompileResult
{
    internal unsafe CompileResult(nint native_result, string inputFileName)
    {
        Status = shaderc_result_get_compilation_status(native_result);
        WarningsCount = (uint)shaderc_result_get_num_warnings(native_result);
        ErrorsCount = (uint)shaderc_result_get_num_errors(native_result);

        if (Status != CompilationStatus.Success)
        {
            string[] lines;
            if (Status == CompilationStatus.InvalidStage)
            {
                var errorMessage = $"{Path.GetFullPath(inputFileName)}(1,1): error: Stage not specified by #pragma or not inferred from file extension";
                lines = [errorMessage];
            }
            else
            {
                string? errorMessage = Marshal.PtrToStringAnsi(shaderc_result_get_error_message(native_result));
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    lines = ParseLines(errorMessage, Status == CompilationStatus.CompilationError);
                }
                else
                {
                    lines = [$"Error: {Status}"];
                }
            }

            StringBuilder builder = new();
            foreach (string line in lines)
            {
                builder.AppendLine(line);
            }
            ErrorMessage = builder.ToString();
            Bytecode = [];
        }
        else
        {
            byte* data = shaderc_result_get_bytes(native_result);
            nuint size = shaderc_result_get_length(native_result);
            Span<byte> span = new(data, (int)size);
            Bytecode = span.ToArray();
        }
    }

    public CompilationStatus Status { get; }
    public byte[] Bytecode { get; }
    public uint WarningsCount { get; }
    public uint ErrorsCount { get; }

    /// <summary>
    /// Returns a null-terminated string that contains any error messages generated
    /// during the compilation.
    /// </summary>
    public string? ErrorMessage { get; }

    [GeneratedRegex(@"(?<path>.*?)(:(?<line>\d+))?: (?<kind>error|warning): (?<message>.*)")]
    private static partial Regex RegexMatchLine();

    private static string[] ParseLines(string text, bool replaceErrorToVisualStudioFormat)
    {
        string[] lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

        if (replaceErrorToVisualStudioFormat)
        {
            Regex regex = RegexMatchLine();
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                Match match = regex.Match(line);
                if (match.Success)
                {
                    string path = match.Groups["path"].Value;
                    Group lineText = match.Groups["line"];
                    int lineNo = 1;
                    if (!string.IsNullOrEmpty(lineText.Value))
                    {
                        lineNo = int.Parse(lineText.Value);
                    }
                    if (string.IsNullOrEmpty(path))
                    {
                        path = "1";
                    }

                    string kind = match.Groups["kind"].Value;
                    string message = match.Groups["message"].Value;

                    lines[i] = $"{path}({lineNo}): {kind}: {message}";
                }
            }
        }

        return lines;
    }

}
