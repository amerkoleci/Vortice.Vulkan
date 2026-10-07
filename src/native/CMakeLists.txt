cmake_minimum_required(VERSION 3.28 FATAL_ERROR)

if (${CMAKE_SOURCE_DIR} STREQUAL ${CMAKE_BINARY_DIR})
	message(FATAL_ERROR "In-source builds are not supported. Please choose a different binary directory.")
	return()
endif()

# Determine if engine is built as a subproject (using add_subdirectory)
# or if it is the master project.
if (NOT DEFINED NATIVE_MASTER_PROJECT)
    set(NATIVE_MASTER_PROJECT OFF)
    if (CMAKE_CURRENT_SOURCE_DIR STREQUAL CMAKE_SOURCE_DIR)
        set(NATIVE_MASTER_PROJECT ON)
        message(STATUS "CMake version: ${CMAKE_VERSION}")
    endif ()
endif ()

# Use solution folders to organize projects
set_property(GLOBAL PROPERTY USE_FOLDERS ON)

# Configure CMake global variables
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)
set(CMAKE_INSTALL_MESSAGE LAZY)
set(CMAKE_POSITION_INDEPENDENT_CODE ON)
set(CMAKE_COLOR_DIAGNOSTICS ON)
set(CMAKE_DEBUG_POSTFIX "_d")

set(CMAKE_OSX_DEPLOYMENT_TARGET "11" CACHE STRING "Minimum macOS deployment version")
set(CMAKE_OSX_ARCHITECTURES "x86_64;arm64" CACHE STRING "Build architectures for OS X")

set(CMAKE_RUNTIME_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/bin")
set(CMAKE_ARCHIVE_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/lib")
set(CMAKE_LIBRARY_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/lib")

# Project declaration
project(native_libs LANGUAGES C CXX)

if (MSVC AND NATIVE_MASTER_PROJECT)
    if (NOT DEFINED CMAKE_MSVC_RUNTIME_LIBRARY)
        set (CMAKE_MSVC_RUNTIME_LIBRARY "MultiThreaded$<$<CONFIG:Debug>:Debug>")
    endif ()
endif ()

# Set the default CMAKE_BUILD_TYPE to Release.
# This should be done before the project command since the latter can set
# CMAKE_BUILD_TYPE itself (it does so for nmake).
if (NATIVE_MASTER_PROJECT AND NOT CMAKE_BUILD_TYPE)
    set(CMAKE_BUILD_TYPE "Release" CACHE STRING "The default build type" FORCE)
    set_property(CACHE CMAKE_BUILD_TYPE PROPERTY STRINGS ${CMAKE_CONFIGURATION_TYPES})
endif ()

# Add the cmake directory to the module path.
list(APPEND CMAKE_MODULE_PATH "${CMAKE_CURRENT_SOURCE_DIR}/cmake")

# Include common utilities
include(GNUInstallDirs)
include(CMakeDependentOption)
include(CPM)

if (IOS OR EMSCRIPTEN)
    set(BUILD_SHARED_LIBS OFF CACHE BOOL "Disable shared library on (IOS, WEB)" FORCE)
elseif(ANDROID)
    set(BUILD_SHARED_LIBS ON CACHE BOOL "Always enable shared library on (ANDROID)" FORCE)    
else()
    option(BUILD_SHARED_LIBS "Build shared library" ON)
endif ()

option(NATIVE_ENABLE_INSTALL "Generate the install target" ${NATIVE_MASTER_PROJECT})

if (NOT EMSCRIPTEN)
    # vulkan
    add_library(vulkan INTERFACE)
    set(VULKAN_INCLUDE_DIR ${CMAKE_CURRENT_SOURCE_DIR}/vulkan/include)
    target_sources(vulkan INTERFACE 
        ${VULKAN_INCLUDE_DIR}/vulkan/vulkan.h
    )
    target_include_directories(vulkan SYSTEM INTERFACE ${VULKAN_INCLUDE_DIR})
    
    target_compile_definitions (vulkan INTERFACE
        $<$<BOOL:${WIN32}>:
            VK_USE_PLATFORM_WIN32_KHR
        >
        $<$<BOOL:${APPLE}>:
            VK_USE_PLATFORM_METAL_EXT;VK_ENABLE_BETA_EXTENSIONS
        >
        $<$<BOOL:${ANDROID}>:
            VK_USE_PLATFORM_ANDROID_KHR 
        >
    )

    # vma
    add_subdirectory(vma)
endif()
