#pragma once

// Minimized version to avoid whole Windows inclusion during generation

#include "vk_platform.h"
#include "vulkan_core.h"

#ifdef VK_USE_PLATFORM_ANDROID_KHR
#include "vulkan_android.h"
#endif

#ifdef VK_USE_PLATFORM_FUCHSIA
#include <zircon/types.h>
#include "vulkan_fuchsia.h"
#endif

#ifdef VK_USE_PLATFORM_IOS_MVK
#include "vulkan_ios.h"
#endif


#ifdef VK_USE_PLATFORM_MACOS_MVK
#include "vulkan_macos.h"
#endif

#ifdef VK_USE_PLATFORM_METAL_EXT
#include "vulkan_metal.h"
#endif

#ifdef VK_USE_PLATFORM_VI_NN
#include "vulkan_vi.h"
#endif

#ifdef VK_USE_PLATFORM_WAYLAND_KHR
#include "vulkan_wayland.h"
#endif

#ifdef VK_USE_PLATFORM_WIN32_KHR

/* When VK_USE_PLATFORM_WIN32_KHR is defined, instead of including vulkan.h directly, we include individual parts of the SDK
 * This is necessary to avoid including <windows.h> which is very heavy - it takes 200ms to parse without WIN32_LEAN_AND_MEAN
 * and 100ms to parse with it. vulkan_win32.h only needs a few symbols that are easy to redefine ourselves.
 */
typedef unsigned long DWORD;
typedef const wchar_t* LPCWSTR;
typedef void* HANDLE;
typedef struct HINSTANCE__* HINSTANCE;
typedef struct HWND__* HWND;
typedef struct HMONITOR__* HMONITOR;
typedef struct _SECURITY_ATTRIBUTES SECURITY_ATTRIBUTES;

#	include "vulkan_win32.h"
#include "vulkan_win32.h"
#endif


#ifdef VK_USE_PLATFORM_XCB_KHR
typedef struct xcb_connection_t xcb_connection_t;
typedef uint32_t xcb_window_t;
typedef uint32_t xcb_visualid_t;
#include "vulkan_xcb.h"
#endif


#ifdef VK_USE_PLATFORM_XLIB_KHR
typedef struct Display Display;
typedef uint32_t Window;
typedef uint64_t VisualID;
#include "vulkan_xlib.h"
#endif


#ifdef VK_USE_PLATFORM_DIRECTFB_EXT
#include <directfb.h>
#include "vulkan_directfb.h"
#endif


#ifdef VK_USE_PLATFORM_XLIB_XRANDR_EXT
#include <X11/Xlib.h>
#include <X11/extensions/Xrandr.h>
#include "vulkan_xlib_xrandr.h"
#endif


#ifdef VK_USE_PLATFORM_GGP
#include <ggp_c/vulkan_types.h>
#include "vulkan_ggp.h"
#endif


#ifdef VK_USE_PLATFORM_SCREEN_QNX
#include <screen/screen.h>
#include "vulkan_screen.h"
#endif


#ifdef VK_USE_PLATFORM_SCI
#include <nvscisync.h>
#include <nvscibuf.h>
#include "vulkan_sci.h"
#endif


#ifdef VK_ENABLE_BETA_EXTENSIONS
#include "vulkan_beta.h"
#endif
