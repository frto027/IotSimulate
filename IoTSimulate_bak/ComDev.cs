using System;
using System.Reflection;
using System.Globalization;

namespace IoTSimulate
{

    public delegate void ComDataEventHandler(Byte[] data,int offset, int len);
    /// <summary>
    /// Com实际设备驱动类实现ComIO接口
    /// </summary>
    public interface IComIO:IDevices
    {

        /// <summary>
        /// 当向Com写入数据时调用此方法
        /// </summary>
        /// <param name="data">数据内容</param>
        /// <param name="offset">起始位置</param>
        /// <param name="len">欲写入的长度</param>
        void ComWrite(Byte[] data,int offset,int len);
        /// <summary>
        /// 绑定监听事件，当Com收到数据时调用这个eventHandler
        /// </summary>
        /// <param name="eventHandler">回掉时间，传入null表示不使用</param>
        void BindReceiveEvent(ComDataEventHandler eventHandler);
    }
    /// <summary>
    /// 用ComConnector绑定ComBase和IComIO
    /// </summary>
    public class ComConnector:IConnector
    {
        private IComIO comDev;
        private ComBase com;

        void Bind(IComIO comDev,ComBase com)
        {
            this.comDev = comDev;
            this.com = com;

            ConnectorToComBase(com, this);

            comDev.BindReceiveEvent(com.OnDataReceive);
        }

        public void ComDevWrite(Byte[] data, int offset, int len)
        {
            comDev.ComWrite(data, offset, len);
        }
        

        public void Close()
        {
            if(comDev != null)
            {
                ConnectorToComBase(com, null);

                comDev.BindReceiveEvent(null);
                comDev = null;
                com = null;
            }
        }

        private void ConnectorToComBase(ComBase com,ComConnector connector)
        {
            
            Type comType = typeof(ComBase);
            comType.Get
        }

    }
    /// <summary>
    /// 所有虚拟Com继承ComBase类
    /// </summary>
    public abstract class ComBase:IDevices
    {
        private ComConnector _connector = null;
        public ComConnector Connector {
            get { return _connector; }
        }

        protected void Write(Byte[] data, int offset, int len)
        {
            if (Connector != null)
                Connector.ComDevWrite(data, offset, len);
        }
        public abstract void OnDataReceive(Byte[] data, int offset, int len);

        public void Close()
        {
            Connector?.Close();
        }
    }
}
