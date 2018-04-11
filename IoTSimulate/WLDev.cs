using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace IoTSimulate
{
    /// <summary>
    /// 用于描述无线设备
    /// </summary>
    public class WLDev : IDevices
    {
        private Queue<byte> dataQueue = new Queue<byte>();

        private WLHostDev host = null;

        public long RemainData { get { return dataQueue.Count; } }

        public int ReadMessage(byte []buff,int offset,int len)
        {
            int i = 0;
            while(i < len && dataQueue.Count > 0)
            {
                buff[offset++] = dataQueue.Dequeue();
                i++;
            }
            return i;
        }
        /// <summary>
        /// 向网络中其它设备广播信息或向中心节点发送信息
        /// </summary>
        public void SendMessage(byte [] buff,int offset,int len,bool broadcast = true)
        {
            host?.MesssageBroadcast(buff, offset, len, broadcast, this);
        }

        public void SetHost(WLHostDev hostDev)
        {
            if(host != hostDev)
            {
                host?.RemoveDev(this);
                host = hostDev;
                host?.HoldDev(this);
            }
        }

        public virtual void Close()
        {
            SetHost(null);
        }
    }

    public class WLHostDev : WLDev
    {
        private LinkedList<WLDev> devs = new LinkedList<WLDev>();

        public WLHostDev()
        {
            SetHost(this);
        }

        public void MesssageBroadcast(byte [] buff,int offset,int len,bool broadcast,WLDev sender = null)
        {
            if (broadcast)
            {
                foreach (var d in from d in devs where d != sender select d)
                {
                    var m = GetMemory(d);
                    for (int i = 0, k = offset; i < len; i++, k++)
                    {
                        m.Enqueue(buff[k]);
                    }
                }
            }
            else
            {
                var m = GetMemory(this);
                for (int i = 0, k = offset; i < len; i++, k++)
                {
                    m.Enqueue(buff[k]);
                }
            }
           
        }

        public Queue<byte> GetMemory(WLDev dev)
        {
            return typeof(WLDev).GetField("dataQueue", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(dev) as Queue<byte>;
        }

        public void HoldDev(WLDev dev)
        {
            devs.AddLast(dev);
        }
        public void RemoveDev(WLDev dev)
        {
            devs.Remove(dev);
        }
        public override void Close()
        {
            base.Close();
            foreach (var d in devs)
                d.SetHost(null);
        }
    }
}
