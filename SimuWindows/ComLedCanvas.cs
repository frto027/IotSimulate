using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using ComLED;

namespace SimuWindows
{

    class ComLedCanvas : DragCanvas
    {
        DispatcherTimer timer = new DispatcherTimer();
        public ComLED.ComLED led = new ComLED.ComLED();

        public ComLedCanvas(Canvas canvas) : base(canvas)
        {
            Width = 100;
            Height = 100;
            Background = new SolidColorBrush(Colors.Green);

            AddClickPoint(new RemoveClickPoint(0, 0, this));

            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += Update;

            timer.Start();

            //添加一个Com口连接点，还没写
        }

        public void Update(object sender, EventArgs e)
        {
            //显示LED A LED B到自身
        }

        public new void Remove()
        {
            base.Remove();
            timer.Tick -= Update;
            timer.Stop();
        }

    }
    
}
