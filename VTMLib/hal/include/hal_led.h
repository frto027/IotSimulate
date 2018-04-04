#ifndef __HAL_LED_H
#define __HAL_LED_H

#include "hal_types.h"

#define HAL_LED_0 0
#define HAL_LED_1 1
#define HAL_LED_2 2
#define HAL_LED_3 3
#define HAL_LED_4 4
#define HAL_LED_5 5
#define HAL_LED_6 6 
#define HAL_LED_7 7

#define HAL_LED_NUM 8

extern void HalLedInit();

extern void HalLedNum(uint8 led,uint8 value);
extern void HalLedSet(uint8 led,uint8 value);
#endif