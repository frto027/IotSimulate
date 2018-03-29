// VHPipe.cpp: 定义 DLL 应用程序的导出函数。
//

#include "stdafx.h"

//defines
#define DLL_API __declspec(dllexport)
//A.将需要用C#编写的函数加入这个枚举（作为id），需要和C#中的Callback保持一致，理论不超过2^7
enum {
	CB_USART_READ,
	CB_USART_WRITE,
	CB_COUNT
};
//存放函数指针
void * VHPipe_Callbacks[CB_COUNT];
//测试某id是否有效
#define CheckCallBack(id) ((id)>=0 && (id)<CB_COUNT && VHPipe_Callbacks[id])
//根据type（函数指针类型），id（上面的枚举），某参数调用Callbacks中的函数
#define Relocate(id,type,...) ((type)VHPipe_Callbacks[id])(__VA_ARGS__)
//由C#代码调用，传入函数指针（C#委托），用于之后被调用，这个函数通过DLL PE汇编直接被列入目标dll的导出表中，不会也不应该被C++代码调用
DLL_API void VHPipe_SetupLinks(INT8 id, void * CallBack) {
	if (id < CB_COUNT && id >= 0)
		VHPipe_Callbacks[id] = CallBack;
}

//B.将需要用C#编写的函数在下面声明，用Relocate重定向，这些函数会出现在VHPipe.dll，但不会出现在目标dll的导出表
//B.注意：如果C#中的委托签名与type不一致（包括Cdecl声明），则程序跑飞！

//这些函数是HAL层、OSAL层的具体实现，真正的函数体在C#中编写（也可以在这里直接编写，可以用c标准库不会污染环境，但也需要谨慎使用）
//需要在目标dll编译的头文件中用__declspec(dllimport)声明，同时编译时链接VHPipe.dll

/*
//sample
//声明类型
typedef int(readUSART_TYPE)();
//声明函数(导出，会被C++调用)
DLL_API int readUSART() {
	if (CheckCallBack(CB_USART_READ))
		return Relocate(CB_USART_READ, readUSART_TYPE *, );//重定向
	else
		return -1;//默认操作（当C#没有指定Callback时的操作）
}
//another sample
typedef void (writeUSART_TYPE)(int);
DLL_API void writeUSART(int ch) {
	if (CheckCallBack(CB_USART_WRITE))
		Relocate(CB_USART_WRITE, writeUSART_TYPE *, ch);
}
*/