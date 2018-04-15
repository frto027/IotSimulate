using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Pipes;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace VHClient
{
    class Program
    {
        /*
        private static void Pipe_USART_Write(int ch)
        {
            Console.WriteLine(ch);
        }
        private static int Pipe_USART_Read()
        {
            return 2;
        }
        */

        private const int UART_COUNT = 3;

        private static PipeStream ledStream;
        private static PipeStream[] UartTx, UartRx;
        private static PipeStream HalEventTx, HalEventRx;
        private static BinaryWriter HalEventTxWriter;
        private static BinaryReader HalEventRxReader;
        private static void Pipe_UartWrite(byte uart,byte value)
        {
            if (uart < UartTx.Length)
            {
                UartTx[uart]?.WriteByte(value);
                UartTx[uart]?.Flush();
            }
        }
        private static byte Pipe_UartRead(byte uart)
        {
            if(uart < UartRx.Length && UartRx[uart] != null)
            {
                return (byte)UartRx[uart].ReadByte();
            }
            return 0;
        }
        private static void Pipe_LedSet(byte led, byte value)
        {
            ledStream?.WriteByte(led);
            ledStream?.WriteByte(value);
            ledStream?.Flush();
        }
        
        private static void StringToArr(string str,byte[] arr)//arr不安全
        {
            int i = 0;
            foreach(char c in str)
            {
                arr[i++] = (byte)c;
            }
            arr[i++] = 0;
        }
        //Bug:arr在C语言中长度不知，所以有问题，这里arr数组长度[不安全]！
        private static string ArrToString(byte [] arr)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach(byte b in arr)
            {
                if(b != 0)
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
        private static void GetHalEvent(byte[] evt,byte[] ret)//数组长度[不安全]
        {
            //这个函数被C++调用，用于获取某个虚拟硬件状态，并将结果填充到ret里面
            HalEventTxWriter?.Write(false);//false表示get请求返回状态
            HalEventTxWriter?.Write(ArrToString(evt));
            HalEventTxWriter?.Flush();
            String str = HalEventRxReader?.ReadString();
            if(str != null)
            {
                StringToArr(str, ret);
            }
        }

        static void Main(string[] args)
        {
            var argMaps = GetArgsMap(args);

            if (argMaps.ContainsKey("f") == false)
            {
                Console.WriteLine("ERROR:no file used");
                return;
            }
            //载入相关函数
            string dllPath = argMaps["f"];
            
            if (argMaps.ContainsKey("leds"))
            {
                ledStream = new AnonymousPipeClientStream(PipeDirection.Out,argMaps["leds"]);
            }

            UartRx = new PipeStream[UART_COUNT];
            UartTx = new PipeStream[UART_COUNT];
            for(int i = 0; i < UART_COUNT; i++)
            {
                string rx = "iuart" + i, tx = "ouart" + i;
                if (argMaps.ContainsKey(rx))
                {
                    UartRx[i] = new AnonymousPipeClientStream(PipeDirection.In, argMaps[rx]);
                }
                if (argMaps.ContainsKey(tx))
                {
                    UartTx[i] = new AnonymousPipeClientStream(PipeDirection.Out,argMaps[tx]);
                }
            }

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

            try
            {
                VHDllLoader loader = new VHDllLoader(dllPath);
                //设置回调函数，用来和虚拟环境交互,必须使用CdeclFunc(有返回值)或者CdeclAction(无返回值)，不能使用普通的Func和Action
                //loader.SetupLinks(VHDllLoader.CallBacks.CB_USART_READ, new CdeclFuncInt(Pipe_USART_Read));
                //loader.SetupLinks(VHDllLoader.CallBacks.CB_USART_WRITE, new CdeclActionInt(Pipe_USART_Write));
                loader.SetupLinks(VHDllLoader.CallBacks.CB_UART_READ, new CdeclFuncByteByte(Pipe_UartRead));
                loader.SetupLinks(VHDllLoader.CallBacks.CB_UART_WRITE, new CdeclActionByteByte(Pipe_UartWrite));
                loader.SetupLinks(VHDllLoader.CallBacks.CB_LEDSET, new CdeclActionByteByte(Pipe_LedSet));
                loader.SetupLinks(VHDllLoader.CallBacks.CB_DO_HAL_EVENT, new CdeclActionByteA(DoHalEvent));
                loader.SetupLinks(VHDllLoader.CallBacks.CB_GET_HAL_EVENT, new CdeclActionByteAByteA(GetHalEvent));

                //这里已经在运行
                int r = loader.RunMain();

                //这里运行结束
                //do clean
                loader.Close();
                //output return value(useless)
                Console.WriteLine("dll [" + dllPath + "] run end.return value:" + r);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.ToString());
                using(var writer = File.AppendText("VHClientError.log"))
                {
                    writer.WriteLine(e.ToString());
                }
            }
            //Debug only
            //Console.ReadKey();
            
        }

        //IO参数处理
        private static Dictionary<String, String> GetArgsMap(string[] args)
        {
            string laststr = "";
            bool doing = false;
            Dictionary<String, String> dictionary = new Dictionary<string, string>();
            foreach (var s in args)
            {
                if (doing)
                {
                    doing = false;
                    dictionary[laststr] = s;
                }
                else if (s.StartsWith("-"))
                {
                    int i = 0;
                    while (s[i] == '-')
                        i++;
                    laststr = s.Substring(i);
                    doing = (laststr != "");
                }
            }
            return dictionary;
        }
    }
}
