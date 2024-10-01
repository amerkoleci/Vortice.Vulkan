// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.ShaderCompiler;

/// <summary>
/// Defines a shader macro.
/// </summary>
public class ShaderMacro
{
    /// <summary>
    /// Creates a new instance of <see cref="ShaderMacro"/>.
    /// </summary>
    /// <param name="key">The key of the macro.</param>
    /// <param name="value">The optional value.</param>
    public ShaderMacro(string key, string? value)
    {
        Key = key;
        Value = value;
    }

    /// <summary>
    /// Gets the key of the macro.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Gets the optional value of the macro.
    /// </summary>
    public string? Value { get; }
}
