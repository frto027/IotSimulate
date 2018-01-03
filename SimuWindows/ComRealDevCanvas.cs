using ComReal;
using System;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;

namespace SimuWindows
{
    public class ComRealDevCanvas:DragCanvas
    {
        ComRealDev com;
        bool isOpen = false;
        GlobalGUIManager global;
        ComCanvas comCvs;

        ClickEventPoint openButton = new ClickEventPoint(100, 110) {
            Width = 20,Height = 20,Background = Brushes.Green
        };
        ClickEventPoint warrButton = new ClickEventPoint(100, 110)
        {
            Width = 20,
            Height = 20,
            Background = Brushes.Orange
        };
        ClickEventPoint closeButton = new ClickEventPoint(100, 110) {
            Width = 20,Height = 20,Background = Brushes.Red
        };

        public ComRealDevCanvas(ComRealDevSetthings setthings ,GlobalGUIManager global) : base(global.rootcvs)
        {
            this.global = global;

            Width = 140;
            Height = 140;
            Background = Brushes.DarkBlue;
            {
                Canvas titleCvs;
                Children.Add(titleCvs = new Canvas
                {
                    Margin = new Thickness(20, 0, 0, 0),
                    Width = 100,
                    Height = 40,
                    Background = new VisualBrush(new Label
                    {
                        Content = "RealCom",
                        Foreground = Brushes.Black
                    })
                    { Stretch = Stretch.Uniform }
                });
                dragList.Add(titleCvs);
            }

            {
                Canvas infoCvs;
                Children.Add(infoCvs = new Canvas
                {
                    Margin = new Thickness(10, 40, 0, 0),
                    Width = 100,
                    Height = 100,
                    Background = new VisualBrush(new Label()
                    {
                        Content = String.Format("Com info:\nName:{0}\n" +
                   "bundrate:{1}\n" +
                   "databits:{2}\n" +
                   "stopbits:{3}\n" +
                   "parity:{4}", setthings.portName, setthings.baudrate, setthings.databits,
                   setthings.stopBits.ToString(), setthings.parity.ToString()),
                        Foreground = Brushes.OrangeRed
                    })
                    { Stretch = Stretch.Uniform }
                });
                dragList.Add(infoCvs);
            }
            try
            {
                com = new ComRealDev(setthings);
            }catch(Exception e)
            {
                //创建错误，清理退出
                Remove();
                throw e;
            }
            

            AddClickPoint(new RemoveClickPoint(0, 0, this));

            AddClickPoint(comCvs = new ComCanvas(100, 40, global, com));

            AddClickPoint(openButton);
            openButton.OnClickEvent += ComOpen;
            warrButton.OnClickEvent += ComOpen;
            closeButton.OnClickEvent += ComClose;
        }

        public void ComOpen()
        {
            try
            {
                RemoveClickPoint(openButton);
                RemoveClickPoint(warrButton);
                com.OpenPort();
                isOpen = true;
                AddClickPoint(closeButton);
            }catch(ArgumentOutOfRangeException e)
            {
                global.TipText("串口打开失败\n" + e.ToString());
                AddClickPoint(warrButton);
            }
            catch(UnauthorizedAccessException e)
            {
                global.TipText("访问拒绝\n" + e.ToString());
                AddClickPoint(warrButton);
            }
            catch(ArgumentException e)
            {
                global.TipText("不支持的端口文件类型\n" + e.ToString());
                AddClickPoint(warrButton);
            }
            catch(InvalidOperationException e)
            {
                global.TipText("端口已被使用\n" + e.ToString());
                AddClickPoint(warrButton);
            }
            catch(System.IO.IOException e)
            {
                global.TipText("端口无效\n" + e.ToString());
                AddClickPoint(warrButton);
            }
        }

        public void ComClose()
        {
            try
            {
                com.ClosePort();
            }catch(System.IO.IOException e)
            {
                global.TipText("串口已失效\n" + e.ToString());
            }
            isOpen = false;
            RemoveClickPoint(closeButton);
            AddClickPoint(openButton);
            
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            comCvs.Update();
        }

        public override void Remove()
        {
            comCvs?.Remove();
            if (isOpen)
                ComClose();
            base.Remove();
        }

        


    }
}
