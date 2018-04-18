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
        private static PipeStream ledStream;

        private static void LedInit(Dictionary<string,string> argMaps)
        {
            if (argMaps.ContainsKey("leds"))
            {
                ledStream = new AnonymousPipeClientStream(PipeDirection.Out, argMaps["leds"]);
            }
        }

        private static void Pipe_LedSet(byte led, byte value)
        {
            ledStream?.WriteByte(led);
            ledStream?.WriteByte(value);
            ledStream?.Flush();
        }
    }
}
