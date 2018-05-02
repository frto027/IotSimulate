#include "hal_base.h"
#include "hal_types.h"
#include "hal_uart.h"
#include "hal_time.h"
#include "hal_led.h"
#include "hal_bpwsn.h"
#include "hal_key.h"
#include "hal_sensor.h"

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
//bpwsn test
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
//KeyTest
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

void HalLedNumDot(uint8 led,uint8 value){
	const static unsigned char HalLedTable[]={
		0x3f,0x06,0x5b,0x4f,
		0x66,0x6d,0x7d,0x07,
		0x7f,0x6f,0x77,0x7c,
		0x39,0x5e,0x79,0x71
	};
	HalLedSet(led,HalLedTable[value%0x10] | 0x80);
}

void runF(){
	float tem;
	int i;
	tem	= HalTemGet();
	
	if(tem < 0){
		HalLedSet(0,0x40);//-
		tem = - tem;
	}
	else{
		HalLedSet(0,0x00);
	}
	
	
	i = (int)tem;
	tem -= i;
	
	for(int k=0;k<3;k++){
		if(k == 0){
			HalLedNumDot(3 - k,i%10);
		}
		else{
			if(i != 0)
				HalLedNum(3 - k,i%10);
			else
				HalLedSet(3 - k,0x00);
		}
		i/=10;
	}
	for(int k=4;k<8;k++){
		tem *= 10;
		HalLedNum(k,(int)tem);
		tem -= (int)tem;
	}
	HalDelayMs(100);
}
int main(){
	while(1){
		runF();
	}
	return 0;
}