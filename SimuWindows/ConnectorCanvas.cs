using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public ConnectClickPoint A, B;
        private Canvas rootcvs;
        private Line line = new Line();

        DispatcherTimer timer = new DispatcherTimer();

        public ConnectorCanvas(ConnectClickPoint A,ConnectClickPoint B,Canvas rootcvs)
        {
            this.A = A;
            this.B = B;
            this.rootcvs = rootcvs;
            //画一条线 从A到B
            line.X1 = A.X();
            line.Y1 = A.Y();
            line.X2 = B.X();
            line.Y2 = B.Y();
            line.Stroke = Brushes.LightSteelBlue;
            line.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            line.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            rootcvs.Children.Add(line);

            timer.Tick += Update;
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Start();

            line.MouseDown += MouseDown;
            line.Cursor = Cursors.No;
        }

        private void Update(object sender, EventArgs e)
        {
            line.X1 = A.X();
            line.Y1 = A.Y();
            line.X2 = B.X();
            line.Y2 = B.Y();
            
        }

        public void MouseDown(object sender, MouseButtonEventArgs e)
        {
            line.MouseDown -= MouseDown;
            Remove();
        }

        public virtual void Remove()
        {
            timer.Stop();
            timer.Tick -= Update;

            rootcvs.Children.Remove(line);
            A.DisConnect();
            B.DisConnect();
            A = null;
            B = null;
        }

    }
}
