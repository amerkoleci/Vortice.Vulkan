# Define target name
set (TARGET_NAME spirv-reflect)

set(SOURCE_FILES
    spirv_reflect.h
    spirv_reflect.c
)

add_library(${TARGET_NAME} ${SOURCE_FILES})
target_include_directories(${TARGET_NAME} PUBLIC
    $<BUILD_INTERFACE:${CMAKE_CURRENT_SOURCE_DIR}>
    $<INSTALL_INTERFACE:${CMAKE_INSTALL_INCLUDEDIR}/${TARGET_NAME}>
)

if (BUILD_SHARED_LIBS)
    target_compile_definitions(${TARGET_NAME} PRIVATE SPV_EXPORT_SYMBOLS)
endif ()

if (WIN32)
	set_target_properties(${TARGET_NAME} PROPERTIES WINDOWS_EXPORT_ALL_SYMBOLS ON)
endif ()

if (NATIVE_ENABLE_INSTALL)
    install (FILES "spirv_reflect.h" DESTINATION ${CMAKE_INSTALL_INCLUDEDIR}/${TARGET_NAME})

    install(TARGETS ${TARGET_NAME}
        ARCHIVE DESTINATION ${CMAKE_INSTALL_LIBDIR}
        LIBRARY DESTINATION ${CMAKE_INSTALL_LIBDIR}
        RUNTIME DESTINATION ${CMAKE_INSTALL_BINDIR}
    )
endif ()
