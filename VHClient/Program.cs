using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Pipes;
using System.IO;
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
        
        private static void Pipe_UartWrite(byte uart,byte value)
        {
            if (uart < UartTx.Length)
            {
                UartTx[uart]?.WriteByte(value);
                UartTx[uart]?.Flush();
            }

            //Console.WriteLine("UART " + uart + " value " + value);
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
            //Console.WriteLine("LED " + led + " value " + value);
        }

        

        static void Main(string[] args)
        {
            var argMaps = GetArgsMap(args);
            //载入相关函数
            string dllPath = argMaps["f"];

            if (argMaps.ContainsKey("leds"))
            {
                ledStream = new AnonymousPipeClientStream(argMaps["leds"]);
            }

            UartRx = new PipeStream[UART_COUNT];
            UartTx = new PipeStream[UART_COUNT];
            for(int i = 0; i < UART_COUNT; i++)
            {
                string rx = "iuart" + i, tx = "ouart" + i;
                if (argMaps.ContainsKey(rx))
                {
                    UartRx[i] = new AnonymousPipeClientStream(argMaps[rx]);
                }
                if (argMaps.ContainsKey(tx))
                {
                    UartTx[i] = new AnonymousPipeClientStream(argMaps[tx]);
                }
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
            }
            //Debug only
            Console.ReadKey();
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
