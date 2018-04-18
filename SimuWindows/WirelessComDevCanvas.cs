using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Input;
using IoTSimulate;
using System.Windows.Media;

namespace SimuWindows
{
    class WirelessComDevCanvas:DragCanvas
    {
        WLCom WLCom = new WLCom();
        ComCanvas ComCanvas;
        WirelessSignal signal;

        DispatcherTimer timer = new DispatcherTimer();
        public WirelessComDevCanvas(GlobalGUIManager global) : base(global.rootcvs)
        {
            Height = 60;
            Width = 180;
            Background = Brushes.AliceBlue;
            //Remove
            AddClickPoint(new RemoveClickPoint(0, 0, this));

            //Title
            Children.Add(new Label()
            {
                Margin = new Thickness(40, 5, 0, 0),
                IsHitTestVisible = false,
                Content = "WirelessDev<->Com"
            });

            //WLCom


            //ComBase
            AddClickPoint(ComCanvas = new ComCanvas(40, 40, global,WLCom.comBase));

            //signal
            Children.Add(signal = new WirelessSignal(global.rootcvs, WLCom, 20, 20));
            //timer
            timer.Tick += Update;
            timer.Interval = TimeSpan.FromMilliseconds(200);
            timer.Start();

        }

        private void Update(object sender, EventArgs e)
        {
            WLCom.Update();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
        }

        public override void Remove()
        {
            timer.Stop();
            ComCanvas.Remove();
            signal.Remove();
            WLCom.Close();
            base.Remove();
        }
    }
}
