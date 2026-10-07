# Define target name
set (TARGET_NAME vma)

add_library(${TARGET_NAME} 
    include/vk_mem_alloc.h
    src/vk_mem_alloc.cpp
)
target_include_directories(${TARGET_NAME} PUBLIC
    $<BUILD_INTERFACE:${CMAKE_CURRENT_SOURCE_DIR}>/include
    $<INSTALL_INTERFACE:${CMAKE_INSTALL_INCLUDEDIR}/${TARGET_NAME}>
)
target_link_libraries(${TARGET_NAME} PRIVATE vulkan)

if (BUILD_SHARED_LIBS)
    if(WIN32)
        # VMA decorates every public function with VMA_CALL_PRE
        target_compile_definitions(${TARGET_NAME} PRIVATE "VMA_CALL_PRE=__declspec(dllexport)")
    else()
        # Everything else stays internal, so the exported surface is the C API and nothing from the C++ implementation.
        set_target_properties(${TARGET_NAME} PROPERTIES
            CXX_VISIBILITY_PRESET hidden
            VISIBILITY_INLINES_HIDDEN ON
        )
        target_compile_definitions(${TARGET_NAME} PRIVATE "VMA_CALL_PRE=__attribute__((visibility(\"default\")))")
    endif ()
endif ()

if (NATIVE_ENABLE_INSTALL)
    install (FILES "include/vk_mem_alloc.h" DESTINATION ${CMAKE_INSTALL_INCLUDEDIR}/${TARGET_NAME})

    install(TARGETS ${TARGET_NAME}
        ARCHIVE DESTINATION ${CMAKE_INSTALL_LIBDIR}
        LIBRARY DESTINATION ${CMAKE_INSTALL_LIBDIR}
        RUNTIME DESTINATION ${CMAKE_INSTALL_BINDIR}
    )
endif ()
