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
            Width = 20,Height = 20,Background = IconRes.PowerOffImgBrush
        };
        ClickEventPoint warrButton = new ClickEventPoint(100, 110)
        {
            Width = 20,Height = 20,Background = IconRes.PowerWarrImgBrush
        };
        ClickEventPoint closeButton = new ClickEventPoint(100, 110) {
            Width = 20,Height = 20,Background = IconRes.PowerOnImgBrush
        };

        public ComRealDevCanvas(ComRealDevSetthings setthings ,GlobalGUIManager global) : base(global.rootcvs)
        {
            this.global = global;

            Width = 140;
            Height = 140;

            SetupBackgrountStyle();
            //title canvas
            Children.Add(new Label()
            {
                Margin = new Thickness(20, 0, 0, 0),
                Content = "RealCom",
                Foreground = Brushes.Black,
                IsHitTestVisible = false,
                FontSize = 18
            });
            //info canvas
            Children.Add(new Label()
            {
                Margin = new Thickness(10,30,0,0),
                IsHitTestVisible = false,
                Content = String.Format("Com info:\nName:{0}\n" +
                   "bundrate:{1}\n" +
                   "databits:{2}\n" +
                   "stopbits:{3}\n" +
                   "parity:{4}", setthings.portName, setthings.baudrate, setthings.databits,
                   setthings.stopBits.ToString(), setthings.parity.ToString()),
                Foreground = Brushes.Black
            });

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

        public override void Remove()
        {
            comCvs?.Remove();
            if (isOpen)
                ComClose();
            base.Remove();
        }

        


    }
}
