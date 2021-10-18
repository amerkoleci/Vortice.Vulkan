// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Vortice.Vulkan
{
    public enum InitHintBool
    {
        /// <summary>
        /// Joystick hat buttons init hint.
        /// </summary>
        JoystickHatButtons = 0x00050001,
        /// <summary>
        /// macOS specific init hint.
        /// </summary>
        CocoaChDirResources = 0x00051001,
        /// <summary>
        /// macOS specific init hint.
        /// </summary>
        CocoaMenuBar = 0x00051002,
        /// <summary>
        /// X11 specific init hint.
        /// </summary>
        X11XcbVulkanSurface = 0x00052001
    }

    public enum WindowHintClientApi
    {
        ClientApi = 0x00022001,
    }

    /// <summary>
    /// Context related boolean attributes.
    /// </summary>
    public enum WindowHintBool
    {
        /// <summary>
        /// Indicates whether the specified window has input focus.
        /// Initial input focus is controlled by the window hint with the same name
        /// </summary>
        Focused = 0x00020001,

        /// <summary>
        /// Indicates whether the specified window is iconified,
        /// whether by the user or with <see cref="GLFW.IconifyWindow"/>.
        /// </summary>
        Iconified = 0x00020002,

        /// <summary>
        /// Indicates whether the specified window is resizable by the user.
        /// This is set on creation with the window hint with the same name.
        /// </summary>
        Resizable = 0x00020003,

        /// <summary>
        /// Indicates whether the specified window is visible.
        /// Window visibility can be controlled with <see cref="GLFW.ShowWindow"/> and <see cref="GLFW.HideWindow"/>
        /// and initial visibility is controlled by the window hint with the same name.
        /// </summary>
        Visible = 0x00020004,

        /// <summary>
        /// Indicates whether the specified window has decorations such as a border,a close widget, etc.
        /// This is set on creation with the window hint with the same name.
        /// </summary>
        Decorated = 0x00020005,

        /// <summary>
        /// Specifies whether the full screen window will automatically iconify and restore
        /// the previous video mode on input focus loss.
        /// Possible values are <c>true</c> and <c>false</c>. This hint is ignored for windowed mode windows.
        /// </summary>
        AutoIconify = 0x00020006,

        /// <summary>
        /// Indicates whether the specified window is floating, also called topmost or always-on-top.
        /// This is controlled by the window hint with the same name.
        /// </summary>
        Floating = 0x00020007,

        /// <summary>
        /// Indicates whether the specified window is maximized,
        /// whether by the user or with <see cref="GLFW.MaximizeWindow"/>.
        /// </summary>
        Maximized = 0x00020008,

        /// <summary>
        /// Specifies whether the cursor should be centered over newly created full screen windows.
        /// Possible values are <c>true</c> and <c>false</c>. This hint is ignored for windowed mode windows.
        /// </summary>
        CenterCursor = 0x00020009,

        /// <summary>
        /// Specifies whether the window framebuffer will be transparent.
        /// If enabled and supported by the system, the window framebuffer alpha channel will be used
        /// to combine the framebuffer with the background.
        /// This does not affect window decorations. Possible values are <c>true</c> and <c>false</c>.
        /// </summary>
        TransparentFramebuffer = 0x0002000A,

        /// <summary>
        /// Indicates whether the cursor is currently directly over the client area of the window,
        /// with no other windows between.
        /// See <a href="https://www.glfw.org/docs/3.3/input_guide.html#cursor_enter">Cursor enter/leave events</a>
        /// for details.
        /// </summary>
        Hovered = 0x0002000B,

        /// <summary>
        /// Specifies whether the window will be given input focus when <see cref="GLFW.ShowWindow"/> is called.
        /// Possible values are <c>true</c> and <c>false</c>.
        /// </summary>
        FocusOnShow = 0x0002000C,

        /// <summary>
        /// Specifies whether the window is transparent to mouse input,
        /// letting any mouse events pass through to whatever window is behind it.
        /// Possible values are <c>true</c> and <c>false</c>.
        /// </summary>
        /// <remarks>
        /// This is only supported for undecorated windows.
        /// Decorated windows with this enabled will behave differently between platforms.
        /// </remarks>
        MousePassthrough = 0x0002000D,

        /// <summary>
        /// Specifies whether the window's context is an OpenGL forward-compatible one.
        /// Possible values are <c>true</c> and <c>false</c>.
        /// </summary>
        OpenGLForwardCompat = 0x00022006,

        /// <summary>
        /// Specifies whether the window's context is an OpenGL debug context.
        /// Possible values are <c>true</c> and <c>false</c>.
        /// </summary>
        OpenGLDebugContext = 0x00022007,

        /// <summary>
        /// Specifies whether errors should be generated by the context.
        /// If enabled, situations that would have generated errors instead cause undefined behavior.
        /// </summary>
        ContextNoError = 0x0002200A,

        /// <summary>
        /// Specifies whether to use stereoscopic rendering. This is a hard constraint.
        /// </summary>
        Stereo = 0x0002100C,

        /// <summary>
        /// Specifies whether the framebuffer should be double buffered.
        /// You nearly always want to use double buffering. This is a hard constraint.
        /// </summary>
        DoubleBuffer = 0x00021010,

        /// <summary>
        /// Specifies whether the framebuffer should be sRGB capable.
        /// If supported, a created OpenGL context will support the
        /// <c>GL_FRAMEBUFFER_SRGB</c> enable( also called <c>GL_FRAMEBUFFER_SRGB_EXT</c>)
        /// for controlling sRGB rendering and a created OpenGL ES context will always have sRGB rendering enabled.
        /// </summary>
        SrgbCapable = 0x0002100E,
    }

    public static unsafe class GLFW
    {
        public const int GLFW_TRUE = 1;
        public const int GLFW_FALSE = 0;

        private static readonly IntPtr s_library = LoadGLFWLibrary();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int glfwInit_t();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void glfwTerminate_t();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void glfwInitHint_t(int hint, int value);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void glfwGetVersion_t(int* major, int* minor, int* revision);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate glfwErrorCallback glfwSetErrorCallback_t(glfwErrorCallback callback);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void glfwErrorCallback(int code, IntPtr message);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate GLFWwindow* glfwCreateWindow_t(int width, int height, byte* title, GLFWmonitor* monitor, GLFWwindow* share);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate GLFWmonitor* glfwGetPrimaryMonitor_t();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int glfwWindowShouldClose_t(GLFWwindow* window);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void glfwGetWindowSize_t(GLFWwindow* window, out int width, out int height);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void glfwPollEvents_t();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate byte** glfwGetRequiredInstanceExtensions_t(out int count);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int glfwCreateWindowSurface_t(VkInstance instance, GLFWwindow* window, void* allocator, out VkSurfaceKHR surface);

        private static readonly glfwInit_t s_glfwInit;
        private static readonly glfwTerminate_t s_glfwTerminate;
        private static readonly glfwInitHint_t s_glfwInitHint;
        private static readonly glfwGetVersion_t s_glfwGetVersion;
        private static readonly glfwSetErrorCallback_t s_glfwSetErrorCallback;

        private static readonly glfwInitHint_t s_glfwWindowHint;
        private static readonly glfwCreateWindow_t s_glfwCreateWindow;
        private static readonly glfwWindowShouldClose_t s_glfwWindowShouldClose;
        private static readonly glfwGetWindowSize_t s_glfwGetWindowSize;
        private static readonly glfwGetPrimaryMonitor_t s_glfwGetPrimaryMonitor;
        private static readonly glfwPollEvents_t s_glfwPollEvents;
        private static readonly glfwGetRequiredInstanceExtensions_t s_glfwGetRequiredInstanceExtensions;
        private static readonly glfwCreateWindowSurface_t s_glfwCreateWindowSurface;

        public static bool glfwInit() => s_glfwInit() == GLFW_TRUE;
        public static void glfwTerminate() => s_glfwTerminate();

        public static void glfwGetVersion(int* major, int* minor, int* revision) => s_glfwGetVersion(major, minor, revision);
        public static glfwErrorCallback glfwSetErrorCallback(glfwErrorCallback callback) => s_glfwSetErrorCallback(callback);

        public static void glfwInitHint(InitHintBool hint, bool value) => s_glfwInitHint((int)hint, value ? GLFW_TRUE : GLFW_FALSE);

        public static void glfwWindowHint(int hint, int value) => s_glfwWindowHint(hint, value);

        public static void glfwWindowHint(WindowHintBool hint, bool value) => s_glfwWindowHint((int)hint, value ? GLFW_TRUE : GLFW_FALSE);


        public static GLFWwindow* glfwCreateWindow(int width, int height, string title, GLFWmonitor* monitor, GLFWwindow* share)
        {
            var ptr = Marshal.StringToHGlobalAnsi(title);

            try
            {
                return s_glfwCreateWindow(width, height, (byte*)ptr, monitor, share);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }
        public static bool glfwWindowShouldClose(GLFWwindow* window) => s_glfwWindowShouldClose(window) == GLFW_TRUE;

        public static void glfwGetWindowSize(GLFWwindow* window, out int width, out int height) => s_glfwGetWindowSize(window, out width, out height);

        public static GLFWmonitor* glfwGetPrimaryMonitor() => s_glfwGetPrimaryMonitor();


        public static void glfwPollEvents() => s_glfwPollEvents();

        public static byte** glfwGetRequiredInstanceExtensions(out int count) => s_glfwGetRequiredInstanceExtensions(out count);

        public static string[] glfwGetRequiredInstanceExtensions()
        {
            var ptr = s_glfwGetRequiredInstanceExtensions(out int count);

            var array = new string[count];
            for (var i = 0; i < count; i++)
            {
                array[i] = Interop.GetString(ptr[i]);
            }

            return array;
        }

        public static VkResult glfwCreateWindowSurface(VkInstance instance, GLFWwindow* window, void* allocator, out VkSurfaceKHR surface)
        {
            return (VkResult)s_glfwCreateWindowSurface(instance, window, allocator, out surface);
        }

        static GLFW()
        {
            s_library = LoadGLFWLibrary();

            s_glfwInit = LoadFunction<glfwInit_t>(nameof(glfwInit));
            s_glfwTerminate = LoadFunction<glfwTerminate_t>(nameof(glfwTerminate));
            s_glfwInitHint = LoadFunction<glfwInitHint_t>(nameof(glfwInitHint));
            s_glfwGetVersion = LoadFunction<glfwGetVersion_t>(nameof(glfwGetVersion));
            s_glfwSetErrorCallback = LoadFunction<glfwSetErrorCallback_t>(nameof(glfwSetErrorCallback));

            s_glfwWindowHint = LoadFunction<glfwInitHint_t>(nameof(glfwWindowHint));
            s_glfwCreateWindow = LoadFunction<glfwCreateWindow_t>(nameof(glfwCreateWindow));
            s_glfwGetPrimaryMonitor = LoadFunction<glfwGetPrimaryMonitor_t>(nameof(glfwGetPrimaryMonitor));
            s_glfwWindowShouldClose = LoadFunction<glfwWindowShouldClose_t>(nameof(glfwWindowShouldClose));
            s_glfwGetWindowSize = LoadFunction<glfwGetWindowSize_t>(nameof(glfwGetWindowSize));

            s_glfwPollEvents = LoadFunction<glfwPollEvents_t>(nameof(glfwPollEvents));

            // Vulkan
            s_glfwGetRequiredInstanceExtensions = LoadFunction<glfwGetRequiredInstanceExtensions_t>(nameof(glfwGetRequiredInstanceExtensions));
            s_glfwCreateWindowSurface = LoadFunction<glfwCreateWindowSurface_t>(nameof(glfwCreateWindowSurface));
        }

        private static IntPtr LoadGLFWLibrary()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return LibraryLoader.LoadLibrary("glfw3.dll");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return LibraryLoader.LoadLibrary("libglfw.so.3.3");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return LibraryLoader.LoadLibrary("libglfw.3.dylib");
            }

            throw new PlatformNotSupportedException("GLFW platform not supported");
        }

        public static T LoadFunction<T>(string name) => LibraryLoader.LoadFunction<T>(s_library, name);
    }

    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly partial struct GLFWmonitor : IEquatable<GLFWmonitor>
    {
        public GLFWmonitor(nint handle) { Handle = handle; }
        public nint Handle { get; }
        public bool IsNull => Handle == 0;
        public static VkDevice Null => new(0);
        public static implicit operator GLFWmonitor(nint handle) => new(handle);
        public static bool operator ==(GLFWmonitor left, GLFWmonitor right) => left.Handle == right.Handle;
        public static bool operator !=(GLFWmonitor left, GLFWmonitor right) => left.Handle != right.Handle;
        public static bool operator ==(GLFWmonitor left, nint right) => left.Handle == right;
        public static bool operator !=(GLFWmonitor left, nint right) => left.Handle != right;
        public bool Equals(GLFWmonitor other) => Handle == other.Handle;
        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is GLFWmonitor handle && Equals(handle);
        /// <inheritdoc/>
        public override int GetHashCode() => Handle.GetHashCode();
        private string DebuggerDisplay => string.Format("GLFWmonitor [0x{0}]", Handle.ToString("X"));
    }

    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly partial struct GLFWwindow : IEquatable<GLFWwindow>
    {
        public GLFWwindow(nint handle) { Handle = handle; }
        public nint Handle { get; }
        public bool IsNull => Handle == 0;
        public static VkDevice Null => new(0);
        public static implicit operator GLFWwindow(nint handle) => new(handle);
        public static bool operator ==(GLFWwindow left, GLFWwindow right) => left.Handle == right.Handle;
        public static bool operator !=(GLFWwindow left, GLFWwindow right) => left.Handle != right.Handle;
        public static bool operator ==(GLFWwindow left, nint right) => left.Handle == right;
        public static bool operator !=(GLFWwindow left, nint right) => left.Handle != right;
        public bool Equals(GLFWwindow other) => Handle == other.Handle;
        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is GLFWwindow handle && Equals(handle);
        /// <inheritdoc/>
        public override int GetHashCode() => Handle.GetHashCode();
        private string DebuggerDisplay => string.Format("GLFWwindow [0x{0}]", Handle.ToString("X"));
    }
}
