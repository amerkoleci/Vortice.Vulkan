// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.SpirvCross;

/// <summary>
/// The exception class for errors that occur in SPIRV-Cross.
/// </summary>
public sealed class SpirvCrossException : Exception
{
    /// <summary>
    /// Gets the result returned by SPIRV-Cross.
    /// </summary>
    public spvc_result Result { get; }

    /// <summary>
    /// Gets if the result is considered an error.
    /// </summary>
    public bool IsError => Result < 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpirvCrossException" /> class.
    /// </summary>
    public SpirvCrossException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpirvCrossException" /> class.
    /// </summary>
    /// <param name="result">The result code that caused this exception.</param>
    /// <param name="message"></param>
    public SpirvCrossException(spvc_result result, string message = "SPIRV-Cross error occured")
        : base($"[{(int)result}] {result} - {message}")
    {
        Result = result;
    }

    public SpirvCrossException(string message)
        : base(message)
    {
    }

    public SpirvCrossException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

}
