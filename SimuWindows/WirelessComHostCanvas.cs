using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using IoTSimulate;
using System.Windows.Threading;
using System.Windows.Input;
using System.Windows.Media;
namespace SimuWindows
{
    class WirelessComHostCanvas:DragCanvas
    {
        public WLComHost WLComHost = new WLComHost();

        private ComCanvas ComCanvas;

        DispatcherTimer timer = new DispatcherTimer();
        public WirelessComHostCanvas(GlobalGUIManager global) : base(global.rootcvs)
        {
            Width = 250;
            Height = 60;

            Background = Brushes.OrangeRed;
            Children.Add(new Label()
            {
                IsHitTestVisible = false,
                Margin = new Thickness(30, 5, 0, 0),
                Content = "wireless(host) <-> com"
            });
            AddClickPoint(new RemoveClickPoint(0, 0, this));

            ComCanvas = new ComCanvas(30, 30, global, WLComHost.comBase);
            AddClickPoint(ComCanvas);

            timer.Tick += Update;
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Start();
        }

        private void Update(object sender, EventArgs e)
        {
            WLComHost.Update();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            ComCanvas.Update();
        }

        public override void Remove()
        {
            ComCanvas.Remove();
            WLComHost.Close();
            base.Remove();

        }
    }
}
