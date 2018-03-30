/* vtm drivers */

#ifndef __VTM_H
#define __VTM_H

#include "vtmbasic.h"

#include "hal_types.h"

extern uint8 EA;

extern uint8 WDCTL;

struct __VTM_BITTYPE{\
    uint8 bit7:1;\
    uint8 bit6:1;\
    uint8 bit5:1;\
    uint8 bit4:1;\
    uint8 bit3:1;\
    uint8 bit2:1;\
    uint8 bit1:1;\
    uint8 bit0:1;\
};
#define __VTM_ACCESS_BITTYPE(name,x) (((struct __VTM_BITTYPE*)(&(name)))->bit##x)
#define SFR(name) extern uint8 name;
#define SFRBIT(name, bit7, bit6, bit5, bit4, bit3, bit2, bit1, bit0) SFR(name)
//由于宏定义不能嵌套，需要手动定义
/*
#define bit7 __VTM_BITTYPE(name,7)
...
*/
#define SBIT(name,addr)

SFRBIT( P0, P0_7, P0_6, P0_5, P0_4, P0_3, P0_2, P0_1, P0_0 )
#define P0_7 __VTM_ACCESS_BITTYPE(P0,7)
#define P0_6 __VTM_ACCESS_BITTYPE(P0,6)
#define P0_5 __VTM_ACCESS_BITTYPE(P0,5)
#define P0_4 __VTM_ACCESS_BITTYPE(P0,4)
#define P0_3 __VTM_ACCESS_BITTYPE(P0,3)
#define P0_2 __VTM_ACCESS_BITTYPE(P0,2)
#define P0_1 __VTM_ACCESS_BITTYPE(P0,1)
#define P0_0 __VTM_ACCESS_BITTYPE(P0,0)
SFRBIT( P1, P1_7, P1_6, P1_5, P1_4, P1_3, P1_2, P1_1, P1_0 )
#define P1_7 __VTM_ACCESS_BITTYPE(P1,7)
#define P1_6 __VTM_ACCESS_BITTYPE(P1,6)
#define P1_5 __VTM_ACCESS_BITTYPE(P1,5)
#define P1_4 __VTM_ACCESS_BITTYPE(P1,4)
#define P1_3 __VTM_ACCESS_BITTYPE(P1,3)
#define P1_2 __VTM_ACCESS_BITTYPE(P1,2)
#define P1_1 __VTM_ACCESS_BITTYPE(P1,1)
#define P1_0 __VTM_ACCESS_BITTYPE(P1,0)

#undef SFR
#undef SFRBIT

#endif //__VTM_H