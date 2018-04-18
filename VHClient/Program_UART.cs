using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VHClient
{
    partial class Program
    {
        private const int UART_COUNT = 3;
        private static PipeStream[] UartTx, UartRx;
        private static void UartInit(Dictionary<string,string> argMaps)
        {
            UartRx = new PipeStream[UART_COUNT];
            UartTx = new PipeStream[UART_COUNT];
            for (int i = 0; i < UART_COUNT; i++)
            {
                string rx = "iuart" + i, tx = "ouart" + i;
                if (argMaps.ContainsKey(rx))
                {
                    UartRx[i] = new AnonymousPipeClientStream(PipeDirection.In, argMaps[rx]);
                }
                if (argMaps.ContainsKey(tx))
                {
                    UartTx[i] = new AnonymousPipeClientStream(PipeDirection.Out, argMaps[tx]);
                }
            }
        }

        private static void Pipe_UartWrite(byte uart, byte value)
        {
            if (uart < UartTx.Length)
            {
                UartTx[uart]?.WriteByte(value);
                UartTx[uart]?.Flush();
            }
        }
        private static byte Pipe_UartRead(byte uart)
        {
            if (uart < UartRx.Length && UartRx[uart] != null)
            {
                return (byte)UartRx[uart].ReadByte();
            }
            return 0;
        }
    }
}
