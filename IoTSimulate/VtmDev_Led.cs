using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTSimulate
{
    public partial class VtmDev
    {
        public const int LED_COUNT = 8;
        
        private byte[] LedValue = new byte[LED_COUNT];

        private LedPipe ledPipe;//处理LED管道

        [VtmFunction(VtmFunctionAttribute.FunctionType.Init)]
        private void Init_Led()
        {
            //led pipe
            AnonymousPipeServerStream ledRx = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable);
            ledPipe = new LedPipe(this, ledRx);
            args += " -leds " + ledRx.GetClientHandleAsString();
        }

        [VtmFunction(VtmFunctionAttribute.FunctionType.Update)]
        private void Update_Led()
        {
            ledPipe?.Update();
        }

        [VtmFunction(VtmFunctionAttribute.FunctionType.Close)]
        private void Close_Led()
        {
            ledPipe?.Close();
        }

        /// <summary>
        /// 获取开发板的共阴LED状态
        /// </summary>
        /// <param name="led">LED编号</param>
        /// <returns>一个字节代表LED状态</returns>
        public byte GetLedValue(int led)
        {
            if (LedLock)
                return 0x56;
            if (led < LedValue.Length)
                return LedValue[led];
            return 0;
        }

        private class LedPipe
        {
            VtmDev dev;
            PipeStream stream;

            byte[] buffer = new byte[1024];

            Task<int> task;
            int already;
            public LedPipe(VtmDev dev, PipeStream stream)
            {
                this.dev = dev;
                this.stream = stream;
                already = 0;
                GetBegin();
            }
            void GetBegin()
            {
                task?.Dispose();
                task = stream.ReadAsync(buffer, already, buffer.Length - already);
            }
            public void Update()
            {
                if (task.IsCompleted)
                {
                    already += task.Result;
                    int i = 0;
                    while (already > 1)
                    {
                        byte id = buffer[i++];
                        byte value = buffer[i++];
                        already -= 2;
                        if (id < VtmDev.LED_COUNT)
                        {
                            dev.LedValue[id] = value;
                        }
                    }
                    if (already == 1)
                    {
                        buffer[0] = buffer[i];//Move last to first
                    }
                    GetBegin();
                }
            }

            public void Close()
            {

            }
        }
    }
}
