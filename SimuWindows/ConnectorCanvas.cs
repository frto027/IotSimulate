using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SimuWindows
{
    /// <summary>
    /// 所有连线Canvas继承此类
    /// </summary>
    public class ConnectorCanvas
    {
        private const double LineMargin = 5;

        public ConnectClickPoint A, B;
        private Canvas rootcvs;

        private Path bpath;
        
        private PathFigure bfigureA,bfigureB,bfigureG;
        //private BezierSegment bezier = new BezierSegment();
        private PolyBezierSegment polyBezierA = new PolyBezierSegment(),
            polyBezierB = new PolyBezierSegment(),
            polyBezierG = new PolyBezierSegment();

        DispatcherTimer timer = new DispatcherTimer();



        public ConnectorCanvas(ConnectClickPoint A,ConnectClickPoint B,Canvas rootcvs)
        {
            this.A = A;
            this.B = B;
            this.rootcvs = rootcvs;
            //画一条线 从A到B
            /*
            line.X1 = A.X();
            line.Y1 = A.Y();
            line.X2 = B.X();
            line.Y2 = B.Y();
            line.Stroke = Brushes.LightSteelBlue;
            line.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            line.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            rootcvs.Children.Add(line);
            */

            bpath = new Path()
            {
                Stroke = Brushes.Red,
                StrokeThickness = 1,
                Data = new PathGeometry()
                {
                    Figures = new PathFigureCollection(new PathFigure[] {
                        bfigureA = new PathFigure()
                        {
                            StartPoint = new Point(A.X(),A.Y()),
                            Segments = new PathSegmentCollection(new PathSegment[]{
                                polyBezierA
                            })
                        },
                        bfigureB = new PathFigure()
                        {
                            StartPoint = new Point(A.X(),A.Y()),
                            Segments = new PathSegmentCollection(new PathSegment[]{
                                polyBezierB
                            })
                        },
                        bfigureG = new PathFigure()
                        {
                            StartPoint = new Point(A.X(),A.Y()),
                            Segments = new PathSegmentCollection(new PathSegment[]{
                                polyBezierG
                            })
                        }
                    })
                }
            };
            
            SetupBezierLink();

            rootcvs.Children.Add(bpath);

            timer.Tick += Update;
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Start();
            
            bpath.MouseDown += MouseDown;
            bpath.Cursor = Cursors.No;
           
        }

        private void SetupBezierLink()
        {
            bfigureG.StartPoint = new Point(A.X() - LineMargin, A.Y());
            bfigureA.StartPoint = new Point(A.X() , A.Y());
            bfigureB.StartPoint = new Point(A.X() + LineMargin, A.Y());

            polyBezierG.Points = UpdatePoints(-LineMargin, -LineMargin);
            polyBezierA.Points = UpdatePoints(0, LineMargin);
            polyBezierB.Points = UpdatePoints(LineMargin, 0);
        }

        private PointCollection UpdatePoints(double begoff = 0,double endoff = 0)
        {
            return new PointCollection(new Point[] {
                new Point(A.X() + begoff, A.Y() - 30),
                new Point(B.X() + endoff, B.Y() - 30),
                new Point(B.X() + endoff, B.Y())
            });
        }
        public void Update(object sender, EventArgs e)
        {
            SetupBezierLink();
        }

        public void MouseDown(object sender, MouseButtonEventArgs e)
        {

            bpath.MouseDown -= MouseDown;
            Remove();
        }

        public virtual void Remove()
        {
            timer.Stop();
            timer.Tick -= Update;
            
            rootcvs.Children.Remove(bpath);
            A.DisConnect();
            B.DisConnect();
            A = null;
            B = null;
        }

    }
}
