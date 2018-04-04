#include "hal_types.h"
#include "hal_time.h"

__declspec(dllimport) void delayMs(uint32 ms);

void HalDelayMs(unsigned int ms){
	delayMs(ms);
}