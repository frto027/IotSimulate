using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IoTSimulate;

namespace SimuWindows
{
    /// <summary>
    /// 所有需要获取Enviroment环境的Canvas设备实现此接口（由EnviromentCanvas调用）
    /// </summary>
    interface IEnviromentSetable
    {
        /// <summary>
        /// 返回坐标来判断是否在某个环境之内
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        void GetXY(out float x, out float y);
        /// <summary>
        /// 设置环境
        /// </summary>
        /// <param name="enviroment"></param>
        void SetEnv(IEnviroment enviroment);
        /// <summary>
        /// 撤销环境(一般没必要)
        /// </summary>
        /// <param name="enviroment"></param>
        void CancelEnv(IEnviroment enviroment);
    }
}
