// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using SDL;
using static SDL.SDL3;

namespace Vortice.Vulkan;

[Flags]
public enum WindowFlags
{
    None = 0,
    Fullscreen = 1 << 0,
    Borderless = 1 << 1,
    Resizable = 1 << 2,
    Minimized = 1 << 3,
    Maximized = 1 << 4,
}

public sealed unsafe class Window
{
    private readonly SDL_Window* _window;

    public unsafe Window(string title, int width, int height, WindowFlags flags = WindowFlags.None)
    {
        Title = title;

        ulong sdl_flags = SDL_WINDOW_HIGH_PIXEL_DENSITY | SDL_WINDOW_VULKAN | SDL_WINDOW_HIDDEN;
        if ((flags & WindowFlags.Fullscreen) != WindowFlags.None)
        {
            sdl_flags |= SDL_WINDOW_FULLSCREEN;
        }
        else
        {
            if ((flags & WindowFlags.Borderless) != WindowFlags.None)
            {
                sdl_flags |= SDL_WINDOW_BORDERLESS;
            }

            if ((flags & WindowFlags.Resizable) != WindowFlags.None)
            {
                sdl_flags |= SDL_WINDOW_RESIZABLE;
            }

            if ((flags & WindowFlags.Minimized) != WindowFlags.None)
            {
                sdl_flags |= SDL_WINDOW_MINIMIZED;
            }

            if ((flags & WindowFlags.Maximized) != WindowFlags.None)
            {
                sdl_flags |= SDL_WINDOW_MAXIMIZED;
            }
        }

        _window = SDL_CreateWindow(title, width, height, (SDL_WindowFlags)sdl_flags);
        if (_window == null)
        {
            throw new Exception("SDL: failed to create window");
        }

        _ = SDL_SetWindowPosition(_window, (int)SDL_WINDOWPOS_CENTERED, (int)SDL_WINDOWPOS_CENTERED);
        Id = SDL_GetWindowID(_window);
    }

    public string Title { get; }

    public SDL_WindowID Id { get; }
    public VkExtent2D Extent
    {
        get
        {
            int width, height;
            SDL_GetWindowSize(_window, &width, &height);
            return new(width, height);
        }
    }

    public void Show()
    {
        _ = SDL_ShowWindow(_window);
    }

    public VkSurfaceKHR CreateSurface(VkInstance instance)
    {
        VkSurfaceKHR_T* surface = default;
        if (!SDL_Vulkan_CreateSurface(_window, (VkInstance_T*)instance.Handle, null, &surface))
        {
            throw new Exception("SDL: failed to create vulkan surface");
        }

        return new VkSurfaceKHR((ulong)new IntPtr(surface).ToInt64());
    }
}
