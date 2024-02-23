// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.SPIRV.Reflect;

/// <summary>
/// The exception class for errors that occur in SPIRV-Reflect.
/// </summary>
public sealed class SPIRVReflectException : Exception
{
    /// <summary>
    /// Gets the result returned by SPIRV-Cross.
    /// </summary>
    public SpvReflectResult Result { get; }

    /// <summary>
    /// Gets if the result is considered an error.
    /// </summary>
    public bool IsError => Result < 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="SPIRVReflectException" /> class.
    /// </summary>
    public SPIRVReflectException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SPIRVReflectException" /> class.
    /// </summary>
    /// <param name="result">The result code that caused this exception.</param>
    /// <param name="message"></param>
    public SPIRVReflectException(SpvReflectResult result, string message = "SPIRV-Cross error occured")
        : base($"[{(int)result}] {result} - {message}")
    {
        Result = result;
    }

    public SPIRVReflectException(string message)
        : base(message)
    {
    }

    public SPIRVReflectException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

}
