using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTSimulate
{
    public class ComTempSensor : ComBase
    {

        public IEnviroment enviroment = null;

        public override void OnDataReceive(byte[] data, int offset, int len)
        {
            //什么都不做
        }

        public void Update()
        {
            byte ret = 0xFF;
            if(enviroment != null)
            {
                int temp = (int)enviroment.GetTemperature();
                ret = (byte)(temp + 127);
            }
            ToConnector(new byte[] { ret }, 0, 1);
        }
    }
}
