using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using IoTSimulate;

namespace SimuWindows.VtmModule
{
    [ModulePosition(Position.C,Position.D)]
    class LedModule:VtmModuleBase
    {
        readonly byte[] leds = new byte[VtmDev.LED_COUNT];
        LedCanvasControl[] LedCanvasControls = new LedCanvasControl[VtmDev.LED_COUNT];

        public LedModule(VtmDev dev) : base(dev)
        {
            //LED CANVAS
            for (int i = 0; i < leds.Length; i++)
            {
                LedCanvasControls[i] = new LedCanvasControl()
                {
                    Width = 40,
                    Height = 80,
                    Margin = new Thickness(40 * i, 0, 0, 0),
                    IsHitTestVisible = false
                };
                Children.Add(LedCanvasControls[i]);
            }
            Background = Brushes.White;
            
        }

        public override void FitPosition(Position position)
        {
            base.FitPosition(position);
            Width = 340;
            Height = 80;
        }

        public override void Update()
        {
            base.Update();
            for (int i = 0; i < leds.Length; i++)
            {
                leds[i] = VtmDev.GetLedValue(i);
            }
            DrawLed();
        }

        void DrawLed()
        {
            //根据Leds[i]绘制Led
            for (int i = 0; i < leds.Length; i++)
            {
                LedCanvasControls[i].Value = leds[i];
                LedCanvasControls[i].Update();
            }

        }
    }
}
