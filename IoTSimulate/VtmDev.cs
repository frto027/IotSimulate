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
    public partial class VtmDev : IDevices
    {
        private static JobControl.Job jobObj = new JobControl.Job();

        private const string VHClientPath = "VHClient.exe";
        //private const string VHClientPath = "F:\\project\\VSNet\\IoTSimulate\\VHClient\\bin\\Debug\\VHClient.exe";

        public const int
            UART_COUNT = 3,
            LED_COUNT = 8;


        private ComBase[] ComPort = new PipeCom[UART_COUNT];//不可以修改ComPort

        private byte[] LedValue = new byte[LED_COUNT];

        private bool LedLock = false;//是否关闭Led显示

        private LedPipe ledPipe;//处理LED管道

        private HalEventPipe halEventPipe;//处理HalEvelt管道

        Process process;

        public bool IsExit { get { return process?.HasExited??true; } }

        string args;
        /// <summary>
        /// 当接收到一个消息的时候，事件处理列表中的所有事件都会被执行
        /// </summary>
        public event Action<string> DoHalEvent;//事件处理列表
        /// <summary>
        /// 当收到一个请求的时候，捕获列表中的函数依次被执行，如果返回值非null，则停止执行，使用此返回值作为状态发送给C语言模块
        /// </summary>
        public List<Func<string,string>> GetHalEventList = new List<Func<string,string>>();//捕获列表


        public VtmDev(String binPath)
        {

            //Configure params
            args = "-f " + "\"" + binPath + "\"";//bin path
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

            //HalEventPipe
            AnonymousPipeServerStream HalEventTx = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable),
                HalEventRx = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable);
            args += " -haleventi " + HalEventTx.GetClientHandleAsString();
            args += " -halevento " + HalEventRx.GetClientHandleAsString();

            halEventPipe = new HalEventPipe(this, new BinaryWriter(HalEventTx), new BinaryReader(HalEventRx));

            //DEBUG HAL DEVICE:HAL SWITCH
            DoHalEvent += (string s) =>
            {
                //Console.WriteLine(s);
                if (s.StartsWith("led "))
                {
                    if(s == "led on")
                    {
                        LedLock = false;
                    }
                    if(s == "led off")
                    {
                        LedLock = true;
                    }
                }
            };
            GetHalEventList.Add((string s) =>
            {
                if (s.StartsWith("led "))
                {
                    if (s == "led getlock")
                    {
                        return LedLock ? "T" : "F";//返回string表示处理此结果
                    }
                }
                return null;//返回null表示不处理此结果
            });

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
            if (LedLock)
                return 0x56;
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
            halEventPipe?.Update();
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
            halEventPipe?.Close();
        }

        private class PipeCom : ComBase
        {
            PipeStream iStream,oStream;

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
        private class LedPipe
        {
            VtmDev dev;
            PipeStream stream;

            byte[] buffer = new byte[1024];

            Task<int> task;
            int already;
            public LedPipe(VtmDev dev,PipeStream stream)
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
                    already +=task.Result;
                    int i = 0;
                    while(already > 1)
                    {
                        byte id = buffer[i++];
                        byte value = buffer[i++];
                        already -= 2;
                        if(id < VtmDev.LED_COUNT)
                        {
                            dev.LedValue[id] = value;
                        }
                    }
                    if(already == 1)
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
        private class HalEventPipe
        {
            BinaryReader input;
            BinaryWriter output;
            private VtmDev parent;
            
            private struct ReadResult
            {
                public bool isDoEvent;
                public string evt;
            }
            
            Queue<Task<ReadResult>> taskList = new Queue<Task<ReadResult>>();
            //Task<ReadResult> task;

            public HalEventPipe(VtmDev parent, BinaryWriter output,BinaryReader input)
            {
                this.input = input;
                this.output = output;
                this.parent = parent;

                NextRead();
            }
            void NextRead()
            {
                var task = new Task<ReadResult>(() => 
                {
                    ReadResult result;
                    result.isDoEvent = input.ReadBoolean();
                    result.evt = input.ReadString();
                    NextRead();
                    return result;
                });
                task.Start();
                taskList.Enqueue(task);
            }
            public void Update()
            {
                var task = taskList.First();
                while(task?.IsCompleted ?? false)
                {
                    taskList.Dequeue();
                    var r = task.Result;
                    if (r.isDoEvent)
                    {
                        parent.DoHalEvent(r.evt);
                    }
                    else
                    {
                        string str = null;
                        foreach(var func in parent.GetHalEventList)
                        {
                            str = func?.Invoke(r.evt);
                            if (str != null)
                                break;
                        }
                        output.Write(str);
                        output.Flush();
                    }
                    task = taskList.First();
                    //NextRead();
                }
            }

            public void Close()
            {
                
            }
        }
    }
}
