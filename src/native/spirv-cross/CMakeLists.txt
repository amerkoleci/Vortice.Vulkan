# Define target name
set (TARGET_NAME spirv-cross)

set(SOURCE_FILES
    ${CMAKE_CURRENT_SOURCE_DIR}/GLSL.std.450.h
	${CMAKE_CURRENT_SOURCE_DIR}/spirv_common.hpp
	${CMAKE_CURRENT_SOURCE_DIR}/spirv_cross_containers.hpp
	${CMAKE_CURRENT_SOURCE_DIR}/spirv_cross_error_handling.hpp
	${CMAKE_CURRENT_SOURCE_DIR}/spirv.hpp
	${CMAKE_CURRENT_SOURCE_DIR}/spirv_cross.hpp
	${CMAKE_CURRENT_SOURCE_DIR}/spirv_cross.cpp
	${CMAKE_CURRENT_SOURCE_DIR}/spirv_parser.hpp
	${CMAKE_CURRENT_SOURCE_DIR}/spirv_parser.cpp
	${CMAKE_CURRENT_SOURCE_DIR}/spirv_cross_parsed_ir.hpp
	${CMAKE_CURRENT_SOURCE_DIR}/spirv_cross_parsed_ir.cpp
	${CMAKE_CURRENT_SOURCE_DIR}/spirv_cfg.hpp
	${CMAKE_CURRENT_SOURCE_DIR}/spirv_cfg.cpp
    ${CMAKE_CURRENT_SOURCE_DIR}/spirv_glsl.hpp
    ${CMAKE_CURRENT_SOURCE_DIR}/spirv_glsl.cpp
    ${CMAKE_CURRENT_SOURCE_DIR}/spirv_hlsl.hpp
    ${CMAKE_CURRENT_SOURCE_DIR}/spirv_hlsl.cpp
    ${CMAKE_CURRENT_SOURCE_DIR}/spirv_msl.hpp
    ${CMAKE_CURRENT_SOURCE_DIR}/spirv_msl.cpp
    ${CMAKE_CURRENT_SOURCE_DIR}/spirv_reflect.hpp
    ${CMAKE_CURRENT_SOURCE_DIR}/spirv_reflect.cpp
    ${CMAKE_CURRENT_SOURCE_DIR}/spirv_cross_c.h
    ${CMAKE_CURRENT_SOURCE_DIR}/spirv_cross_c.cpp
)

add_library(${TARGET_NAME} ${SOURCE_FILES})
target_include_directories(${TARGET_NAME} PUBLIC
    $<BUILD_INTERFACE:${CMAKE_CURRENT_SOURCE_DIR}>
    $<INSTALL_INTERFACE:${CMAKE_INSTALL_INCLUDEDIR}/${TARGET_NAME}>
)

if (BUILD_SHARED_LIBS)
    target_compile_definitions(${TARGET_NAME} PRIVATE SPVC_EXPORT_SYMBOLS)
endif ()

target_compile_definitions(${TARGET_NAME} PRIVATE SPIRV_CROSS_C_API_GLSL=1)
target_compile_definitions(${TARGET_NAME} PRIVATE SPIRV_CROSS_C_API_HLSL=1)
target_compile_definitions(${TARGET_NAME} PRIVATE SPIRV_CROSS_C_API_MSL=1)
target_compile_definitions(${TARGET_NAME} PRIVATE SPIRV_CROSS_C_API_REFLECT=1)

if (NATIVE_ENABLE_INSTALL)
    install (FILES "spirv_cross_c.h" DESTINATION ${CMAKE_INSTALL_INCLUDEDIR}/${TARGET_NAME})

    install(TARGETS ${TARGET_NAME}
        ARCHIVE DESTINATION ${CMAKE_INSTALL_LIBDIR}
        LIBRARY DESTINATION ${CMAKE_INSTALL_LIBDIR}
        RUNTIME DESTINATION ${CMAKE_INSTALL_BINDIR}
    )
endif()
