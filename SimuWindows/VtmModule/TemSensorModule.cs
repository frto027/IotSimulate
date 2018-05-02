using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using IoTSimulate;

namespace SimuWindows.VtmModule
{
    [ModulePosition(Position.A,Position.B,Position.C,Position.D)]
    class TemSensorModule : EnvModuleBase
    {

        public TemSensorModule(VtmDev dev,GlobalGUIManager global) : base(dev, global)
        {
            Children.Add(new Canvas()
            {
                Margin = new System.Windows.Thickness(SensorPosition.X, SensorPosition.Y, 0, 0),
                Width = 5,
                Height = 5,
                Background = Brushes.Yellow
            });
        }
        public override void CancelEnv(IEnviroment enviroment)
        {
            if(VtmDev.TemSensorEnv == enviroment)
            {
                VtmDev.TemSensorEnv = null;
            }
        }

        public override void SetEnv(IEnviroment enviroment)
        {
            VtmDev.TemSensorEnv = enviroment;
        }
        public override void Remove()
        {
            VtmDev.TemSensorEnv = null;
            base.Remove();
        }
    }
}
