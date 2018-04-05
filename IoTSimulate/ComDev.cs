using System;
using System.Reflection;
using System.Linq;

namespace IoTSimulate
{

    delegate void ComDataEventHandler(Byte[] data,int offset, int len);

    /// <summary>
    /// 绑定串口，Rxd--Txd Txd--Rxd 
    /// </summary>
    /// 调试标记：是否需要Close掉IComIO
    public class ComConnector:IConnector
    {
        private ComBase com1,com2;

        public void Bind(ComBase com1,ComBase com2)
        {
            if (com1 == null)
                return;
            this.com1 = com1;
            this.com2 = com2;

            ConnectorToComBase(com1, this);
            SetSendDelegate(com1, com2.OnDataReceive);

            ConnectorToComBase(com2, this);
            SetSendDelegate(com2, com1.OnDataReceive);
            
        }
        
        public void Close()
        {
            if(com1 != null)
            {
                ConnectorToComBase(com1, null);
                SetSendDelegate(com1, null);
                ConnectorToComBase(com2, null);
                SetSendDelegate(com2, null);

                com1 = null;
                com2 = null;
            }
        }
        /// <summary>
        /// 通过反射设置com._connector = connector;
        /// </summary>
        private void ConnectorToComBase(ComBase com,ComConnector connector)
        {
            FieldInfo field = null;

            var fs = typeof(ComBase).GetFields(BindingFlags.NonPublic|BindingFlags.Instance);
            //Linq
            var x = 
                from f in fs where f.Name == "_connector" select f;
            field = x.First();

            field.SetValue(com, connector);
        }

        private void SetSendDelegate(ComBase com, ComDataEventHandler callback)
        {
            FieldInfo field = null;

            var fs = typeof(ComBase).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            //Linq
            var x =
                from f in fs where f.Name == "_sendToConnector" select f;
            field = x.First();

            field.SetValue(com, callback);
        }
    }
    /// <summary>
    /// 所有虚拟Com继承ComBase类
    /// </summary>
    public abstract class ComBase:IDevices
    {
        private ComConnector _connector = null;
        private ComDataEventHandler _sendToConnector = null;
        public ComConnector Connector {
            get { return _connector; }
        }
        /// <summary>
        /// 当有数据到达时调用此方法写到Connector
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="len"></param>
        protected void ToConnector(Byte[] data, int offset, int len)
        {
            if (Connector != null && _sendToConnector != null)
                _sendToConnector(data, offset, len);
        }
        public abstract void OnDataReceive(Byte[] data, int offset, int len);

        public virtual void Close()
        {
            Connector?.Close();
        }
    }
}
