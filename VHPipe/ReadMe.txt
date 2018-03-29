本项目编译DLL，用于提供编译二进制的运行时环境

mcu二进制编译时与硬件交互的全部函数均在此DLL中定义（理论上不使用c标准库）
二进制编译时需要动态链接此dll，导入DLL中的相关函数作为硬件驱动，将SetupLinks直接转发至导出表

编译指令：
gcc -shared xxx.c -L[Path of VHPipe.dll] -lVHPipe -o xxx.bin