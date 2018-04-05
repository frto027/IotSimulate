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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SimuWindows
{
    /// <summary>
    /// LedCanvas.xaml 的交互逻辑
    /// </summary>
    public partial class LedCanvasControl : UserControl
    {
        public byte Value = 0;

        public LedCanvasControl()
        {
            InitializeComponent();
        }

        public void Update()
        {
            for(int i = 0; i < 8; i++)
            {
                imgcontainers.Children[i + 1].Visibility =
                    ((Value & (0x01 << i)) != 0) ?
                    Visibility.Visible : Visibility.Hidden;
            }
        }
    }
}
