#ifndef __VTMBASIC_H
#define __VTMBASIC_H

//定义main函数为导出函数，以便在虚拟进程中执行
__declspec(dllexport) int main(void);

//从其它dll中导入相关的函数，与虚拟硬件进行交互
//函数转发（将VHPipe的初始化函数转发到导出表）
asm (".section .drectve\n\t.ascii \" -export:VHPipe_SetupLinks=VHPipe.VHPipe_SetupLinks\"");

#endif //__VTMBASIC_H