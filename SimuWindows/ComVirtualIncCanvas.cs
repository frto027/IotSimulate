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
            Children.Add(new Label() {
                IsHitTestVisible = false,
                Content = "VirIncCom",
                FontSize = 20,
                Margin = new Thickness(25,0,0,0)
            });

            Height = 80;
            Width = 140;

            SetupBackgrountStyle();

            AddClickPoint(new RemoveClickPoint(0, 0, this));

            AddClickPoint(comCanvas = new ComCanvas(100, 35, global, com));

            AddClickPoint(Button = new ClickEventPoint(10, 30) {
                Height = 40,
                Width = 60,
                
            });

            Button.Children.Add(new Label()
            {
                Margin = new Thickness(0, 0, 0, 0),
                IsHitTestVisible = false,
                FontSize = 20,
                Content = "Send",
                Foreground = Brushes.Green,
            });

            Button.SetupBackgrountStyle();

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
