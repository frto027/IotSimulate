using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Pipes;

namespace VHClient
{
    partial class Program
    {
        private const int BPWSN_PACKAGE_SIZE = 16;

        private static PipeStream BPWSN_Rx, BPWSN_Tx;
        static void BPWSN_Read(byte[] b)
        {
            for(int i = 0; i < BPWSN_PACKAGE_SIZE; i++)
            {
                b[i] = (byte)(BPWSN_Rx?.ReadByte() ?? 0);
            }
        }

        static void BPWSN_Send(byte[] b)
        {
            for(int i = 0; i < BPWSN_PACKAGE_SIZE; i++)
            {
                BPWSN_Tx?.WriteByte(b[i]);
                BPWSN_Tx?.Flush();
            }
        }

        static void BPWSN_Init(Dictionary<string,string> argsMap)
        {
            if (argsMap.ContainsKey("bpwsni"))
            {
                BPWSN_Rx = new AnonymousPipeClientStream(argsMap["bpwsni"]);
            }
            if (argsMap.ContainsKey("bpwsno"))
            {
                BPWSN_Tx = new AnonymousPipeClientStream(argsMap["bpwsno"]);
            }
        }
    }
}
