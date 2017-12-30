using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private Canvas c1, c2;
        private Label L1, L2;

        private ComCanvas comCanvas;
        public ComLedCanvas(GlobalGUIManager manager) : base(manager.rootcvs)
        {
            Width = 100;
            Height = 100;
            Background = new SolidColorBrush(Colors.Green);

            AddClickPoint(new RemoveClickPoint(0, 0, this));

            c1 = new Canvas();
            c1.Height = 40;
            c1.Width = 20;
            c1.Background = new VisualBrush(L1 = new Label());
            c1.Margin = new System.Windows.Thickness(20, 20, 0, 0);
            c2 = new Canvas();
            c2.Height = 40;
            c2.Width = 20;
            c2.Background = new VisualBrush(L2 = new Label());
            c2.Margin = new System.Windows.Thickness(40, 20, 0, 0);

            Children.Add(c1);
            Children.Add(c2);

            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += Update;

            timer.Start();

            //添加一个Com口连接点
            Children.Add(comCanvas = new ComCanvas(40, 60, manager));
        }

        public void Update(object sender, EventArgs e)
        {
            //显示LED A LED B到自身
            L1.Content = led.A.ToString();
            L2.Content = led.B.ToString();
        }
        //这里更新一下连线，以提升界面响应速度
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            comCanvas.Update();
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
