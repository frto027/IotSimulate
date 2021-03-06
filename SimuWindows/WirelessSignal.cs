﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using IoTSimulate;


namespace SimuWindows
{
    /// <summary>
    /// 用于无线设备
    /// 给WLDev提供到Host的绑定支持
    /// 需要Children.Add到控件自身
    /// </summary>
    class WirelessSignal:Canvas
    {
        WLDev dev;
        Canvas rootcvs;
        public double SignalDistance = 1000;
        private const double SignalLineLength = 40;//信号线的长度
        private Line line = new Line() { IsHitTestVisible = false, Stroke = Brushes.DarkOrange,
            StrokeDashArray = new DoubleCollection(new double[] { 5,4,4,3,3,2,2,1,1,0.5,1,1,2,2,3,3 }),
            StrokeThickness = 2
        };

        private Canvas aimHost;

        DispatcherTimer timer = new DispatcherTimer();

        private const double VisualRadius = 5;

        public WirelessSignal(Canvas rootcvs,WLDev dev,double x, double y)
        {
            this.IsHitTestVisible = false;
            this.rootcvs = rootcvs;
            this.dev = dev;
            Margin = new Thickness(x, y, 0, 0);
            rootcvs.Children.Add(line);
            DragCanvas.MouseMoveAction += UpdateMove;

            Children.Add(new Canvas()
            {
                Margin = new Thickness(-VisualRadius, -VisualRadius, 0, 0),
                Width = 2 * VisualRadius,
                Height = 2 * VisualRadius,
                Background = new DrawingBrush()
                {
                    Drawing = new GeometryDrawing()
                    {
                        Brush = Brushes.Brown,
                        Geometry = new EllipseGeometry(new Point(VisualRadius, VisualRadius), VisualRadius, VisualRadius)
                    }
                }
            });

            timer.Tick += UpdateTick;
            timer.Interval = TimeSpan.FromMilliseconds(200);
            timer.Start();
        }

        private void UpdateTick(object sender, EventArgs e)
        {
            Update();
        }

        private void Update()
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

        private void UpdateMove()
        {
            if (aimHost == null)
            {
                line.Visibility = Visibility.Collapsed;
            }
            else
            {
                Point p = TranslatePoint(new Point(0, 0), rootcvs);
                Point p2 = aimHost.TranslatePoint(new Point(0, 0), rootcvs);

                var offv = (p2 - p);
                offv.Normalize();
                offv *= SignalLineLength;
                p2 = p + offv;

                line.Visibility = Visibility.Visible;
                line.X1 = p.X;
                line.Y1 = p.Y;
                line.X2 = p2.X;
                line.Y2 = p2.Y;
            }
        }

        public void Remove()
        {
            timer.Stop();
            
            DragCanvas.MouseMoveAction -= UpdateMove;
            rootcvs.Children.Remove(line);
        }
    }
}
