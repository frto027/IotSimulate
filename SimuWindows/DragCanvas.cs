using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SimuWindows
{

    public class DragCanvas:Canvas
    {
        private Canvas parent;

        private bool onDrag = false;
        double m_bx, m_by;//鼠标按下时的margin
        double p_bx, p_by;//鼠标按下时相对parent的位置
         
        public DragCanvas(Canvas parent)
        {
            this.parent = parent;
            parent.Children.Add(this);
            Cursor = Cursors.ScrollAll;
            /*
             * 子类重写构造函数，并设置Height，With，Background
             * */
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (IsMouseDirectlyOver == false)
                return;

            //重置到顶层
            parent.Children.Remove(this);
            parent.Children.Add(this);
            //记录坐标开始拖动
            Point pos = e.GetPosition(parent);
            m_bx = Margin.Left;
            m_by = Margin.Top;
            p_bx = pos.X;
            p_by = pos.Y;
            onDrag = true;
            //注册回掉，当鼠标移出父控件时停止移动
            parent.MouseLeave += StopDrag;
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (onDrag)
            {
                Point pos = e.GetPosition(parent);
                Margin = new Thickness(m_bx + (pos.X - p_bx), m_by + (pos.Y - p_by), 0, 0);
            }
        }
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            StopDrag(null,null);
        }

        private void StopDrag(object sender, MouseEventArgs e)
        {
            onDrag = false;
            parent.MouseLeave -= StopDrag;
        }

        protected void AddClickPoint(ClickPoint point)
        {
            Children.Add(point);
        }

        public void Remove()
        {
            parent.Children.Remove(this);
        }


    }

    public abstract class ClickPoint:Canvas
    {
        public ClickPoint(double x,double y)
        {
            Margin = new Thickness(x, y, 0, 0);
            Cursor = Cursors.Hand;
        }
        private bool clickReady = false;
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (!IsMouseDirectlyOver)
                return;
            clickReady = true;
        }
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            clickReady = false;
        }
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            if (!IsMouseDirectlyOver)
                clickReady = false;
            if (clickReady)
                OnClick();
            clickReady = false;
        }
        public abstract void OnClick();
    }

    public class RemoveClickPoint : ClickPoint
    {
        private DragCanvas aim;
        public RemoveClickPoint(double x,double y,DragCanvas aim) : base(x, y)
        {
            this.aim = aim;
            Width = 20;
            Height = 20;
            Background = new SolidColorBrush(Colors.Red);
        }

        public override void OnClick()
        {
            aim.Remove();
        }
    }
}
