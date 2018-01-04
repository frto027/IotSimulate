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
    /// ComDevSetthingTipBox.xaml 的交互逻辑
    /// </summary>
    public partial class ComDevSetthingTipBox : Window
    {
        private bool ExitByNormal = false;
        private class PrivateContext
        {
            public string PortName { get; set; }
            public int Baudrate { get; set; }
            public int Databits { get; set; }
            public System.IO.Ports.Parity Parity { get; set; }
            public System.IO.Ports.StopBits StopBits { get; set; }
            public override string ToString()
            {
                return String.Format("Portname={0},Baudrate={1},Databits={2},Parity={3},StopBits={4}",
                    PortName, Baudrate, Databits, Parity.ToString(), StopBits.ToString());
            }
        }

        private static PrivateContext PergamentContext = new PrivateContext()
        {
            PortName = "COM1",
            Baudrate = 9600,
            Databits = 8,
            Parity = System.IO.Ports.Parity.None,
            StopBits = System.IO.Ports.StopBits.One
        };

        public ComDevSetthingTipBox()
        {
            InitializeComponent();
            globalGrid.DataContext = PergamentContext;
            
        }

        public bool Query(out ComReal.ComRealDevSetthings setthings)
        {
            ExitByNormal = false;
            ComPortNameList.Items.Clear();

            PrivateContext privateContext = globalGrid.DataContext as PrivateContext;

            try
            {
                var pns = System.IO.Ports.SerialPort.GetPortNames();
                foreach (String s in pns)
                    ComPortNameList.Items.Add(s);
                if(privateContext != null)
                    privateContext.PortName = pns.FirstOrDefault() ?? privateContext.PortName;
            }
            catch (System.ComponentModel.Win32Exception) { }
            ShowDialog();
            
            setthings = new ComReal.ComRealDevSetthings
            {
                portName = privateContext?.PortName,
                baudrate = privateContext?.Baudrate ?? 9600,
                databits = privateContext?.Databits ?? 8,
                parity = privateContext?.Parity ?? System.IO.Ports.Parity.Even,
                stopBits = privateContext?.StopBits ?? System.IO.Ports.StopBits.One
            };
            
            
            return ExitByNormal;
        }

        protected override void OnClosed(EventArgs e)
        {
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ExitByNormal = true;
            Close();
        }
    }
}
