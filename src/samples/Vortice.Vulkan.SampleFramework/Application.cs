// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.InteropServices;
using static Vortice.Vulkan.GLFW;

namespace Vortice.Vulkan;

public abstract class Application : IDisposable
{
    private static readonly unsafe glfwErrorCallback s_errorCallback = GlfwError;
    private bool _closeRequested = false;

    protected Application()
    {
        if (!glfwInit())
        {
            throw new PlatformNotSupportedException("GLFW is not supported");
        }

        glfwSetErrorCallback(s_errorCallback);

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            glfwInitHint(InitHintBool.CocoaChDirResources, false);
        }

        glfwWindowHint((int)WindowHintClientApi.ClientApi, 0);

        // Create main window.
        MainWindow = new Window(Name, 1280, 720);
    }

    public abstract string Name { get; }

    public Window MainWindow { get; }

    public virtual void Dispose()
    {
    }

    protected virtual void Initialize()
    {

    }

    protected virtual void OnTick()
    {
    }

    public void Run()
    {
        Initialize();

        while (!MainWindow.ShoudClose &&
            !_closeRequested)
        {
            OnTick();
            glfwPollEvents();
        }
    }

    protected virtual void OnDraw(int width, int height)
    {

    }

    private static unsafe void GlfwError(int code, sbyte* message)
    {
        throw new Exception(new string(message));
    }
}
