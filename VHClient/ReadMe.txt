本项目是提供虚拟MCU环境的进程，生成的exe通过传入的参数（匿名管道）与父进程进行交互

打包的虚拟MCU运行一个二进制文件（本质上是一个dll，导出函数SetupLinks用于配置回掉，main函数是入口）

目前有以下参数

-f <filepath>	指定二进制文件的路径

TODO:
-iuart0 <pipe>
-ouart0 <pipe>	指定虚拟串口0的IO匿名管道
-iuart1 <pipe>
-ouart1 <pipe>	指定虚拟串口1的IO匿名管道
-iuart2 <pipe>
-ouart2 <pipe>	指定虚拟串口2的IO匿名管道

-leds <pipe> 指定led匿名输出管道，每次输出2byte代表led数码管编号和值（共阴）