using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTSimulate
{
    /// <summary>
    /// 按键模型
    /// </summary>
    public partial class VtmDev
    {
        public const int TrigKeyCount = 4;
        //keys[i]表示第i个按键还没有被处理的事件次数
        private int[] keys = new int[TrigKeyCount];

        public bool TrigKeyActive = false;

        [VtmFunction(VtmFunctionAttribute.FunctionType.Init)]
        private void Init_TrigKey()
        {
            for(int i = 0; i < TrigKeyCount; i++)
            {
                keys[i] = 0;
            }
        }

        public void TrigKeyPress(int which)
        {
            if (TrigKeyActive == false)
                return;
            if(which >= 0 && which < TrigKeyCount)
            {
                keys[which]++;
            }
        }

        [VtmFunction(VtmFunctionAttribute.FunctionType.HalGetEvent)]
        private string GetHalEvent_TrigKey(string s)
        {
            if (s.StartsWith("trigkey"))
            {
                if (TrigKeyActive == false)
                    return "F";

                s = s.Substring(7);
                int r;
                if(int.TryParse(s,out r))
                {
                    if(r >= TrigKeyCount || r < 0||keys[r]<=0)
                    {
                        return "F";
                    }
                    else
                    {
                        keys[r]--;
                        return "T";
                    }
                }
                else
                {
                    return "F";
                }
            }
            return null;
        }


    }
}
