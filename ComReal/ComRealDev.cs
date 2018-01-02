using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IoTSimulate;

using System.IO.Ports;

namespace ComReal
{
    public class ComRealDev : ComBase
    {
        SerialPort port;
        public ComRealDev(ComRealDevSetthings setthings)
        {
            port = new SerialPort()
            {
                PortName = setthings.portName,
                BaudRate = setthings.baudrate,
                Parity = setthings.parity,
                DataBits = setthings.databits,
                StopBits = setthings.stopBits
            };
            port.DataReceived += Port_DataReceived;
        }

        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int i = port.BytesToRead;
            byte[] data = new byte[i];
            port.Read(data, 0, i);

            ToConnector(data, 0, i);
        }

        public override void OnDataReceive(byte[] data, int offset, int len)
        {
            if (port.IsOpen)
                port.Write(data, offset, len);
        }

        public void OpenPort()
        {
            port.Open();
        }

        public void ClosePort()
        {
            port.Close();
        }
        
        
    }
    public struct ComRealDevSetthings
    {
        public String portName;
        public int baudrate;
        public Parity parity;
        public int databits;
        public StopBits stopBits;
    }


}
