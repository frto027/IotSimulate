using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTSimulate
{
    /// <summary>
    /// 将封包转发至串口的WLPackageHost，同时将串口数据广播至其它节点
    /// </summary>
    public class WLComHost:WLHostDev
    {
        public readonly ComBase comBase;

        public WLComHost()
        {
            comBase = new WLComport(this);
        }

        public virtual void Update()
        {
            ((WLComport)comBase).Update();
        }
    }
    public class WLCom : WLDev
    {
        public readonly ComBase comBase;

        public WLCom()
        {
            comBase = new WLComport(this);
        }

        public virtual void Update()
        {
            ((WLComport)comBase).Update();
        }
    }

    class WLComport : ComBase
    {
        private WLDev host;
        public WLComport(WLDev h)
        {
            host = h;
        }

        public override void OnDataReceive(byte[] data, int offset, int len)
        {
            host.SendMessage(data, offset, len);
        }

        public void Update()
        {
            byte[] buff = new byte[1024];
            int len = host.ReadMessage(buff, 0, buff.Length);
            if (len > 0)
            {
                ToConnector(buff, 0, len);
            }
        }
    }

}
