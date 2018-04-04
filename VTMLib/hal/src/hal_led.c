#include "hal_types.h"
#include "hal_led.h"

__declspec(dllimport) void LedSet(uint8 which, uint8 value);



void HalLedInit(){
	
}

void HalLedNum(uint8 led,uint8 value){
	const static unsigned char HalLedTable[]={
		0x3f,0x06,0x5b,0x4f,
		0x66,0x6d,0x7d,0x07,
		0x7f,0x6f,0x77,0x7c,
		0x39,0x5e,0x79,0x71
	};
	HalLedSet(led,HalLedTable[value%0x10]);
}
void HalLedSet(uint8 led,uint8 value){
	if(led<HAL_LED_NUM)
		LedSet(led,value);
}
