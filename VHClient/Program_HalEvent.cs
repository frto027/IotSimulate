using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VHClient
{
    partial class Program
    {

        private static PipeStream HalEventTx, HalEventRx;
        private static BinaryWriter HalEventTxWriter;
        private static BinaryReader HalEventRxReader;

        private static void HalEventInit(Dictionary<string,string> argMaps)
        {
            if (argMaps.ContainsKey("haleventi"))
            {
                HalEventRx = new AnonymousPipeClientStream(PipeDirection.In, argMaps["haleventi"]);
                HalEventRxReader = new BinaryReader(HalEventRx);
            }

            if (argMaps.ContainsKey("halevento"))
            {
                HalEventTx = new AnonymousPipeClientStream(PipeDirection.Out, argMaps["halevento"]);
                HalEventTxWriter = new BinaryWriter(HalEventTx);
            }
        }

        private static void StringToArr(string str, byte[] arr)//arr不安全
        {
            int i = 0;
            foreach (char c in str)
            {
                arr[i++] = (byte)c;
            }
            arr[i++] = 0;
        }
        //Bug:arr在C语言中长度不知，所以有问题，这里arr数组长度[不安全]！
        private static string ArrToString(byte[] arr)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in arr)
            {
                if (b != 0)
                {
                    stringBuilder.Append((char)b);
                }
                else
                {
                    break;
                }
            }
            return stringBuilder.ToString();
        }

        //TODO：此函数应该由SetupLinks绑定到C++的某个函数，并将evt映射到某个函数内部
        //输入：evt，C#收到此消息后需要改变相应的硬件状态
        private static void DoHalEvent(byte[] evt)//数组长度[不安全]
        {
            //这个函数被C++调用
            //通知C#虚拟端执行某个事件
            HalEventTxWriter?.Write(true);//true表示纯事件
            HalEventTxWriter?.Write(ArrToString(evt));
            HalEventTxWriter?.Flush();
        }
        //TODO:此函数应由SetupLinks绑定到某个函数
        //输入：evt，表示查询字符串，C#在HalRx收到这个之后必须从HalTx发送状态
        //输出：ret，表示结果的字符串，由C#的HalTx发送，VHPipe.dll的C++代码解析
        private static void GetHalEvent(byte[] evt, byte[] ret)//数组长度[不安全]
        {
            //这个函数被C++调用，用于获取某个虚拟硬件状态，并将结果填充到ret里面
            HalEventTxWriter?.Write(false);//false表示get请求返回状态
            HalEventTxWriter?.Write(ArrToString(evt));
            HalEventTxWriter?.Flush();
            String str = HalEventRxReader?.ReadString();
            if (str != null)
            {
                StringToArr(str, ret);
            }
        }

    }
}
