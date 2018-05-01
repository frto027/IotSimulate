using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using IoTSimulate;

namespace SimuWindows.VtmModule
{
    [ModulePosition(Position.A,Position.B,Position.C,Position.D)]
    class BpwsnModule:VtmModuleBase
    {
        WirelessSignal signal;
        public BpwsnModule(VtmDev dev,GlobalGUIManager global) : base(dev,global)
        {
            Background = Brushes.Gray;
            signal = new WirelessSignal(global.rootcvs, dev.WirelessPackageDevice, 20, 20);
            Children.Add(signal);
        }

        public override void Remove()
        {
            signal.Remove();
            base.Remove();
        }
    }
}
