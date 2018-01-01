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
        private Canvas c1, c2;
        private Label L1, L2;

        private ComCanvas comCanvas;
        public ComLedCanvas(GlobalGUIManager manager) : base(manager.rootcvs)
        {
            Width = 100;
            Height = 80;
            Background = Brushes.Green;

            Canvas titleCvs = new Canvas
            {
                Margin = new Thickness(15, 0, 0, 0),
                Height = 25,
                Width = 80,
                Background = new VisualBrush
                {

                    Stretch = Stretch.Uniform,
                    Visual = new Label
                    {
                        Content = "ComLed",
                        Foreground = Brushes.LightGreen
                    }
                }
            };
            Children.Add(titleCvs);
            dragList.Add(titleCvs);


            AddClickPoint(new RemoveClickPoint(0, 0, this));

            c1 = new Canvas
            {
                Height = 80,
                Width = 40,
                Margin = new System.Windows.Thickness(0, 5, 0, 0),
                Background = new VisualBrush()
                {
                    Visual = L1 = new Label() { Foreground = Brushes.Red },
                    Stretch = Stretch.Uniform,
                }
            };

            c2 = new Canvas
            {
                Height = 80,
                Width = 40,
                Background = new VisualBrush() {
                    Visual = L2 = new Label() { Foreground = Brushes.Red },
                    Stretch = Stretch.Uniform
                },
                Margin = new System.Windows.Thickness(20, 5, 0, 0)
            };

            Children.Add(c1);
            Children.Add(c2);

            dragList.Add(c1);//当鼠标在从c1 c2上按下时也要响应拖拽
            dragList.Add(c2);

            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += Update;

            timer.Start();

            //添加一个Com口连接点
            AddClickPoint(comCanvas = new ComCanvas(70, 40, manager,led.com));
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
