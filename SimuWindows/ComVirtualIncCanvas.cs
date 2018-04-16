using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using IoTSimulate;
using ComVirtualIncreacement;
using System.Windows.Input;
using System.Windows;
namespace SimuWindows
{
    public class ComVirtualIncCanvas : DragCanvas
    {
        ComVirInc com = new ComVirInc();
        ComCanvas comCanvas;
        ClickEventPoint Button;
        public ComVirtualIncCanvas(GlobalGUIManager global) : base(global.rootcvs)
        {
            //初始化一个TitleCvs用于显示标题
            Canvas titleCvs = new Canvas
            {
                Margin = new Thickness(30, 0, 0, 0),
                Height = 25,
                Width = 80,
                Background = new VisualBrush
                {

                    Stretch = Stretch.Uniform,
                    Visual = new Label
                    {
                        Content = "VirIncCom"
                    }
                }
            };
            Children.Add(titleCvs);
            dragList.Add(titleCvs);


            Height = 80;
            Width = 140;
            Background = Brushes.Gray;
            AddClickPoint(new RemoveClickPoint(0, 0, this));

            AddClickPoint(comCanvas = new ComCanvas(100, 35, global, com));

            AddClickPoint(Button = new ClickEventPoint(10, 20) {
                Height = 40,
                Width = 80,
                
                Background = new VisualBrush()
                {
                    Visual = new Label
                    {
                        Content = "Send",
                        Foreground = Brushes.Green
                    }
                },
                
            });
            Button.OnClickEvent += com.SendData;
        }


        public override void Remove()
        {
            Button.OnClickEvent -= com.SendData;
            com.Close();
            comCanvas.Remove();
            base.Remove();
        }
    }

    
}
