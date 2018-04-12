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
    /// <summary>
    /// 拖拽组件的根节点，所有能够拖拽的组件继承这个类
    /// </summary>
    public class DragCanvas:Canvas
    {
        public static event Action MouseMoveAction;//千万记得Remove的时候这里要移除事件，否则会内存泄漏

        private Canvas parent;

        private bool onDrag = false;
        double m_bx, m_by;//鼠标按下时的margin
        double p_bx, p_by;//鼠标按下时相对parent的位置
        /// <summary>
        /// 鼠标按下，当鼠标直接位于dragList中元素上时，响应拖拽
        /// 当鼠标位于dragHardList中元素上时，无论如何响应拖拽
        /// </summary>
        public readonly List<UIElement> dragList = new List<UIElement>(),dragHardList = new List<UIElement>();

        MainWindow mainWindow = null;

        public DragCanvas(Canvas parent)
        {
            this.parent = parent;
            parent.Children.Add(this);
            Cursor = Cursors.ScrollAll;

            dragList.Add(this);

            {//从parent向上爬到MainWindow
                DependencyObject elem = parent;
                while(elem != null && elem is MainWindow == false)
                {
                    elem = VisualTreeHelper.GetParent(elem);
                }
                mainWindow = elem as MainWindow;
            }
            /*
             * 子类重写构造函数，并设置Height，With，Background
             * */
        }

        protected sealed override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            //当鼠标直接位于dragList中元素上时才会响应
            if (dragList.Find(c => c.IsMouseDirectlyOver) == null && dragHardList.Find(c => c.IsMouseOver) == null)
                return;

            //重置到顶层
            parent.Children.Remove(this);
            parent.Children.Add(this);
            //分类
            //这里希望能够调用MainWindow.KeepSort()
            mainWindow?.KeepSort(null,null);

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
            //鼠标移动时触发全局事件更新画布
            MouseMoveAction?.Invoke();

        }
        protected sealed override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
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

        protected void RemoveClickPoint(ClickPoint point)
        {
            Children.Remove(point);
        }

        public virtual void Remove()
        {
            parent.Children.Remove(this);
        }


    }
    /// <summary>
    /// 所有按钮的根节点，所有按钮继承这个类并实现OnClick
    /// </summary>
    public abstract class ClickPoint:Canvas
    {
        public ClickPoint(double x,double y)
        {
            Margin = new Thickness(x, y, 0, 0);
            Cursor = Cursors.Hand;
        }
        private bool clickReady = false;
        protected sealed override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (!IsMouseDirectlyOver)
                return;
            clickReady = true;
        }
        protected sealed override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            clickReady = false;
        }
        protected sealed override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
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
    /// <summary>
    /// 事件驱动的ClickPoint
    /// </summary>
    public class ClickEventPoint : ClickPoint
    {
        public event Action OnClickEvent;
        public ClickEventPoint(double x,double y) : base(x, y)
        {

        }
        public sealed override void OnClick()
        {
            OnClickEvent();
        }
    }

    /// <summary>
    /// ClickPoint一个实现类，用于连线另一个ConnectClickPoint
    /// </summary>
    public abstract class ConnectClickPoint : ClickPoint
    {
        protected GlobalGUIManager globalGUIManager;

        public abstract double X();
        public abstract double Y();//坐标

        public ConnectClickPoint(double x,double y,GlobalGUIManager globalGUIManager) : base(x, y)
        {
            this.globalGUIManager = globalGUIManager;
        }

        public override void OnClick()
        {
            if(globalGUIManager.DragOther == null)
            {
                if (StartDrag())
                {
                    globalGUIManager.DragOther = this;
                    //开始拖拽
                    globalGUIManager.BeginDragDraw(X(),Y());
                }
            }
            else
            {
                if(globalGUIManager.DragOther != this)
                    globalGUIManager.DragOther.EndDrag(this);

                globalGUIManager.EndDragDraw();
                globalGUIManager.DragOther = null;
            }
            
        }
        /// <summary>
        /// 返回True开始连线，返回False停止连线，xy是画线起点坐标
        /// </summary>
        /// <returns></returns>
        public virtual bool StartDrag() { return true; }
        /// <summary>
        /// 连线触发事件
        /// </summary>
        /// <param name="other">对方的连线节点</param>
        public abstract void EndDrag(ConnectClickPoint other);
        public abstract void DisConnect();

    }

    /// <summary>
    /// ClickPoint的一个实现类，点击后调用aim.Remove
    /// </summary>
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
