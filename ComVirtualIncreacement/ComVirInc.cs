using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IoTSimulate;

namespace ComVirtualIncreacement
{
    public class ComVirInc : ComBase
    {
        byte s = 0;
        public byte NextOut { get { return s; } }

        public override void OnDataReceive(byte[] data, int offset, int len)
        {
            Console.WriteLine("Data receive by Vir Com");
        }

        public void SendData()
        {
            ToConnector(new byte[] { s++ }, 0, 1);
        }
    }
}
