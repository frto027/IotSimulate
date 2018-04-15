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
        public const int UART_COUNT = 3;

        private ComBase[] ComPort = new PipeCom[UART_COUNT];//不可以修改ComPort


        [VtmFunction(VtmFunctionAttribute.FunctionType.Init)]
        private void Init_Comport()
        {
            for (int i = 0; i < ComPort.Length; i++)//uart IO pipe
            {
                AnonymousPipeServerStream
                    Tx = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable),
                    Rx = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable);
                args += " -iuart" + i.ToString() + " " + Tx.GetClientHandleAsString();
                args += " -ouart" + i.ToString() + " " + Rx.GetClientHandleAsString();
                ComPort[i] = new PipeCom(Rx, Tx);
            }
        }
 
        [VtmFunction(VtmFunctionAttribute.FunctionType.Update)]
        private void Update_Comport()
        {
            foreach (PipeCom p in ComPort)
            {
                p.Update();
            }
        }

        [VtmFunction(VtmFunctionAttribute.FunctionType.Close)]
        private void Close_Comport()
        {
            foreach (PipeCom p in ComPort)
            {
                p?.Close();
            }
        }

        /// <summary>
        /// 获取开发板的串口连接
        /// </summary>
        /// <param name="num">串口编号</param>
        /// <returns>串口基类</returns>
        public ComBase GetComPortBase(int num)
        {
            if (num < UART_COUNT)
            {
                return ComPort[num];
            }
            return null;
        }

        private class PipeCom : ComBase
        {
            PipeStream iStream, oStream;

            byte[] buffer = new byte[1024];
            Task<int> task;

            public PipeCom(PipeStream iStream, PipeStream oStream)
            {
                this.iStream = iStream;
                this.oStream = oStream;

                ReadNext();
            }

            public override void OnDataReceive(byte[] data, int offset, int len)
            {
                oStream.Write(data, offset, len);
                oStream.Flush();
            }

            private void ReadNext()
            {
                task?.Dispose();
                task = iStream.ReadAsync(buffer, 0, buffer.Length);
            }

            public void Update()
            {
                if (task.IsCompleted)
                {
                    ToConnector(buffer, 0, task.Result);
                    ReadNext();
                }
            }

            public override void Close()
            {
                base.Close();
            }
        }
    }
}
