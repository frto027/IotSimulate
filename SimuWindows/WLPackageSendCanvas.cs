using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using IoTSimulate;
namespace SimuWindows
{
    class WLPackageSendCanvas:DragCanvas
    {
        private const double WindowDistance = 20;
        private NumberOnlyTextBox[] textBoxes = new NumberOnlyTextBox[WLPackageDev.PACKAGE_SIZE];

        WLPackageDev dev = new WLPackageDev();

        WirelessSignal signal;

        DispatcherTimer timer = new DispatcherTimer();
        public WLPackageSendCanvas(GlobalGUIManager global) : base(global.rootcvs)
        {
            Width = 20 + textBoxes.Length * WindowDistance + 20;
            Height = 60;
            Background = Brushes.DarkGreen;
            //title
            Children.Add(new Label() {IsHitTestVisible = false, Content = "Wireless Package Sender", Margin = new Thickness(30, 0, 0, 0) });
            //remove
            AddClickPoint(new RemoveClickPoint(0, 0, this));

            for(int i = 0; i < textBoxes.Length; i++)
            {
                textBoxes[i] = new NumberOnlyTextBox()
                {
                    Margin = new Thickness(20 + WindowDistance * i, 20, 0, 0),
                    Width = WindowDistance,
                };
                Children.Add(textBoxes[i]);
            }

            ClickEventPoint clickEventPoint = new ClickEventPoint(20, 40)
            {
                Width = 10,Height = 10,Background = Brushes.LightGreen
            };
            clickEventPoint.OnClickEvent += Submit;
            AddClickPoint(clickEventPoint);

            signal = new WirelessSignal(global.rootcvs, dev, 20, 20);
            Children.Add(signal);

            timer.Tick += Update;
            timer.Interval = TimeSpan.FromMilliseconds(200);
            timer.Start();
        }

        private void Update(object sender, EventArgs e)
        {
            dev.Update();
        }

        private void Submit()
        {
            textBoxes[0].Value = 0x55;
            textBoxes[1].Value = 0xaa;
            byte chk = 0;
            for(int i=0;i<textBoxes.Length - 1; i++)
            {
                chk += textBoxes[i].Value;
            }
            textBoxes[textBoxes.Length - 1].Value = chk;

            byte[] pkg =
                (from box in textBoxes select box.Value).ToArray();
            dev.SendPackage(pkg, 0, true);
        }

        public override void Remove()
        {
            timer.Stop();
            signal.Remove();
            dev.Close();
            base.Remove();
        }

        private class NumberOnlyTextBox : TextBox
        {
            private byte value;
            public byte Value { get { LimitText(); return value; } set {
                    Text = value + "";
                    LimitText(true);
                } }
            public NumberOnlyTextBox()
            {
                Text = "0";
                value = 0;
            }
            protected override void OnTextChanged(TextChangedEventArgs e)
            {
                base.OnTextChanged(e);
                //LimitText();
            }
            public void LimitText(bool changezero = false)
            {
                int v = value;
                try
                {
                    if (Text.Length == 0)
                    {
                        if(changezero == false)
                        {
                            return;
                        }
                        v = 0;
                    }

                    else
                    {
                        if (Text.StartsWith("0x"))
                        {
                            v = int.Parse(Text.Substring(2), System.Globalization.NumberStyles.HexNumber);
                        }
                        else
                        {
                            v = int.Parse(Text);
                        }

                    }
                }
                catch (Exception) { }

                if (v < 0)
                {
                    v += 0x100;
                }
                v %= 0x100;
                value = (byte)v;
                Text = value.ToString();
            }

            protected override void OnLostFocus(RoutedEventArgs e)
            {
                base.OnLostFocus(e);
                LimitText(true);
            }

            protected override void OnGotFocus(RoutedEventArgs e)
            {
                base.OnGotFocus(e);
                SelectAll();
            }

            protected override void OnAccessKey(AccessKeyEventArgs e)
            {
                base.OnAccessKey(e);
                
            }
        }
    }
}
