SRC+=main.c
#这个指向hal目录所在文件夹
LIB_ROOT=./..
#这个是直接指向VHPipe.dll所在文件夹
VHPipe_LIB=./../..

CC=gcc

HAL_ROOT=$(LIB_ROOT)/hal

HAL_INCLUDE=$(HAL_ROOT)/include

SRC+= $(wildcard $(HAL_ROOT)/src/*.c)

INCLUDE+= -I $(HAL_INCLUDE)
INCLUDE+= -I .


LIB_DIR+= -L $(VHPipe_LIB)

#下面这个是VHPipe.dll编译生成的目录，如果没办法编译的话改一下这个
#VHPipeBuildDir=$(LIB_ROOT)/../Debug

LIB += -lVHPipe

.PHONY:a.bin debug.bin
a.bin:
	@$(CC) $(DEFS) $(INCLUDE) $(LIB_DIR) $(LIB) -shared $(SRC) -o a.bin
	@echo build end
debug.bin:
	@echo Delete *.ilk *.exp *.lib *.pdb .....
	@del $(subst /,\,$(VHPipeBuildDir)/*.ilk $(VHPipeBuildDir)/*.exp $(VHPipeBuildDir)/*.lib $(VHPipeBuildDir)/*.pdb)
	@$(CC) $(DEFS) $(INCLUDE) $(LIB_DIR) -L $(VHPipeBuildDir) $(LIB) -shared $(SRC) -o debug.bin
	@echo build end