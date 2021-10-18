// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using static Vortice.Vulkan.GLFW;

namespace Vortice.Vulkan
{
    [Flags]
    public enum WindowFlags
    {
        None = 0,
        Fullscreen = 1 << 0,
        FullscreenDesktop = 1 << 1,
        Hidden = 1 << 2,
        Borderless = 1 << 3,
        Resizable = 1 << 4,
        Minimized = 1 << 5,
        Maximized = 1 << 6,
    }

    public sealed unsafe class Window
    {
        private readonly GLFWwindow* _window;

        public unsafe Window(string title, int width, int height, WindowFlags flags = WindowFlags.None)
        {
            Title = title;

            bool fullscreen = false;
            GLFWmonitor* monitor = null;
            if ((flags & WindowFlags.Fullscreen) != WindowFlags.None)
            {
                monitor = glfwGetPrimaryMonitor();
                fullscreen = true;
            }

            if ((flags & WindowFlags.FullscreenDesktop) != WindowFlags.None)
            {
                monitor = glfwGetPrimaryMonitor();
                //auto mode = glfwGetVideoMode(monitor);
                //
                //glfwWindowHint(GLFW_RED_BITS, mode->redBits);
                //glfwWindowHint(GLFW_GREEN_BITS, mode->greenBits);
                //glfwWindowHint(GLFW_BLUE_BITS, mode->blueBits);
                //glfwWindowHint(GLFW_REFRESH_RATE, mode->refreshRate);

                glfwWindowHint(WindowHintBool.Decorated, false);
                fullscreen = true;
            }


            if (!fullscreen)
            {
                if ((flags & WindowFlags.Borderless) != WindowFlags.None)
                {
                    glfwWindowHint(WindowHintBool.Decorated, false);
                }
                else
                {
                    glfwWindowHint(WindowHintBool.Decorated, true);
                }

                if ((flags & WindowFlags.Resizable) != WindowFlags.None)
                {
                    glfwWindowHint(WindowHintBool.Resizable, true);
                }

                if ((flags & WindowFlags.Hidden) != WindowFlags.None)
                {
                    glfwWindowHint(WindowHintBool.Visible, false);
                }

                if ((flags & WindowFlags.Minimized) != WindowFlags.None)
                {
                    glfwWindowHint(WindowHintBool.Iconified, true);
                }

                if ((flags & WindowFlags.Maximized) != WindowFlags.None)
                {
                    glfwWindowHint(WindowHintBool.Maximized, true);
                }
            }

            _window = glfwCreateWindow(width, height, title, monitor, null);
            //Handle = hwnd;

            glfwGetWindowSize(_window, out width, out height);
            Extent = new VkExtent2D(width, height);
        }

        public string Title { get; }
        public VkExtent2D Extent { get; }
        //public IntPtr Handle { get; }

        public bool ShoudClose => glfwWindowShouldClose(_window);

        public VkResult CreateSurface(VkInstance instance, out VkSurfaceKHR surface)
        {
            return glfwCreateWindowSurface(instance, _window, null, out surface);
        }
    }
}
