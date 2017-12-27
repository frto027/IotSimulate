using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IoTSimulate;

/// <summary>
/// 实现数码管
/// </summary>
namespace ComLED
{
    public class ComLED
    {
        byte text = 0;

        public char A
        {
            get
            {
                int x = (text >> 4) & 0xF;
                if (x < 10)
                    return (char)('0' + x);
                else
                    return (char)('A' + x - 10);
            }
        }
        public char B
        {
            get
            {
                int x = text & 0xF;
                if (x < 10)
                    return (char)('0' + x);
                else
                    return (char)('A' + x - 10);
            }
        }

        public readonly ComBase com;

        public ComLED()
        {
            com = new LedVirCom(this);
        }


        class LedVirCom : ComBase
        {
            ComLED led = null;
            public LedVirCom(ComLED led)
            {
                this.led = led;
            }
            public override void OnDataReceive(byte[] data, int offset, int len)
            {
                if (len > 0)
                    led.text = data[offset + len - 1];
            }
        }
    }


}
