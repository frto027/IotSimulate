# 这是啥？

本来是做一个可以仿真一些物联网设备的软件，可以通过串口和真实设备产生互动，目前只做了软件底层的构架，包括界面绘制、传感器接口抽象、以及使用C语言开发单片机程序的接口和环境等。

# 乱七八糟的文档
[这个链接](https://raw.githubusercontent.com/frto027/IotSimulate/master/files/report.docx)是一个能体现软件构架的文档，是之前在学校提交的文档的一部分。

这个工程是自己编程的一个练习

# 环境
VS2017 WPF，单片机的模拟是C++编写的，需要用到C++。模拟的只有api，不是指令集的模拟。

# 能干啥
实际上并不能干啥

## 模拟一些串口
这些主要还是用来调试的。
- **虚拟串口** 上面有一个按钮，每按一下发送一个字节，依次递增
- **虚拟数码管** 可以显示串口输入的最后一个字节
- **真实串口** 可以和真实的串口产生互动，直接把它当作对应串口的程序就可以了

## 模拟环境
还是用来调试的。
- **用户环境** 圆圈内的传感器受到环境影响
- **串口温度传感器** 以很短的周期通过串口发送当前环境温度(数据长度1字节)

## 模拟无线网络
简单的无线网络模拟

只模拟了简单的设备，下面两个设备配合在一起就足以组建超复杂的无线网了(虽然现实中不一定会出现这种奇葩的网络)
- **WLHost串口终端** 串口收到的数据会广播到无线网络所有设备，无线网络中某个WL串口终端发送的数据会单独从串口发送
- **WL串口终端** 串口收到的数据会广播到无线网络，无线网络收到的数据会转发到串口输出

以及还有两个DEBUG的带校验位的封包收发，可以用来和**单片机**相关的API一起调试。

## VTM开发板

VTM开发板通过子进程的方式将C语言代码编译成x86的api，C语言的函数接口是通过唯一的一个VHPipe.dll提供的，因此不包含标准库函数等，只能使用提供的库函数。每个开发板都有独立的进程，相互隔离。
- 使用之前需要处理依赖，手动编译`VHProgress/VHPipe`项目，否则无法成功加载二进制文件，如果出现sdk版本不行，自行修改VHPipe项目属性中的Windows SDK版本号。

### HAL层定义
直接看头文件吧，在`VTMLib\hal\include`
### 样例
样例位于`VTMLib\Project`，`output`目录中有一些编译好可以测试的二进制文件
### 手动编译开发板程序
手动编译需要mingw的开发环境，相关文件暂时没有在git上传，抱歉。
### 运行错误日志
开发板通过异步开启VHClient进程、使用进程管道通信、并通过VHPipe.dll中封装的函数与C语言的环境进行通讯，流程比较复杂。错误日志统一写在软件目录的`VHClientError.log`文件中。
