using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IoTSimulate
{
    class StreamFowardThread
    {
        bool Closed = false;
        Stream from, to;

        Thread thread;

        public StreamFowardThread(Stream from,Stream to)
        {
            this.from = from;
            this.to = to;
        }

        public void Start()
        {
            if(thread == null)
            {
                thread = new Thread(Foward);
                thread.Start(this);
            }
        }

        private static void Foward(object sft)
        {
            StreamFowardThread s = (StreamFowardThread)sft;
            while(s.Closed == false)
            {
                Console.WriteLine("begin read");
                int b = s.from.ReadByte();
                Console.WriteLine("Read ok " + b);
                if(b == -1)
                {
                    s.to.Close();
                    return;
                }
                else
                {
                    s.to.WriteByte((byte)b);
                    s.to.Flush();
                }
            }
        }

        public void Close()
        {
            Closed = true;
            thread.Abort();//okay
        }
    }
}
