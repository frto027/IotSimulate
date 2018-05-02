using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ComLED;

namespace SimuWindows
{

    class ComLedCanvas : DragCanvas
    {
        DispatcherTimer timer = new DispatcherTimer();
        public ComLED.ComLED led = new ComLED.ComLED();
        private Label L1, L2,Lcount;

        private ComCanvas comCanvas;
        public ComLedCanvas(GlobalGUIManager manager) : base(manager.rootcvs)
        {
            Width = 100;
            Height = 80;


            SetupBackgrountStyle();

            Children.Add(new Label()
            {
                IsHitTestVisible = false,
                Content = "ComLed",
                FontSize = 16,
                Margin = new Thickness(18,0,0,0)
            });


            AddClickPoint(new RemoveClickPoint(0, 0, this));

            L1 = new Label()
            {
                Foreground = Brushes.Red,
                Margin = new Thickness(5, 20, 0, 0),
                FontSize = 28,
                IsHitTestVisible = false
            };
            L2 = new Label()
            {
                Foreground = Brushes.Red,
                Margin = new Thickness(25, 20, 0, 0),
                FontSize = 28,
                IsHitTestVisible = false
            };
            Lcount = new Label()
            {
                Margin = new Thickness(10, 55, 0, 0)
            };
            
            Children.Add(L1);
            Children.Add(L2);
            Children.Add(Lcount);

            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += Update;

            timer.Start();

            //添加一个Com口连接点
            AddClickPoint(comCanvas = new ComCanvas(60, 40, manager,led.com));
        }

        public void Update(object sender, EventArgs e)
        {
            //显示LED A LED B到自身
            L1.Content = led.A.ToString();
            L2.Content = led.B.ToString();
            Lcount.Content = "count:" + led.count;
        }


        public override void Remove()
        {
            comCanvas.Remove();
            base.Remove();
            timer.Tick -= Update;
            timer.Stop();
        }

    }
    
}
