#include "hal_base.h"
#include "hal_types.h"
#include "hal_uart.h"
#include "hal_time.h"
#include "hal_led.h"
#include "hal_bpwsn.h"

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

void runD(){
	union BpwsnPackage pkg;
	for(int i=0;i<HAL_BPWSN_MESSAGE_SIZE;i++){
		pkg.msg[i]=0;
	}
	for(int i=0;i<HAL_BPWSN_MESSAGE_SIZE * 10;i++){
		pkg.msg[i%HAL_BPWSN_MESSAGE_SIZE]=i%0x100;
		
		HalBpwsnSumPackage(&pkg);
		HalBpwsnSendPackage(&pkg);
		HalDelayMs(1000);
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
		runD();
	}
	return 0;
}