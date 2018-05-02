using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using IoTSimulate;
using System.Windows.Threading;

namespace SimuWindows
{
    class WLPackageDisplayCanvas:DragCanvas
    {
        private Label DisplayLabel = new Label() {IsHitTestVisible = false, Margin = new Thickness(30, 20, 0, 0),Content = "???" };
        WLPackageDev dev = new WLPackageDev();
        WirelessSignal signal;
        DispatcherTimer timer = new DispatcherTimer();
        public WLPackageDisplayCanvas(GlobalGUIManager global) : base(global.rootcvs)
        {
            Width = 40 + WLPackageDev.PACKAGE_SIZE * 30;
            Height = 40;
            Background = Brushes.DarkGreen;
            //title
            Children.Add(new Label()
            {
                Content = "PackageDisplay(DEBUG)",
                Margin = new Thickness(40, 0, 0, 0),
                IsHitTestVisible = false

            });
            //remove button
            AddClickPoint(new RemoveClickPoint(0, 0, this));

            //signal
            signal = new WirelessSignal(global.rootcvs, dev, 20, 20);
            Children.Add(signal);

            dev.OnPackageArrive = (b) => {
                string s = "";
                foreach(byte bt in b)
                {
                    s += bt.ToString() + " "; 
                }
                DisplayLabel.Content = s;
            };

            Children.Add(DisplayLabel);

            timer.Tick += Update;
            timer.Interval = TimeSpan.FromMilliseconds(200);
            timer.Start();
        }

        private void Update(object sender, EventArgs e)
        {
            dev.Update();
        }
        
        public override void Remove()
        {
            timer.Stop();
            signal.Remove();
            dev.Close();
            base.Remove();
        }

    }
}
