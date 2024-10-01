// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.
// Idea and code based on: https://github.com/XenoAtom/XenoAtom.ShaderCompiler/blob/main/src/XenoAtom.ShaderCompiler/ShaderCompilerContext.cs

using System.Runtime.InteropServices;
using System.Text;
using static Vortice.ShaderCompiler.Native;

namespace Vortice.ShaderCompiler;

public sealed unsafe class Compiler : IDisposable
{
    private nint _compiler;
    private GCHandle _handle;
    private readonly HashSet<string> _includedFiles = [];
    private readonly List<string> _allIncludeDirectories = [];

    public Compiler()
    {
        _compiler = shaderc_compiler_initialize();
        if (_compiler == 0)
        {
            throw new Exception("Cannot initialize native handle");
        }

        _handle = GCHandle.Alloc(this);
    }

    public void Dispose()
    {
        if (_compiler == 0)
            return;

        if (_handle.IsAllocated)
        {
            _handle.Free();
        }

        shaderc_compiler_release(_compiler);

        _compiler = 0;
    }

    public static void GetSpvVersion(out uint version, out uint revision) => shaderc_get_spv_version(out version, out revision);

    public CompileResult Compile(string fileName, CompilerOptions? options = default)
    {
        if (!File.Exists(fileName))
            throw new FileNotFoundException(fileName);

        return Compile(File.ReadAllText(fileName), fileName, options);
    }

