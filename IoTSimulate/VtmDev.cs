using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Pipes;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace IoTSimulate
{
    /// <summary>
    /// 虚拟开发板，创建时需要提供二进制文件路径
    /// 平台相关
    /// </summary>
    public class VtmDev : IDevices
    {
        private static JobControl.Job jobObj = new JobControl.Job();

        private const string VHClientPath = "F:\\project\\VSNet\\IoTSimulate\\VHClient\\bin\\Debug\\VHClient.exe";

        public const int
            UART_COUNT = 3,
            LED_COUNT = 8;


        private ComBase[] ComPort = new PipeCom[UART_COUNT];//不可以修改ComPort

        private byte[] LedValue = new byte[LED_COUNT];

        private LedPipe ledPipe;

        Process process;

        public bool IsExit { get { return process?.HasExited??true; } }

        string args;

        public VtmDev(String binPath)
        {
            
            //Configure params
            args = "-f " + binPath;//bin path
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
            
        }
        /// <summary>
        /// 启动新进程，（如果有）终止旧进程
        /// </summary>
        public void Start()
        {
            if(process != null && process.HasExited == false)
            {
                process.Kill();
                process.WaitForExit();
            }
            //start up Client
            process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = VHClientPath,
                    Arguments = args,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            jobObj.AddProcess(process.Handle);
        }
        /// <summary>
        /// 向子进程发送Kill
        /// </summary>
        public void InformStop()
        {
            process?.Kill();
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

        public void Update()
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
            if (process != null && process.HasExited == false)
                process?.Kill();//小心抛异常！
            foreach(PipeCom p in ComPort)
            {
                p?.Close();
            }
            ledPipe?.Close();
        }

        private class PipeCom : ComBase
        {
            PipeStream iStream,oStream;

            byte[] buffer = new byte[1];
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
        private class LedPipe
        {
            VtmDev dev;
            PipeStream stream;

            byte[] buffer = new byte[2];

            Task<int> task;
            int already;
            public LedPipe(VtmDev dev,PipeStream stream)
            {
                this.dev = dev;
                this.stream = stream;

                GetBegin();
            }
            void GetBegin()
            {
                already = 0;
                task?.Dispose();
                task = stream.ReadAsync(buffer, already, 2 - already);
            }
            bool GetOK()
            {
                if (task.IsCompleted)
                {
                    already += task.Result;
                    if(already == 2)
                    {
                        return true;
                    }
                    else
                    {
                        task.Dispose();
                        task = stream.ReadAsync(buffer, already, 2 - already);
                        return false;
                    }
                }
                return false;
            }
            public void Update()
            {
                if (GetOK())
                {
                    if (buffer[0] < VtmDev.LED_COUNT)
                    {
                        dev.LedValue[buffer[0]] = buffer[1];
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
