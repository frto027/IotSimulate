#ifndef __HAL_UART_H
#define __HAL_UART_H

#include "hal_types.h"

#define UART_0 0
#define UART_1 1
#define UART_2 2

extern int HalUartOpen(uint8 port);

extern void HalUartRead(uint8 port,uint8 * buff,unsigned int count);
extern void HalUartWrite(uint8 port,uint8 * buff,unsigned int count);

extern int HalUartClose(uint8 port);
#endif