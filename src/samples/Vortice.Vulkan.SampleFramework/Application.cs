// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.InteropServices;
using SDL;
using static SDL.SDL3;
using static SDL.SDL_LogPriority;
using static SDL.SDL_EventType;
using System.Runtime.CompilerServices;

namespace Vortice.Vulkan;

public abstract class Application : IDisposable
{
    private bool _closeRequested = false;

    protected unsafe Application()
    {
        if (!SDL_Init(SDL_InitFlags.SDL_INIT_VIDEO))
        {
            throw new PlatformNotSupportedException("SDL is not supported");
        }

        SDL_SetLogOutputFunction(&Log_SDL, 0);

        if (!SDL_Vulkan_LoadLibrary((byte*)null))
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
                if (evt.type == (uint)SDL_EVENT_QUIT)
                {
                    running = false;
                    break;
                }

                if (evt.type == (uint)SDL_EVENT_WINDOW_CLOSE_REQUESTED && evt.window.windowID == MainWindow.Id)
                {
                    running = false;
                    break;
                }
                else if (evt.type >= (uint)SDL_EVENT_WINDOW_FIRST
                    && evt.type <= (uint)SDL_EVENT_WINDOW_LAST)
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
            case SDL_EVENT_WINDOW_RESIZED:
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

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static unsafe void Log_SDL(nint _,int categoryInt, SDL_LogPriority priority, byte* messagePtr)
    {
        string? message = PtrToStringUTF8(messagePtr);
        SDL_LogCategory category = (SDL_LogCategory)categoryInt;
        if (priority >= SDL_LOG_PRIORITY_ERROR)
        {
            Log.Error($"[{priority}] SDL: {message}");
            throw new Exception(message);
        }
        else
        {
            Log.Info($"[{priority}] SDL: {message}");
        }
    }
}
