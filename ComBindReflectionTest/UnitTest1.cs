using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IoTSimulate;

namespace ComBindReflectionTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ComIOBindToComBaseByConnector()
        {
            //初始化两个设备
            ComIO comIO = new MyTestComDev();
            ComBase comBase = new MyVirtualCom();
            //初始化一个连接线Connector
            ComConnector connector = new ComConnector();
            //链接
            connector.Bind(comIO, comBase);
            Assert.IsNotNull(comBase.Connector);

            Console.WriteLine("ComBase send data to ComIO");
            (comBase as MyVirtualCom)?.SendTest();

            Console.WriteLine("ComIO receive data from real device to ComBase");
            (comIO as MyTestComDev)?.SendTest();

            //移除连接线
            connector.Close();
            Assert.IsNull(comBase.Connector);
        }
    }

    class MyTestComDev : ComIO
    {
        public override void Close()
        {
            base.Close();
        }

        public override void WhenDataSend(byte[] data, int offset, int len)
        {
            Console.WriteLine("ComDev ReceiveData:");
            for (int i = 0; i < len; i++)
                Console.Write("{0},",data[offset + i]);
            Console.WriteLine();
        }

        public void SendTest()
        {
            ToConnector(new byte[] { 0, 0, 0 }, 0, 3);
        }
    }
    class MyVirtualCom : ComBase
    {
        public override void OnDataReceive(byte[] data, int offset, int len)
        {
            Console.WriteLine("Com ReceiveData:");
            for (int i = 0; i < len; i++)
                Console.Write("{0},", data[offset + i]);
            Console.WriteLine();
        }

        public void SendTest()
        {
            ToConnector(new byte[] { 1, 2, 3 }, 0, 3);
        }
    }
}
