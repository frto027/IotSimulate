using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTSimulate
{
    /// <summary>
    /// 模拟一个环境
    /// </summary>
    public interface IEnviroment
    {
        float GetTemperature();
        float GetLight();
    }
}
