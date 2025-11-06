cmake_minimum_required(VERSION 3.10)
project(BluePrintOne)

set(CMAKE_CXX_STANDARD 17)

add_executable(BluePrintOne WIN32 main.cpp)
target_link_libraries(BluePrintOne comctl32 comdlg32)
