// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Vulkan;

/// <summary>
/// The exception class for errors that occur in vulka.
/// </summary>
public sealed class VkException : Exception
{
    /// <summary>
    /// Gets the result returned by Vulkan.
    /// </summary>
    public VkResult Result { get; }

    /// <summary>
    /// Gets if the result is considered an error.
    /// </summary>
    public bool IsError => Result < 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="VkException" /> class.
    /// </summary>
    public VkException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VkException" /> class.
    /// </summary>
    /// <param name="result">The result code that caused this exception.</param>
    /// <param name="message"></param>
    public VkException(VkResult result, string message = "Vulkan error occured")
        : base($"[{(int)result}] {result} - {message}")
    {
        Result = result;
    }

    public VkException(string message)
        : base(message)
    {
    }

    public VkException(string message, Exception innerException) : base(message, innerException)
    {
    }

}
