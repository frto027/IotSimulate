using System;
using System.Reflection;
using System.Linq;

namespace IoTSimulate
{

    delegate void ComDataEventHandler(Byte[] data,int offset, int len);
    /// <summary>
    /// Com实际设备驱动类实现ComIO接口
    /// </summary>
    public abstract class ComIO:IDevices
    {
        private ComConnector _connector = null;
        public ComConnector Connector
        {
            get { return _connector; }
        }

        private ComDataEventHandler eventHandler = null;


        /// <summary>
        /// Connector调用此方法向ComDev发送数据，ComDev需要重写此方法将数据发送至真实串口
        /// </summary>
        /// <param name="data">数据内容</param>
        /// <param name="offset">起始位置</param>
        /// <param name="len">欲写入的长度</param>
        public abstract void WhenDataSend(Byte[] data,int offset,int len);
        /// <summary>
        /// 当ComDev产生数据并发送到Connector时触发
        /// </summary>
        public void ToConnector(Byte[] data, int offset, int len)
        {
            eventHandler?.Invoke(data, offset, len);
        }

        /// <summary>
        /// 绑定监听事件，当Com收到数据时调用这个eventHandler
        /// 被ComConnector反射调用
        /// </summary>
        /// <param name="eventHandler">回掉时间，传入null表示不使用</param>
        private void BindReceiveEvent(ComDataEventHandler eventHandler)
        {
            this.eventHandler = eventHandler;
        }
        /// <summary>
        /// 重写记得调用base.Close()
        /// </summary>
        public virtual void Close()
        {
            _connector?.Close();
        }
    }
    /// <summary>
    /// 用ComConnector绑定ComBase和IComIO
    /// </summary>
    /// 调试标记：是否需要Close掉IComIO
    public class ComConnector:IConnector
    {
        private ComIO comDev;
        private ComBase com;

        public void Bind(ComIO comDev,ComBase com)
        {
            this.comDev = comDev;
            this.com = com;

            ConnectorToComBase(com, this);


            HandlerToComIO(comDev ,com.OnDataReceive);
        }

        public void ComDevWrite(Byte[] data, int offset, int len)
        {
            comDev.WhenDataSend(data, offset, len);
        }
        

        public void Close()
        {
            if(comDev != null)
            {
                ConnectorToComBase(com, null);

                HandlerToComIO(comDev, null);
                comDev = null;
                com = null;
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
        /// <summary>
        /// 反射调用comdev.BindReceiveEvent
        /// </summary>
        private void HandlerToComIO(ComIO comdev, ComDataEventHandler handler)
        {
            var flist = typeof(ComIO).GetMethod("BindReceiveEvent", BindingFlags.NonPublic|BindingFlags.Instance);
            flist.Invoke(comdev,new Object[] { handler });
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
        /// <summary>
        /// 当有数据到达时调用此方法写到Connector
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="len"></param>
        protected void ToConnector(Byte[] data, int offset, int len)
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