    public CompileResult Compile(string source, string fileName, CompilerOptions? options = default)
    {
        options ??= new CompilerOptions();

        _allIncludeDirectories.Clear();
        _allIncludeDirectories.AddRange(options.IncludeDirectories);
        _allIncludeDirectories.Add(Environment.CurrentDirectory);

        // Clear the list of included files
        _includedFiles.Clear();

        // Setup options now
        nint native_options = shaderc_compile_options_initialize();

        try
        {
            // Add macro definitions
            foreach (ShaderMacro define in options.Defines)
            {
                shaderc_compile_options_add_macro_definition(native_options, define.Key, define.Value ?? string.Empty);
            }

            shaderc_compile_options_set_optimization_level(native_options, options.OptimizationLevel);

            bool hasHlslFileName = fileName.EndsWith(".hlsl");
            SourceLanguage sourceLanguage = SourceLanguage.GLSL;
            if (options.SourceLanguage.HasValue)
            {
                sourceLanguage = options.SourceLanguage.Value;
            }
            else
            {
                sourceLanguage = hasHlslFileName ? SourceLanguage.HLSL : SourceLanguage.GLSL;
            }
            shaderc_compile_options_set_source_language(native_options, sourceLanguage);

            if (options.TargetEnv.HasValue)
            {
                uint version = 0;
                TargetEnvironment targetEnv = TargetEnvironment.Default;
                switch (options.TargetEnv.Value)
                {
                    case TargetEnvironmentVersion.Vulkan_1_0:
                        version = (uint)options.TargetEnv.Value;
                        targetEnv = TargetEnvironment.Vulkan;
                        break;
                    case TargetEnvironmentVersion.Vulkan_1_1:
                        version = (uint)options.TargetEnv.Value;
                        targetEnv = TargetEnvironment.Vulkan;
                        break;
                    case TargetEnvironmentVersion.Vulkan_1_2:
                        version = (uint)options.TargetEnv.Value;
                        targetEnv = TargetEnvironment.Vulkan;
                        break;
                    case TargetEnvironmentVersion.Vulkan_1_3:
                        version = (uint)options.TargetEnv.Value;
                        targetEnv = TargetEnvironment.Vulkan;
                        break;
                    case TargetEnvironmentVersion.OpenGL_4_5:
                        version = (uint)options.TargetEnv.Value;
                        targetEnv = TargetEnvironment.OpenGL;
                        break;
                }

                shaderc_compile_options_set_target_env(native_options, targetEnv, version);
            }

            if (options.TargetSpv.HasValue)
            {
                shaderc_compile_options_set_target_spirv(native_options, options.TargetSpv.Value);
            }

            if (options.InvertY.HasValue)
            {
                shaderc_compile_options_set_invert_y(native_options, options.InvertY.Value);
            }

            if (options.GeneratedDebug.HasValue && options.GeneratedDebug.Value)
            {
                shaderc_compile_options_set_generate_debug_info(native_options);
            }

            if (options.SuppressWarnings.HasValue && options.SuppressWarnings.Value)
            {
                shaderc_compile_options_set_suppress_warnings(native_options);
            }

            if (options.WarningsAsErrors.HasValue && options.WarningsAsErrors.Value)
            {
                shaderc_compile_options_set_warnings_as_errors(native_options);
            }

            if (options.NaNClamp.HasValue)
            {
                shaderc_compile_options_set_nan_clamp(native_options, options.NaNClamp.Value);
            }

            if (options.AutoMapLocations.HasValue)
            {
                shaderc_compile_options_set_auto_map_locations(native_options, options.AutoMapLocations.Value);
            }

            if (options.AutoBindUniforms.HasValue)
            {
                shaderc_compile_options_set_auto_bind_uniforms(native_options, options.AutoBindUniforms.Value);
            }

            if (options.GLSL_Version.HasValue && options.GLSL_Profile.HasValue)
            {
                shaderc_compile_options_set_forced_version_profile(native_options, options.GLSL_Version.Value, options.GLSL_Profile.Value);
            }


            foreach (var limit in options.Limits)
            {
                shaderc_compile_options_set_limit(native_options, limit.Key, limit.Value);
            }

            foreach (BindingBase binding in options.Bindings)
            {
                if (binding.ShaderStage.HasValue)
                {
                    shaderc_compile_options_set_binding_base_for_stage(native_options, binding.ShaderStage.Value, binding.Kind, binding.Base);
                }
                else
                {
                    shaderc_compile_options_set_binding_base(native_options, binding.Kind, binding.Base);
                }
            }

            if (sourceLanguage == SourceLanguage.HLSL)
            {
                if (options.HlslOffsets.HasValue)
                {
                    shaderc_compile_options_set_hlsl_offsets(native_options, options.HlslOffsets.Value);
                }

                if (options.HlslFunctionality1.HasValue)
                {
                    shaderc_compile_options_set_hlsl_functionality1(native_options, options.HlslFunctionality1.Value);
                }

                if (options.HLSLIoMapping.HasValue)
                {
                    shaderc_compile_options_set_hlsl_io_mapping(native_options, options.HLSLIoMapping.Value);
                }

                if (options.Hlsl16BitTypes.HasValue)
                {
                    shaderc_compile_options_set_hlsl_16bit_types(native_options, options.Hlsl16BitTypes.Value);
                }

                foreach (HLSLRegisterSetAndBinding binding in options.HLSLRegisterSetAndBindings)
                {
                    if (binding.ShaderStage.HasValue)
                    {
                        fixed (byte* regPtr = binding.Register.GetUtf8Span())
                        fixed (byte* setPtr = binding.Set.GetUtf8Span())
                        fixed (byte* bindingPtr = binding.Binding.GetUtf8Span())
                            shaderc_compile_options_set_hlsl_register_set_and_binding_for_stage(native_options, binding.ShaderStage.Value, regPtr, setPtr, bindingPtr);
                    }
                    else
                    {
                        fixed (byte* regPtr = binding.Register.GetUtf8Span())
                        fixed (byte* setPtr = binding.Set.GetUtf8Span())
                        fixed (byte* bindingPtr = binding.Binding.GetUtf8Span())
                            shaderc_compile_options_set_hlsl_register_set_and_binding(native_options, regPtr, setPtr, bindingPtr);
                    }
                }
            }

            shaderc_compile_options_set_include_callbacks(native_options, &shaderc_include_resolve, &shaderc_include_result_release, GCHandle.ToIntPtr(_handle));

            ShaderKind shaderKind;

            if (options.ShaderStage.HasValue)
            {
                shaderKind = options.ShaderStage.Value;
            }
            else
            {
                shaderKind = ShaderKind.GLSL_InferFromSource;
                if (fileName.EndsWith(".vert.hlsl") || fileName.EndsWith(".vert"))
                {
                    shaderKind = ShaderKind.VertexShader;
                }
                else if (fileName.EndsWith(".frag.hlsl") || fileName.EndsWith(".frag"))
                {
                    shaderKind = ShaderKind.FragmentShader;
                }
                else if (fileName.EndsWith(".comp.hlsl") || fileName.EndsWith(".comp"))
                {
                    shaderKind = ShaderKind.ComputeShader;
                }
                else if (fileName.EndsWith(".geom.hlsl") || fileName.EndsWith(".geom"))
                {
                    shaderKind = ShaderKind.GeometryShader;
                }
                else if (fileName.EndsWith(".tesc.hlsl") || fileName.EndsWith(".tesc"))
                {
                    shaderKind = ShaderKind.TessControlShader;
                }
                else if (fileName.EndsWith(".tese.hlsl") || fileName.EndsWith(".tese"))
                {
                    shaderKind = ShaderKind.TessEvaluationShader;
                }
            }

            nint result = shaderc_compile_into_spv(_compiler, source, shaderKind, fileName, options.EntryPoint, native_options);
            CompileResult compileResult = new(result, fileName);
            shaderc_result_release(result);
            return compileResult;
        }
        finally
        {
            shaderc_compile_options_release(native_options);
        }
    }

