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
        public double SignalDistance = 200;

        public WLComHost WLComHost = new WLComHost();

        private ComCanvas ComCanvas;

        Canvas RangeCanvas;
        EllipseGeometry RangeGeometry;

        DispatcherTimer timer = new DispatcherTimer();

        Point CenterPoint = new Point(60, 30);

        Canvas rootcvs,maskcvs;
        public WirelessComHostCanvas(GlobalGUIManager global) : base(global.rootcvs)
        {
            Width = 250;
            Height = 60;

            Background = Brushes.OrangeRed;

            rootcvs = global.rootcvs;
            maskcvs = global.maskcvs;
            //Title
            Children.Add(new Label()
            {
                IsHitTestVisible = false,
                Margin = new Thickness(30, 5, 0, 0),
                Content = "wireless(host) <-> com"
            });
            //Remove
            AddClickPoint(new RemoveClickPoint(0, 0, this));
            //Combase
            ComCanvas = new ComCanvas(30, 30, global, WLComHost.comBase);
            AddClickPoint(ComCanvas);

            //Range
            maskcvs.Children.Add(RangeCanvas = new Canvas()
            {
                Margin = new Thickness(0,0,0,0),
                Background = new DrawingBrush()
                {
                    Drawing = new GeometryDrawing()
                    {
                        Pen = new Pen(Brushes.Red,0.2),
                        Geometry = RangeGeometry = new EllipseGeometry(),
                        Brush = new SolidColorBrush(Color.FromArgb(60, 100, 100, 10))
                    },
                },
                IsHitTestVisible = false
            });
            UpdateRange();

            timer.Tick += Update;
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Start();
        }

        private void Update(object sender, EventArgs e)
        {
            WLComHost.Update();
        }
        private void UpdateRange()
        {
            Point p = TranslatePoint(CenterPoint, maskcvs);
            RangeCanvas.Margin = new Thickness(-SignalDistance + p.X, -SignalDistance + p.Y, 0, 0);
            RangeCanvas.Width = RangeCanvas.Height = 2 * SignalDistance;
            RangeGeometry.Center = new Point(SignalDistance, SignalDistance);
            RangeGeometry.RadiusX = RangeGeometry.RadiusY = SignalDistance;
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            ComCanvas.Update();
            UpdateRange();
        }

        public override void Remove()
        {
            maskcvs.Children.Remove(RangeCanvas);
            timer.Stop();
            ComCanvas.Remove();
            WLComHost.Close();
            base.Remove();

        }

        public double DistanceSquareOf(double x,double y)
        {
            Point p = TranslatePoint(CenterPoint, rootcvs);
            double L = p.X, T = p.Y;
            L -= x;
            T -= y;
            return L * L + T * T;
        }
    }
}
