#ifndef __HAL_BASE_H
#define __HAL_BASE_H

asm (".section .drectve\n\t.ascii \" -export:VHPipe_SetupLinks=VHPipe.VHPipe_SetupLinks\"");

__declspec(dllexport) int main(void);


#endif