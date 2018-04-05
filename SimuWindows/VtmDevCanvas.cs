using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using IoTSimulate;
using System.Windows.Controls;
using System.Windows.Input;

namespace SimuWindows
{
    class VtmDevCanvas:DragCanvas
    {
        VtmDev dev;
        GlobalGUIManager global;

        readonly byte[] leds = new byte[VtmDev.LED_COUNT];
        readonly ComCanvas [] coms = new ComCanvas[VtmDev.UART_COUNT];

        Label StatusLabel = new Label()
        {
            Margin = new Thickness(85, 0, 0, 0),
            IsHitTestVisible = false,
            Content = "Ready",
            Background = Brushes.YellowGreen
        };

        LedCanvasControl[] LedCanvasControls = new LedCanvasControl[VtmDev.LED_COUNT];

        DispatcherTimer timer = new DispatcherTimer();

        ClickEventPoint RunButton = new ClickEventPoint(60, 0)
        {
            Background = Brushes.Green,
            Width = 20,Height = 20
        };

        public VtmDevCanvas(GlobalGUIManager global,String binpath) : base(global.rootcvs)
        {
            this.global = global;

            Width = 350;
            Height = 200;
            Background = Brushes.WhiteSmoke;

            AddClickPoint(new RemoveClickPoint(0, 0, this));
            AddClickPoint(RunButton);

            
            //LED CANVAS
            for(int i = 0; i < leds.Length; i++)
            {
                LedCanvasControls[i] = new LedCanvasControl()
                {
                    Width = 40,
                    Height = 80,
                    Margin = new Thickness(20 + 40 * i, 20, 0, 0),
                    IsHitTestVisible = false
                };
                Children.Add(LedCanvasControls[i]);
            }


            Children.Add(StatusLabel);
            //Dev
            dev = new VtmDev(binpath);
            //ComCanvas
            for(int i = 0; i < coms.Length; i++)
            {
                coms[i] = new ComCanvas(60 + 20 * i, 100, global, dev.GetComPortBase(i));
                AddClickPoint(coms[i]);
            }

            //title
            Children.Add(new Label()
            {
                Content = "VTM-C",
                FontSize = 30,
                Foreground = Brushes.SlateBlue,
                Margin = new Thickness(150, 100, 0, 0),
                IsHitTestVisible = false
            });

            timer.Tick += Update;
            timer.Interval = TimeSpan.FromMilliseconds(50);

            RunButton.OnClickEvent += Run;
        }

        void Run()
        {
            if (dev.IsExit)
            {
                dev.Start();
                timer.Start();
                RunButton.Background = Brushes.Red;
                StatusLabel.Content = "Running";
                StatusLabel.Background = Brushes.Green;
            }
            else
            {
                dev.InformStop();
            }
        }

        private void Update(object sender, EventArgs e)
        {
            if (dev.IsExit)
            {
                StatusLabel.Content = "Stop";
                StatusLabel.Background = Brushes.Red;
                RunButton.Background = Brushes.Green;
                timer.Stop();
                return;
            }

            dev.Update();

            for(int i = 0; i < leds.Length; i++)
            {
                leds[i] = dev.GetLedValue(i);
            }
            DrawLed();
        }

        void DrawLed()
        {
            //根据Leds[i]绘制Led
            for(int i = 0; i < leds.Length; i++)
            {
                LedCanvasControls[i].Value = leds[i];
                LedCanvasControls[i].Update();
            }
            
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            foreach (var c in coms)
                c?.Update();
        }

        public override void Remove()
        {
            timer.Stop();
            try
            {
                dev?.Close();
            }catch(Exception e)
            {
                global.TipText(e.ToString());
            }

            foreach(var com in coms)
            {
                com?.Remove();
            }
            base.Remove();
        }
    }
}