    [UnmanagedCallersOnly]
    private static shaderc_include_result* shaderc_include_resolve(nint user_data,
        byte* requested_source,
        int type,
        byte* requesting_source,
        nuint include_depth)
    {
        Compiler context = (Compiler)GCHandle.FromIntPtr((IntPtr)user_data).Target!;

        string requestedSource = Marshal.PtrToStringUTF8((nint)requested_source)!;
        string requestingSource = Marshal.PtrToStringUTF8((nint)requesting_source)!;
        shaderc_include_result* includeResult = (shaderc_include_result*)NativeMemory.AllocZeroed((nuint)sizeof(shaderc_include_result));

        try
        {
            if ((shaderc_include_type)type == shaderc_include_type.shaderc_include_type_relative)
            {
                string? includeDirectory = Path.GetDirectoryName(requestingSource);
                if (string.IsNullOrEmpty(includeDirectory))
                {
                    includeDirectory = Environment.CurrentDirectory;
                }

                {
                    string includeFile = Path.GetFullPath(Path.Combine(includeDirectory, requestedSource));
                    if (File.Exists(includeFile))
                    {
                        var content = File.ReadAllText(includeFile);
                        includeResult->content = AllocateString(content, out includeResult->content_length);
                        includeResult->source_name = AllocateString(includeFile, out includeResult->source_name_length);
                        context._includedFiles.Add(includeFile);
                        goto include_found;
                    }
                }
            }

            foreach (string includeDirectory in context._allIncludeDirectories)
            {
                // Make sure that we support relative include directories
                string includeFile = Path.GetFullPath(Path.Combine(includeDirectory, requestedSource));
                if (File.Exists(includeFile))
                {
                    string content = File.ReadAllText(includeFile);
                    includeResult->content = AllocateString(content, out includeResult->content_length);
                    includeResult->source_name = AllocateString(includeFile, out includeResult->source_name_length);
                    context._includedFiles.Add(includeFile);
                    break;
                }
            }

        include_found:
            return includeResult;
        }
        catch
        {
            // ignore
        }

        return includeResult;
    }

    [UnmanagedCallersOnly]
    private static void shaderc_include_result_release(nint user_data, shaderc_include_result* include_result)
    {
        if (include_result == null)
            return;

        if (include_result->content != null)
        {
            NativeMemory.Free((void*)include_result->content);
        }

        if (include_result->source_name != null)
        {
            NativeMemory.Free((void*)include_result->source_name);
        }

        NativeMemory.Free((void*)include_result);
    }

    private static byte* AllocateString(string content, out nuint length)
    {
        length = (nuint)Encoding.UTF8.GetByteCount(content);
        byte* byteContent = (byte*)NativeMemory.Alloc((nuint)length + 1);
        fixed (char* pContent = content)
        {
            Encoding.UTF8.GetBytes(pContent, content.Length, byteContent, (int)length);
        }
        byteContent[length] = 0;
        return byteContent;
    }
}
