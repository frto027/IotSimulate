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
int main(){
	/*
	for(int i=0;i<0x10;i++){
		HalLedNum(i%8,i);
		HalDelayMs(1000);
	}
	*/
	while(1){
		runB();
	}
	return 0;
}