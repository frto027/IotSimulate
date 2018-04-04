SRC+=main.c

LIB_ROOT=F:/project/VSNet/IoTSimulate/VTMLib

CC=gcc

HAL_ROOT=$(LIB_ROOT)/hal

HAL_INCLUDE=$(HAL_ROOT)/include

SRC+= $(wildcard $(HAL_ROOT)/src/*.c)

INCLUDE+= -I $(HAL_INCLUDE)
INCLUDE+= -I .


#LIB_DIR+= -L $(LIB_ROOT)/lib <--到时候把VHPipe.dll复制到这个目录
#下面这个是VHPipe.dll编译生成的目录，如果没办法编译的话改一下这个
VHPipeBuildDir=$(LIB_ROOT)/../Debug
LIB_DIR+= -L $(VHPipeBuildDir)

LIB += -lVHPipe

.PHONY:a.dll cleanbuild
a.dll:cleanbuild
	@$(CC) $(DEFS) $(INCLUDE) $(LIB_DIR) $(LIB) -shared $(SRC) -o a.dll
	@echo build end
cleanbuild:
	@echo Delete *.ilk *.exp *.lib *.pdb .....
	@del $(subst /,\,$(VHPipeBuildDir)/*.ilk $(VHPipeBuildDir)/*.exp $(VHPipeBuildDir)/*.lib $(VHPipeBuildDir)/*.pdb)
	
run:a.dll
	echo start run...
	@set PATH=$(PATH);$(subst /,\,$(VHPipeBuildDir))&\
	$(LIB_ROOT)/bin/VHClient.exe -f a.dll
	
