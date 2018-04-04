#include "hal_uart.h"

__declspec(dllimport) void writeUART(uint8 ch);
__declspec(dllimport) uint8 readUART();

int HalUartOpen(uint8 port){
	
}

void HalUartRead(uint8 port,uint8 * buff,unsigned int count){
	switch(port){
		case UART_0:
			while(count--)
				*buff++=readUART();
		break;
	}
}

void HalUartWrite(uint8 port,uint8 * buff,unsigned int count){
	switch(port){
		case UART_0:
			while(count--)
				writeUART(*buff++);
		break;
	}
}

int HalUartClose(uint8 port){
	
}