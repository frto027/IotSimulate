using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IoTSimulate;

namespace WLDevUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            byte[] b1 = new byte[1024], b2 = new byte[1024];

            for(int i = 0; i < 1024; i++)
            {
                b1[i] = (byte)i;
            }

            var dev = new WLDev();
            var dev2 = new WLDev();
            var host = new WLHostDev();
            dev.SetHost(host);
            dev2.SetHost(host);
            dev.SendMessage(b1, 0, 1024);

            Assert.AreEqual(0, dev.RemainData);
            Assert.AreEqual(1024, dev2.RemainData);
            Assert.AreEqual(1024, host.RemainData);
            
        }

        [TestMethod]
        public void TestChar()
        {
            string s = "222";
            char[] arr = { (char)50, (char)50, (char)50, (char)0, (char)50 };
            Console.WriteLine(new String(arr));
        }
    }
}
