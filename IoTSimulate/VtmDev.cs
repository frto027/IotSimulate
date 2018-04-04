using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Pipes;
using System.IO;
using System.Diagnostics;

namespace IoTSimulate
{
    /// <summary>
    /// 虚拟开发板，创建时需要提供二进制文件路径
    /// </summary>
    public class VtmDev : IDevices
    {
        private const string VHClientPath = "F:\\project\\VSNet\\IoTSimulate\\VHClient\\bin\\Debug\\VHClient.exe";

        public const int
            UART_COUNT = 3,
            LED_COUNT = 8;

        Process process;

        private ComBase[] ComPort = new PipeCom[UART_COUNT];//不可以修改ComPort

        private byte[] LedValue = new byte[LED_COUNT];

        private LedPipe ledPipe;
        
        public VtmDev(String binPath)
        {
            //Configure params
            String args = "-f " + binPath;//bin path
            for(int i = 0; i < ComPort.Length; i++)//uart IO pipe
            {
                AnonymousPipeServerStream
                    Tx = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable),
                    Rx = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable);
                args += " -iuart" + i.ToString() + " " + Tx.GetClientHandleAsString();
                args += " -ouart" + i.ToString() + " " + Rx.GetClientHandleAsString();
                ComPort[i] = new PipeCom(Rx, Tx);
            }
            //led pipe
            AnonymousPipeServerStream ledRx = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable);
            ledPipe = new LedPipe(this, ledRx);
            args += " -leds " + ledRx.GetClientHandleAsString();
            //start up Client
            process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = VHClientPath,
                    Arguments = args,
                    UseShellExecute = false
                }
            };
            process.Start();
        }
        /// <summary>
        /// 获取开发板的串口连接
        /// </summary>
        /// <param name="num">串口编号</param>
        /// <returns>串口基类</returns>
        public ComBase GetComPortBase(int num)
        {
            if(num < UART_COUNT)
            {
                return ComPort[num];
            }
            return null;
        }
        /// <summary>
        /// 获取开发板的共阴LED状态
        /// </summary>
        /// <param name="led">LED编号</param>
        /// <returns>一个字节代表LED状态</returns>
        public byte GetLedValue(int led)
        {
            if (led < LedValue.Length)
                return LedValue[led];
            return 0;
        }

        void Update()
        {
            foreach(PipeCom p in ComPort)
            {
                p.Update();
            }
            ledPipe?.Update();
        }

        public void Close()
        {
            //kill program
            try
            {
                process?.Kill();
            }catch(Exception)
            {

            }
        }

        private class PipeCom : ComBase
        {
            PipeStream iStream,oStream;
            
            public PipeCom(PipeStream iStream, PipeStream oStream)
            {
                this.iStream = iStream;
                this.oStream = oStream;
            }
            public override void OnDataReceive(byte[] data, int offset, int len)
            {
                oStream.Write(data, offset, len);
                oStream.Flush();
            }

            public void Update()
            {
                if(iStream.IsMessageComplete == false)
                {
                    byte[] buffer = new byte[1024];
                    int len = iStream.Read(buffer, 0, buffer.Length);
                    ToConnector(buffer, 0, len);
                }
            }
        }
        private class LedPipe
        {
            VtmDev dev;
            PipeStream stream;
            public LedPipe(VtmDev dev,PipeStream stream)
            {
                this.dev = dev;
                this.stream = stream;
            }

            public void Update()
            {
                if(stream.IsMessageComplete == false)
                {
                    int target = stream.ReadByte();
                    int val = stream.ReadByte();
                    if (target < VtmDev.LED_COUNT)
                    {
                        dev.LedValue[target] = (byte)val;
                    }
                }
            }
        }
    }
}
