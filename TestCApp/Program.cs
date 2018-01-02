using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace TestCApp
{
    class Program
    {
        static void Main(string[] args)
        {
            SerialPort serial = new SerialPort();
            serial.PortName = "CM1";
            
            while (true) ;
        }
    }
}
