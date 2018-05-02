using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTSimulate
{
    public partial class VtmDev
    {
        public IEnviroment TemSensorEnv;
        
        [VtmFunction(VtmFunctionAttribute.FunctionType.HalGetEvent)]
        private string HalGetEvent_TemSensor(string evt)
        {
            if (evt.StartsWith("temsensor"))
            {
                float tem = TemSensorEnv?.GetTemperature() ?? 0;
                return "[" + tem + "]";
            }
            return null;
        }
    }
}
