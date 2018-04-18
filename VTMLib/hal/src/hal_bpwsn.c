#include "hal_bpwsn.h"

__declspec(dllimport) void HalBpwsnSend(const uint8* pkg);
__declspec(dllimport) void HalBpwsnRead(uint8* pkg);

void HalBpwsnSendPackage(BpwsnPackage * pkg){
	uint8 chk = 0;
	for(int i=0;i<HAL_BPWSN_SIZE - 1;i++){
		chk += pkg->data[i];
	}
	pkg->chk = chk;
	HalBpwsnSend(pkg->data);
}

int HalBpwsnGetPackage(BpwsnPackage * pkg){
	BpwsnPackage tpkg;
	uint8 chk = 0;
	
	HalBpwsnRead(tpkg->data);
	
	for(int i=0;i<HAL_BPWSN_SIZE - 1;i++){
		chk += tpkg->data[i];
	}
	if(chk != tpkg->chk){
		return 0;
	}
	for(int i=0;i<HAL_BPWSN_SIZE;i++){
		pkg->data[i] = tpkg->data[i];
	}
	return 1;
}