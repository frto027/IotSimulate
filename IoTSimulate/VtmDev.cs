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

        private event Action UpdateList;
        private event Action CloseList;

        public VtmDev(String binPath)
        {

            //Configure params
            args = "-f " + "\"" + binPath + "\"";//bin path

            //HalEventPipe
            AnonymousPipeServerStream
                HalEventTx = CreatePipeToArgument("haleventi", false),
                HalEventRx = CreatePipeToArgument("halevento", true);
            halEventPipe = new HalEventPipe(this, new BinaryWriter(HalEventTx), new BinaryReader(HalEventRx));

            //反射函数处理
            foreach (var func in typeof(VtmDev).GetMethods(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))
            {
                foreach (VtmFunctionAttribute attr in func.GetCustomAttributes(typeof(VtmFunctionAttribute), true))
                {
                    switch (attr.AttrType)
                    {
                        case VtmFunctionAttribute.FunctionType.Init:
                            func.Invoke(this, null);
                            break;
                        case VtmFunctionAttribute.FunctionType.Update:
                            var delgt_update = func.CreateDelegate(typeof(Action), this);
                            UpdateList += () => delgt_update.DynamicInvoke(null);
                            break;
                        case VtmFunctionAttribute.FunctionType.Close:
                            var delgt_close = func.CreateDelegate(typeof(Action), this);
                            CloseList += () => delgt_close.DynamicInvoke(null);
                            break;
                        case VtmFunctionAttribute.FunctionType.HalDoEvent:
                            var delgt_doevent = func.CreateDelegate(typeof(Action<string>), this);
                            DoHalEvent += (s) => delgt_doevent.DynamicInvoke(s);
                            break;
                        case VtmFunctionAttribute.FunctionType.HalGetEvent:
                            var delgt_getevent = func.CreateDelegate(typeof(Func<string, string>), this);
                            GetHalEventList.Add((s) => delgt_getevent.DynamicInvoke(s) as string);
                            break;
                    }
                }
            }
        }
        /// <summary>
        /// 建立一个方向为In或者Out的匿名管道，并将管道的名字附加到-arg参数后面
        /// </summary>
        /// <param name="arg">管道名字对应的参数</param>
        /// <param name="directionIn">管道方向，true是In，即从外部输入到这里的C#，false是Out，即输出到外部</param>
        /// <returns>匿名管道(可转化为IO流)</returns>
        private AnonymousPipeServerStream CreatePipeToArgument(string arg,bool directionIn)
        {
            var r = new AnonymousPipeServerStream(directionIn ? PipeDirection.In : PipeDirection.Out, HandleInheritability.Inheritable);
            args += " -" + arg + " " + r.GetClientHandleAsString();
            return r;
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
       

        public void Update()
        {
            halEventPipe?.Update();

            UpdateList();
        }

        public void Close()
        {
            //kill program
            try
            {
                if (process != null && process.HasExited == false)
                    process?.Kill();//小心抛异常！
            }
            catch (Exception) { }
            
            CloseList();

            halEventPipe?.Close();
        }

        
        /// <summary>
        /// 提供另一种Hal交互方式：字符消息管道，见LedLock.cs，以及VtmPipe.dll中两个函数的实现
        /// </summary>
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
                        if(str == null)
                        {
                            throw new ArgumentException("字符状态获取失败，不存在对应于" + r.evt + "的解析程序");
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

        public class VtmFunctionAttribute : Attribute
        {
            public enum FunctionType { Init,Update,HalDoEvent,HalGetEvent,Close};
            private FunctionType type;

            public FunctionType AttrType => type;

            public VtmFunctionAttribute(FunctionType type)
            {
                this.type = type;
            }
        }
    }
}
