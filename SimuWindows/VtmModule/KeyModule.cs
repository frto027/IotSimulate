using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IoTSimulate;
using System.Windows.Media;
using System.Windows.Threading;

namespace SimuWindows.VtmModule
{
    [ModulePosition(Position.A,Position.B,Position.C,Position.D)]
    class KeyModule : VtmModuleBase
    {
        Brush[] KeyColors = new Brush[] {
            Brushes.Red,Brushes.GreenYellow,Brushes.LightYellow,Brushes.AliceBlue
        };
        Brush PressedColor = Brushes.Black;

        ClickEventPoint[] keys = new ClickEventPoint[] {
            new ClickEventPoint(10,10){ Width = 30,Height = 30 },
            new ClickEventPoint(60,10){ Width = 30,Height = 30 },
            new ClickEventPoint(10,60){ Width = 30,Height = 30 },
            new ClickEventPoint(60,60){ Width = 30,Height = 30 },
        };
        DateTime [] EndTimes = new DateTime[VtmDev.TrigKeyCount];

        private const double ShowDelayMs = 200;

        DispatcherTimer UITimer = new DispatcherTimer();

        public KeyModule(VtmDev dev, GlobalGUIManager global) : base(dev, global)
        {
            Background = Brushes.Gray;
            //key to UI
            for(int i = 0; i < keys.Length; i++)
            {
                keys[i].Background = KeyColors[i];
                EndTimes[i] = DateTime.Now;
                Children.Add(keys[i]);
            }
            //key click event
            for(int i = 0; i < VtmDev.TrigKeyCount; i++)
            {
                int x = i;//闭包
                keys[i].OnHalfClickEvent += () => {
                    VtmDev.TrigKeyPress(x);
                    keys[x].Background = PressedColor;
                    EndTimes[x] = DateTime.Now.AddMilliseconds(ShowDelayMs);
                    //Active Timer
                    UITimer.Start();
                };
            }
            VtmDev.TrigKeyActive = true;

            UITimer.Interval = TimeSpan.FromMilliseconds(ShowDelayMs / 3);
            UITimer.Tick += UpdateUI;
        }

        private void UpdateUI(object sender, EventArgs e)
        {
            bool remain = false;
            var now = DateTime.Now;
            for (int i = 0; i < keys.Length; i++)
            {
                if (now > EndTimes[i])
                {
                    //恢复
                    keys[i].Background = KeyColors[i];
                }
                else
                {
                    remain = true;
                }
            }

            //No more Button
            if(remain == false)
            {
                UITimer.Stop();
            }
        }


        public override void Remove()
        {
            VtmDev.TrigKeyActive = false;
            base.Remove();
        }

    }
}
