#include "hal_base.h"
#include "hal_types.h"
#include "hal_uart.h"
#include "hal_time.h"
#include "hal_led.h"

void runA(){
	for(int i=0;i<32;i++){
		for(int j=0;j<8;j++){
			HalLedSet(j,i*8+j);
		}
		HalDelayMs(200);
	}
}
void runB(){
	for(int i=0;i<0x10;i++){
		for(int j=0;j<8;j++){
			HalLedNum(j,(i+j)%0x10);
		}
		HalDelayMs(1000);
	}
}
void runC(){
	uint8 buff;
	for(int i=0;i<0xFF;i++){
		for(int j=0;j<4;j++){
			HalLedNum(j*2,i/0x10);
			HalLedNum(j*2+1,i%0x10);
			buff=i;
			HalUartWrite(UART_0,&buff,1);
		}
		HalDelayMs(400);
	}
}
int main(){
	/*
	for(int i=0;i<0x10;i++){
		HalLedNum(i%8,i);
		HalDelayMs(1000);
	}
	*/
	while(1){
		runC();
	}
	return 0;
}