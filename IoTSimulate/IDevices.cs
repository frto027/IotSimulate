using System;
using System.Collections.Generic;
using System.Text;

namespace IoTSimulate
{
    /// <summary>
    /// 连接线，连接线也是特殊的设备
    /// </summary>
    public interface IConnector:IDevices
    {
    }
    /// <summary>
    /// 设备
    /// </summary>
    public interface IDevices
    {
        //约定，如果有将以反射形式调用
        //void Update();
        void Close();
    }
}
