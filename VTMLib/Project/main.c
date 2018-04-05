#include "hal_base.h"
#include "hal_types.h"
#include "hal_uart.h"
#include "hal_time.h"
#include "hal_led.h"

int main(){
	uint8 buff[10];
	buff[0]=2;
	HalUartWrite(UART_0,buff,1);
	HalDelayMs(1000);
	HalUartWrite(UART_0,buff,1);
	HalLedSet(1,0xFC);
	HalDelayMs(1000);
	return 0;
}