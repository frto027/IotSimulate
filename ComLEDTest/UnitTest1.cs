using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IoTSimulate;
using ComLED;

namespace ComLEDTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void LEDShouldDisplay()
        {
            //准备Com设备和LED数码管
            ComBase comIO = new MyComDevice();
            ComLED.ComLED led = new ComLED.ComLED();
            //连线
            ComConnector connector = new ComConnector();
            connector.Bind(comIO, led.com);

            Assert.AreEqual(led.A, '0');
            Assert.AreEqual(led.B, '0');

            //串口发送
            ((MyComDevice)comIO).Display(0xFC);

            Assert.AreEqual(led.A, 'F');
            Assert.AreEqual(led.B, 'C');
            //断开连线
            connector.Close();

            ((MyComDevice)comIO).Display(0xDA);

            Assert.AreNotEqual(led.A, 'D');
            Assert.AreNotEqual(led.B, 'A');

            Assert.AreEqual(led.A, 'F');
            Assert.AreEqual(led.B, 'C');
        }
    }

    public class MyComDevice : ComBase
    {
        public void Display(byte data)
        {
            ToConnector(new byte[] { data }, 0, 1);
        }

        public override void OnDataReceive(byte[] data, int offset, int len)
        {
            throw new NotImplementedException();
        }
    }
}
