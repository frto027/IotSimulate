using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
namespace SimuWindows
{
    public class GlobalGUIManager
    {
        public ConnectClickPoint DragOther = null;//如果为null则无拖拽，非null则有拖拽
        public Canvas rootcvs;//根Canvas

        public Action<double, double> BeginDragDraw;//开始实时画线,(x,y)起始坐标
        public Action EndDragDraw;//终止画线

        public Action<String> TipText;

        public Canvas maskcvs;//浮动在最顶层的Canvas，一般用来显示范围什么的
    }
}
