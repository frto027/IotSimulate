using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTSimulate
{
    public partial class VtmDev
    {
        private bool LedLock = false;//是否关闭Led显示

        [VtmFunction(VtmFunctionAttribute.FunctionType.Init)]
        private void Init_LedLock()
        {
            //DEBUG HAL DEVICE:HAL SWITCH
            //状态设置事件
            DoHalEvent += (string s) =>
            {
                //Console.WriteLine(s);
                if (s.StartsWith("led "))
                {
                    if (s == "led on")
                    {
                        LedLock = false;
                    }
                    if (s == "led off")
                    {
                        LedLock = true;
                    }
                }
            };
            //状态获取事件
            GetHalEventList.Add((string s) =>
            {
                if (s.StartsWith("led "))
                {
                    if (s == "led getlock")
                    {
                        return LedLock ? "T" : "F";//返回string表示处理此结果
                    }
                }
                return null;//返回null表示不处理此结果
            });
        }
    }
}
