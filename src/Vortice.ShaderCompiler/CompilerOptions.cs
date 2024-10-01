// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.ShaderCompiler;

public sealed class CompilerOptions
{
    public string EntryPoint { get; set; } = "main";

    public List<string> IncludeDirectories { get; } = [];

    /// <summary>
    /// Gets the list of defines.
    /// </summary>
    public List<ShaderMacro> Defines { get; } = [];

    public OptimizationLevel OptimizationLevel { get; set; } = OptimizationLevel.Performance;

    public SourceLanguage? SourceLanguage { get; set; }

    /// <summary>
    /// Gets or sets whether to invert Y
    /// </summary>
    public bool? InvertY { get; set; }

    /// <summary>
    /// Gets or sets whether to generate debug information.
    /// </summary>
    public bool? GeneratedDebug { get; set; }

    /// <summary>
    /// Gets or sets whether should suppress compiler warnings.
    /// </summary>
    public bool? SuppressWarnings { get; set; }

    /// <summary>
    /// Sets the compiler mode to treat all warnings as errors. Note the
    /// suppress-warnings mode overrides this option, i.e. if both
    /// warning-as-errors and suppress-warnings modes are set, warnings will not
    /// be emitted as error messages.
    /// </summary>
    public bool? WarningsAsErrors { get; set; }

    /// <summary>
    /// Gets or sets the target environment (vulkan, opengl...)
    /// </summary>
    public TargetEnvironmentVersion? TargetEnv { get; set; }

    /// <summary>
    /// Gets or sets the shader stage (vertex, fragment, compute...)
    /// </summary>
    public ShaderKind? ShaderStage { get; set; }

    /// <summary>
    /// Gets or sets the target SPIR-V version.
    /// </summary>
    public SpirVVersion? TargetSpv { get; set; }

    /// <summary>
    /// Sets whether the compiler generates code for max and min builtins which,
    /// if given a NaN operand, will return the other operand. Similarly, the clamp
    /// builtin will favour the non-NaN operands, as if clamp were implemented
    /// as a composition of max and min.
    /// </summary>
    public bool? NaNClamp { get; set; }

    /// <summary>
    /// Sets whether the compiler should automatically assign locations to uniform variables that don't have explicit locations in the shader source.
    /// </summary>
    public bool? AutoMapLocations { get; set; }

    /// <summary>
    /// Sets whether the compiler should automatically assign bindings to uniforms that aren't already explicitly bound in the shader source.
    /// </summary>
    public bool? AutoBindUniforms { get; set; }

    /// <summary>
    /// Gets the limits.
    /// </summary>
    public Dictionary<Limit, int> Limits { get; } = [];

    /// <summary>
    /// Gets the uniform kind base bindings.
    /// </summary>
    public List<BindingBase> Bindings { get; } = [];

    public int? GLSL_Version { get; set; }
    public Profile? GLSL_Profile { get; set; }

    #region HLSL options
    public bool? HlslOffsets { get; set; }
    public bool? HlslFunctionality1 { get; set; }
    public bool? HLSLIoMapping { get; set; }
    public bool? Hlsl16BitTypes { get; set; }

    /// <summary>
    /// Gets the hlsl register sets and bindings.
    /// </summary>
    public List<HLSLRegisterSetAndBinding> HLSLRegisterSetAndBindings { get; } = [];
    #endregion
}
