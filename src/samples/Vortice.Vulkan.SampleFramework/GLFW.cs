// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Vortice.Vulkan;

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
    /// Specifies whether the window will be given input focus when <see cref="GLFW.glfwShowWindow"/> is called.
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

public static unsafe partial class GLFW
{
    public const string Library = "glfw3";

    public const int GLFW_TRUE = 1;
    public const int GLFW_FALSE = 0;

    [LibraryImport(Library, EntryPoint = nameof(glfwInit))]
    public static partial int glfwInitPrivate();

    public static bool glfwInit() => glfwInitPrivate() == GLFW_TRUE;

    [LibraryImport(Library)]
    public static partial void glfwTerminate();

    [LibraryImport(Library)]
    public static partial void glfwGetVersion(int* major, int* minor, int* revision);

    [LibraryImport(Library)]
    public static partial delegate* unmanaged<int, sbyte*, void> glfwSetErrorCallback(delegate* unmanaged<int, sbyte*, void> callback);

    [LibraryImport(Library)]
    public static partial void glfwInitHint(int hint, int value);

    [LibraryImport(Library)]
    public static partial void glfwWindowHint(int hint, int value);

    public static void glfwInitHint(InitHintBool hint, bool value) => glfwInitHint((int)hint, value ? GLFW_TRUE : GLFW_FALSE);

    public static void glfwWindowHint(WindowHintBool hint, bool value) => glfwWindowHint((int)hint, value ? GLFW_TRUE : GLFW_FALSE);

    [LibraryImport(Library)]
    private static partial GLFWwindow glfwCreateWindow(int width, int height, byte* title, GLFWmonitor monitor, GLFWwindow share);

