using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using IoTSimulate;


namespace SimuWindows
{
    /// <summary>
    /// 用于无线设备
    /// </summary>
    class WirelessSignal:Canvas
    {
        WLDev dev;
        Canvas rootcvs;
        public double SignalDistance = 1000;

        private Line line = new Line() { IsHitTestVisible = false,Stroke = Brushes.DarkOrange };

        private Canvas aimHost;

        public WirelessSignal(Canvas rootcvs,WLDev dev,double x, double y)
        {
            this.IsHitTestVisible = false;
            this.rootcvs = rootcvs;
            this.dev = dev;
            Margin = new Thickness(x, y, 0, 0);
            rootcvs.Children.Add(line);
        }

        public void Update()
        {
            WLHostDev host = null;
            Canvas aimhost = null;
            double mindistance = double.PositiveInfinity;
            foreach(var x in rootcvs.Children)
            {
                switch (x)
                {
                    case WirelessComHostCanvas wlch:
                        Point p = TranslatePoint(new Point(0, 0), rootcvs);
                        double dist = wlch.DistanceSquareOf(p.X, p.Y);
                        if (dist < mindistance && dist <= wlch.SignalDistance * wlch.SignalDistance && dist <= SignalDistance * SignalDistance)
                        {
                            mindistance = dist;
                            host = wlch.WLComHost;
                            aimhost = wlch;
                        }
                        break;
                }
            }
            dev.SetHost(host);
            aimHost = aimhost;
           
        }

        public void UpdateMove()
        {
            if (aimHost == null)
            {
                line.Visibility = Visibility.Collapsed;
            }
            else
            {
                Point p = TranslatePoint(new Point(0, 0), rootcvs);
                Point p2 = aimHost.TranslatePoint(new Point(0, 0), rootcvs);
                line.Visibility = Visibility.Visible;
                line.X1 = p.X;
                line.Y1 = p.Y;
                line.X2 = p2.X;
                line.Y2 = p2.Y;
            }
        }

        public void Remove()
        {
            rootcvs.Children.Remove(line);
        }
    }
}
