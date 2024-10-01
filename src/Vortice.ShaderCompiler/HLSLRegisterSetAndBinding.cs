// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.ShaderCompiler;

/// <summary>
/// Defines a HLSL register set and binding.
/// </summary>
public class HLSLRegisterSetAndBinding
{
    /// <summary>
    /// Creates a new instance of <see cref="HLSLRegisterSetAndBinding"/>.
    /// </summary>
    public HLSLRegisterSetAndBinding(string register, string set, string binding)
    {
        Register = register;
        Set = set;
        Binding = binding;
    }

    /// <summary>
    /// Creates a new instance of <see cref="HLSLRegisterSetAndBinding"/>.
    /// </summary>
    public HLSLRegisterSetAndBinding(ShaderKind stage, string register, string set, string binding)
        : this(register, set, binding)
    {
        ShaderStage = stage;
    }

    public ShaderKind? ShaderStage { get; set; }

    /// <summary>
    /// Gets the register.
    /// </summary>
    public string Register { get; }

    /// <summary>
    /// Gets the set.
    /// </summary>
    public string Set { get; }

    /// <summary>
    /// Gets the binding.
    /// </summary>
    public string Binding { get; }
}
