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
        /*
        [VtmFunction(VtmFunctionAttribute.FunctionType.Init)]
        private void Init_LedLock()
        {
            //DEBUG HAL DEVICE:HAL SWITCH

            //这里注册事件到DoHalEvent，GetHalEvent，以便能够在合适的时候被调用。
            //DoHalEvent有一个String参数
            //GetHalEvent输入一个String参数，返回值是String，如果不是自己的事件，需要返回null。
            
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
        */
        //上面Init函数是一种写法，这里是另一种使用注解的写法，这种写法更舒服一点。。
        [VtmFunction(VtmFunctionAttribute.FunctionType.HalDoEvent)]
        private void DoHalEvent_LedLock(string s)
        {
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
        }

        [VtmFunction(VtmFunctionAttribute.FunctionType.HalGetEvent)]
        private string GetHalEvent_LedLock(string s)
        {
            if (s.StartsWith("led "))
            {
                if (s == "led getlock")
                {
                    return LedLock ? "T" : "F";//返回string表示处理此结果
                }
            }
            return null;//返回null表示不处理此结果
        }
    }
}
