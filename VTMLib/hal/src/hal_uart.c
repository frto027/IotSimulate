#include "hal_uart.h"

__declspec(dllimport) void writeUART(uint8 which,uint8 ch);
__declspec(dllimport) uint8 readUART(uint8 which);

int HalUartOpen(uint8 port){
	
}

void HalUartRead(uint8 port,uint8 * buff,unsigned int count){
	switch(port){
		case UART_0:
		case UART_1:
		case UART_2:
			while(count--)
				*buff++=readUART(port);
		break;
	}
}

void HalUartWrite(uint8 port,uint8 * buff,unsigned int count){
	switch(port){
		case UART_0:
		case UART_1:
		case UART_2:
			while(count--)
				writeUART(port,*buff++);
		break;
	}
}

int HalUartClose(uint8 port){
	
}