    public static GLFWwindow glfwCreateWindow(int width, int height, string title, GLFWmonitor monitor, GLFWwindow share)
    {
        var ptr = Marshal.StringToHGlobalAnsi(title);

        try
        {
            return glfwCreateWindow(width, height, (byte*)ptr, monitor, share);
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
    }

    [LibraryImport(Library, EntryPoint = nameof(glfwWindowShouldClose))]
    public static partial int glfwWindowShouldClosePrivate(GLFWwindow window);

    public static bool glfwWindowShouldClose(GLFWwindow window) => glfwWindowShouldClosePrivate(window) == GLFW_TRUE;

    [LibraryImport(Library)]
    public static partial void glfwGetWindowSize(GLFWwindow window, out int width, out int height);

    [LibraryImport(Library)]
    public static partial void glfwShowWindow(GLFWwindow window);

    [LibraryImport(Library)]
    public static partial GLFWmonitor glfwGetPrimaryMonitor();

    [LibraryImport(Library)]
    public static partial void glfwPollEvents();

    [LibraryImport(Library)]
    public static partial nint glfwGetRequiredInstanceExtensions(out int count);

    public static string[] glfwGetRequiredInstanceExtensions()
    {
        nint ptr = glfwGetRequiredInstanceExtensions(out int count);

        string[] array = new string[count];
        if (count > 0 && ptr != 0)
        {
            var offset = 0;
            for (int i = 0; i < count; i++, offset += IntPtr.Size)
            {
                IntPtr p = Marshal.ReadIntPtr(ptr, offset);
                array[i] = Marshal.PtrToStringAnsi(p)!;
            }
        }

        return array;
    }

    [LibraryImport(Library)]
    public static partial VkResult glfwCreateWindowSurface(VkInstance instance, GLFWwindow window, void* allocator, VkSurfaceKHR* pSurface);

    static GLFW()
    {
        NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), OnDllImport);
    }

    private static IntPtr OnDllImport(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        if (TryResolveLibrary(libraryName, assembly, searchPath, out IntPtr nativeLibrary))
        {
            return nativeLibrary;
        }

        return NativeLibrary.Load(libraryName, assembly, searchPath);
    }

    private static bool TryResolveLibrary(string libraryName, Assembly assembly, DllImportSearchPath? searchPath, out nint nativeLibrary)
    {
        nativeLibrary = IntPtr.Zero;
        if (libraryName is not Library)
            return false;

        string rid = RuntimeInformation.RuntimeIdentifier;

        string nugetNativeLibsPath = Path.Combine(AppContext.BaseDirectory, "runtimes", rid, "native");
        bool isNuGetRuntimeLibrariesDirectoryPresent = Directory.Exists(nugetNativeLibsPath);
        string dllName = Library;

        if (OperatingSystem.IsWindows())
        {
            dllName = $"{Library}.dll";

            if (!isNuGetRuntimeLibrariesDirectoryPresent)
            {
                rid = RuntimeInformation.ProcessArchitecture switch
                {
                    Architecture.X64 => "win-x64",
                    Architecture.Arm64 => "win-arm64",
                    _ => "win-x64"
                };

                nugetNativeLibsPath = Path.Combine(AppContext.BaseDirectory, "runtimes", rid, "native");
                isNuGetRuntimeLibrariesDirectoryPresent = Directory.Exists(nugetNativeLibsPath);
            }
        }
        else if (OperatingSystem.IsLinux())
        {
            dllName = $"libglfw.so.3";
        }
        else if (OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst())
        {
            dllName = $"libglfw.3.dylib";
        }

        if (isNuGetRuntimeLibrariesDirectoryPresent)
        {
            string fullPath = Path.Combine(AppContext.BaseDirectory, "runtimes", rid, "native", dllName);

            if (NativeLibrary.TryLoad(fullPath, out nativeLibrary))
            {
                return true;
            }
        }

        if (NativeLibrary.TryLoad(Library, assembly, searchPath, out nativeLibrary))
        {
            return true;
        }

        nativeLibrary = 0;
        return false;
    }
}

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly partial struct GLFWmonitor(nint handle) : IEquatable<GLFWmonitor>
{
    public nint Handle { get; } = handle; public bool IsNull => Handle == 0;
    public static GLFWmonitor Null => new(0);
    public static implicit operator GLFWmonitor(nint handle) => new(handle);
    public static bool operator ==(GLFWmonitor left, GLFWmonitor right) => left.Handle == right.Handle;
    public static bool operator !=(GLFWmonitor left, GLFWmonitor right) => left.Handle != right.Handle;
    public static bool operator ==(GLFWmonitor left, nint right) => left.Handle == right;
    public static bool operator !=(GLFWmonitor left, nint right) => left.Handle != right;
    public bool Equals(GLFWmonitor other) => Handle == other.Handle;
    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is GLFWmonitor handle && Equals(handle);
    /// <inheritdoc/>
    public override int GetHashCode() => Handle.GetHashCode();
    private string DebuggerDisplay => string.Format("GLFWmonitor [0x{0}]", Handle.ToString("X"));
}

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly partial struct GLFWwindow(nint handle) : IEquatable<GLFWwindow>
{
    public nint Handle { get; } = handle; public bool IsNull => Handle == 0;
    public static GLFWwindow Null => new(0);
    public static implicit operator GLFWwindow(nint handle) => new(handle);
    public static bool operator ==(GLFWwindow left, GLFWwindow right) => left.Handle == right.Handle;
    public static bool operator !=(GLFWwindow left, GLFWwindow right) => left.Handle != right.Handle;
    public static bool operator ==(GLFWwindow left, nint right) => left.Handle == right;
    public static bool operator !=(GLFWwindow left, nint right) => left.Handle != right;
    public bool Equals(GLFWwindow other) => Handle == other.Handle;
    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is GLFWwindow handle && Equals(handle);
    /// <inheritdoc/>
    public override int GetHashCode() => Handle.GetHashCode();
    private string DebuggerDisplay => string.Format("GLFWwindow [0x{0}]", Handle.ToString("X"));
}
