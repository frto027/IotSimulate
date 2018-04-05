using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SimuWindows
{
    /// <summary>
    /// LoadingForm.xaml 的交互逻辑
    /// </summary>
    public partial class LoadingForm : Window
    {
        private Func<bool> isOk;
        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
        public LoadingForm(Func<bool> isOk)
        {
            InitializeComponent();
            this.isOk = isOk;
            timer.Interval = TimeSpan.FromMilliseconds(200);
            timer.Tick += Tick;
            timer.Start();
        }

        private void Tick(object sender, EventArgs e)
        {
            if (isOk())
            {
                timer.Stop();
                Close();
            }
        }
    }
}
