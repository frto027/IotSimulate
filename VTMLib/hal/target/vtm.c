#include "vtm.h"

#define DAPI(t) __declspec(dllexport) t
//中断
DAPI(uint8) EA;
//看门狗
DAPI(uint8) WDCTL;
//P0 P1口
#define SFR(name) DAPI(uint8) name;
SFR(P0)
SFR(P1)