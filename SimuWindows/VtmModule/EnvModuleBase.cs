using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using IoTSimulate;

namespace SimuWindows.VtmModule
{
    abstract class EnvModuleBase : VtmModuleBase,IEnviromentSetable
    {
        protected Point SensorPosition = new Point(40, 40);
        private Canvas rootcvs;

        public EnvModuleBase(VtmDev dev, GlobalGUIManager global) : base(dev, global)
        {
            EnviromentCanvas.EnvSetableList.Add(this);
            rootcvs = global.rootcvs;
        }

        public void GetXY(out float x, out float y)
        {
            var p = TranslatePoint(SensorPosition, rootcvs);
            x = (float)p.X;
            y = (float)p.Y;
        }

        public abstract void CancelEnv(IEnviroment enviroment);
        public abstract void SetEnv(IEnviroment enviroment);
    }
}
