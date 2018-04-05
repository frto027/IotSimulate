#include "hal_base.h"
#include "hal_types.h"
#include "hal_uart.h"
#include "hal_time.h"
#include "hal_led.h"

int main(){
	for(int i=0;i<0x10;i++){
		HalLedNum(i%8,i);
		HalDelayMs(1000);
	}
	return 0;
}