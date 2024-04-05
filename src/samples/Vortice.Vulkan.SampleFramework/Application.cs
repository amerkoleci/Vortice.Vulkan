// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.InteropServices;
using SDL;
using static SDL.SDL;

namespace Vortice.Vulkan;

public abstract class Application : IDisposable
{
    private bool _closeRequested = false;

    protected unsafe Application()
    {
        if (SDL_Init(SDL_InitFlags.Video) != 0)
        {
            throw new PlatformNotSupportedException("SDL is not supported");
        }

        SDL_SetLogOutputFunction(Log_SDL);

        if (SDL_Vulkan_LoadLibrary() < 0)
        {
            throw new PlatformNotSupportedException("SDL: Failed to init vulkan");
        }

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

    public unsafe void Run()
    {
        Initialize();
        MainWindow.Show();

        bool running = true;

        while (running && !_closeRequested)
        {
            SDL_Event evt;
            while (SDL_PollEvent(&evt))
            {
                if (evt.type == SDL_EventType.Quit)
                {
                    running = false;
                    break;
                }

                if (evt.type == SDL_EventType.WindowCloseRequested && evt.window.windowID == MainWindow.Id)
                {
                    running = false;
                    break;
                }
                else if (evt.type >= SDL_EventType.WindowFirst && evt.type <= SDL_EventType.WindowLast)
                {
                    HandleWindowEvent(evt);
                }
            }

            if (!running)
                break;

            OnTick();
        }
    }

    protected virtual void OnDraw(int width, int height)
    {

    }

    private void HandleWindowEvent(in SDL_Event evt)
    {
        switch (evt.window.type)
        {
            case SDL_EventType.WindowResized:
                //_minimized = false;
                HandleResize(evt);
                break;
        }
    }

    private void HandleResize(in SDL_Event evt)
    {
        //if (MainWindow.ClientSize.width != evt.window.data1 ||
        //    MainWindow.ClientSize.height != evt.window.data2)
        {
            //_graphicsDevice.Resize((uint)evt.window.data1, (uint)evt.window.data2);
            //OnSizeChanged(evt.window.data1, evt.window.data2);
        }
    }

    //[UnmanagedCallersOnly]
    private static void Log_SDL(SDL_LogCategory category, SDL_LogPriority priority, string description)
    {
        if (priority >= SDL_LogPriority.Error)
        {
            Log.Error($"[{priority}] SDL: {description}");
            throw new Exception(description);
        }
        else
        {
            Log.Info($"[{priority}] SDL: {description}");
        }
    }
}
