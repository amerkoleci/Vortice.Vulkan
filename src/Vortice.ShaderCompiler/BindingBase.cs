// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.ShaderCompiler;

/// <summary>
/// Defines a binding base.
/// </summary>
public class BindingBase
{
    /// <summary>
    /// Creates a new instance of <see cref="BindingBase"/>.
    /// </summary>
    public BindingBase(UniformKind kind, uint @base)
    {
        Kind = kind;
        Base = @base;   
    }

    /// <summary>
    /// Creates a new instance of <see cref="HLSLRegisterSetAndBinding"/>.
    /// </summary>
    public BindingBase(ShaderKind stage, UniformKind kind, uint @base)
        : this(kind, @base)
    {
        ShaderStage = stage;
    }

    public ShaderKind? ShaderStage { get; set; }

    /// <summary>
    /// Gets the uniform kind.
    /// </summary>
    public UniformKind Kind { get; }

    /// <summary>
    /// Gets the base.
    /// </summary>
    public uint Base { get; }
}
