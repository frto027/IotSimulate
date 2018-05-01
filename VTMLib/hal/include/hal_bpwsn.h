#ifndef __BPWSN_H
#define __BPWSN_H

#include "hal_types.h"

#define HAL_BPWSN_SIZE 16
#define HAL_BPWSN_MESSAGE_SIZE 11
union BpwsnPackage{
	uint8 data[HAL_BPWSN_SIZE];
	struct{
		uint8 head[2];
		uint8 sensor_type;//sensor type
		uint8 sid;//sensor id
		uint8 msg[HAL_BPWSN_MESSAGE_SIZE];
		uint8 chk;//check bit
	};
};
//Add head and checksum to package
extern void HalBpwsnSumPackage(union BpwsnPackage * pkg);
//Broadcast a package to network
extern void HalBpwsnSendPackage(union BpwsnPackage * pkg);
//Receive a package from network(block program).return 0 if failed(check error).return 1 if success.
extern int HalBpwsnGetPackage(union BpwsnPackage * pkg);

#endif //__BPWSN_H