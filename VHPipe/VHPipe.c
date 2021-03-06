// VHPipe.cpp: 定义 DLL 应用程序的导出函数。
//

#include "stdafx.h"

//defines
#define DLL_API __declspec(dllexport)
//A.将需要用C#编写的函数加入这个枚举（作为id），需要和C#中的Callback保持一致，理论不超过2^7
enum {
	CB_UART_READ,
	CB_UART_WRITE,
	CB_LEDSET,
	CB_DO_HAL_EVENT,
	CB_GET_HAL_EVENT,
	CB_BPWSN_READ,
	CB_BPWSN_SEND,
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

// EIP劫持，将线程hThread劫持到function上，function返回后线程正常运行，用于模拟中断
BOOL InterruptThreadWithFunction(DWORD hThread, void(*function)(void)) {
	CONTEXT Context;
	if (SuspendThread(hThread) == -1) {
		return FALSE;
	}
	Context.ContextFlags = CONTEXT_CONTROL;
	if (GetThreadContext(hThread, &Context) == FALSE) {
		ResumeThread(hThread);
		return FALSE;
	}
	PDWORD esp = (PDWORD *)Context.Esp;
	*--esp = Context.Eip;
	Context.Esp = esp;
	Context.Eip = function;
	if (SetThreadContext(hThread, &Context) == FALSE) {
		ResumeThread(hThread);
		return FALSE;
	}
	else {
		ResumeThread(hThread);
		return TRUE;
	}
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

#include <stdio.h>

typedef void (FuncVoidU8U8)(UINT8, UINT8);
typedef UINT8 (FuncU8U8)(UINT8);
typedef void (FuncVoidU8A)(UINT8 *);
/* -------------HAL EVENT------------------ */
typedef void (FuncVoidUA8)(UINT8[]);
typedef void (FuncVoidUA8UA8)(UINT8[], UINT8[]);
void doHalEvent(const UINT8 evt[]) {
	if (CheckCallBack(CB_DO_HAL_EVENT)) {
		Relocate(CB_DO_HAL_EVENT, FuncVoidUA8 *, evt);
	}
}
void getHalEvent(const UINT8 evt[], UINT8 result[]) {
	if (CheckCallBack(CB_GET_HAL_EVENT)) {
		Relocate(CB_GET_HAL_EVENT, FuncVoidUA8UA8 *, evt, result);
	}
	else {
		result[0] = '\0';
	}
}

/* -------------HAL_UART------------------- */
DLL_API void writeUART(UINT8 which,UINT8 x) {
	if (CheckCallBack(CB_UART_WRITE)) {
		Relocate(CB_UART_WRITE, FuncVoidU8U8 *, which, x);
	}
}
DLL_API UINT8 readUART(UINT8 which) {
	if (CheckCallBack(CB_UART_READ)) {
		return Relocate(CB_UART_READ, FuncU8U8 *, which);
	}
}

/* ------------HAL_TIME---------------------------- */
DLL_API void delayMs(UINT32 ms) {
	Sleep(ms);
}

DLL_API void LedSet(UINT8 which, UINT8 value) {
	if (CheckCallBack(CB_LEDSET)) {
		Relocate(CB_LEDSET, FuncVoidU8U8 *, which, value);
	}
	/*
	static UINT8 buff[8];
	if (which < 8) {
		buff[which] = value;
	}
	for (int i = 0; i < 8; i++) {
		printf("%02X ", buff[i]);
	}
	printf("\n");
	*/
}
/* ---------------HalLedLock----------------- */
//示例：通过字符串通道与主机交流，编写相关函数
DLL_API void HalLedLock(UINT8 lock) {
	if (lock) {
		doHalEvent("led off");
	}
	else {
		doHalEvent("led on");
	}
}

DLL_API UINT8 HalLedIsLock() {
	UINT8 buff[256];
	getHalEvent("led getlock", buff);

	if (strcmp(buff, "T") == 0) {
		return 1;
	}
	if (strcmp(buff, "F") == 0) {
		return 0;
	}
	return 0;//虚拟硬件不支持(buff为空白)
}

#define KEY_COUNT 4

void DebugString(const char * str) {
	FILE * f;
	fopen_s(&f, "D:\\log.txt", "w+");
	int i;
	fprintf(f, "%s|", str);
	for (i = 0; str[i]; i++) {
		fprintf(f, "%d ", str[i]);
	}

	fprintf(f, "|%d\n", i);
	fclose(f);
}

DLL_API UINT8 HalKeyDown(UINT8 key) {
	UINT8 buff[256];
	UINT8 str[256];
	if (key >= KEY_COUNT)
		return 0;

	sprintf_s(str,256,"trigkey%d", key);

	getHalEvent(str, buff);

	return strcmp(buff,"T")==0;
}
DLL_API void HalBpwsnSend(const UINT8 * pkg) {
	if (CheckCallBack(CB_BPWSN_SEND)) {
		Relocate(CB_BPWSN_SEND, FuncVoidU8A*, pkg);
	}
}
DLL_API void HalBpwsnRead(UINT8 * pkg) {
	if (CheckCallBack(CB_BPWSN_READ)) {
		Relocate(CB_BPWSN_READ, FuncVoidU8A*, pkg);
	}
}

DLL_API float HalTemGet() {
	UINT8 buff[256];
	getHalEvent("temsensor", buff);
	float ret;
	sscanf_s(buff, "[%f]", &ret);
	return ret;
}
