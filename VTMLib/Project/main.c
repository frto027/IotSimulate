#include "hal_base.h"
#include "hal_types.h"
#include "hal_uart.h"
#include "hal_time.h"
#include "hal_led.h"
#include "hal_bpwsn.h"
#include "hal_key.h"

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
void runE(){
	static uint8 buff[4];
	if(HalKeyDown(KEY_A)){
		buff[0]++;
	}
	if(HalKeyDown(KEY_B)){
		buff[1]++;
	}
	if(HalKeyDown(KEY_C)){
		buff[2]++;
	}
	if(HalKeyDown(KEY_D)){
		buff[3]++;
	}
	for(int i=0;i<4;i++){
		HalLedNum(i*2,buff[i]>>4);
		HalLedNum(i*2+1,buff[i]&0xF);
	}
	HalDelayMs(50);
}
int main(){
	while(1){
		runE();
	}
	return 0;
}