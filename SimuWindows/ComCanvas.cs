using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using IoTSimulate;

namespace SimuWindows
{
    /// <summary>
    /// 串口端点的实现类，继承ConnectClickPoint
    /// 在EndDrag方法中新建一个对应的ConnectorCanvas与另一个ComCanvas连接
    /// 在DisConnect中移除对连接线的引用
    /// 建议编写Remove方法，移除所有的连接线
    /// </summary>
    public class ComCanvas : ConnectClickPoint
    {
        public ComConnectorCanvas ConnectedConnector = null;

        public ComBase ComBase { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="x">x偏移</param>
        /// <param name="y">y偏移</param>
        /// <param name="globalGUIManager">全局对象</param>
        /// <param name="comBase">绑定的串口</param>
        public ComCanvas(double x, double y, GlobalGUIManager globalGUIManager,ComBase comBase):base(x, y, globalGUIManager)
        {
            Width = 10;
            Height = 10;
            Background = Brushes.DarkGray;
            this.ComBase = comBase;
        }
        public override bool StartDrag()
        {
            return ConnectedConnector == null;
        }
        public override void EndDrag(ConnectClickPoint other)
        {
            
            
                if (other is ComCanvas o)
                {
                    if (ConnectedConnector == null && o.ConnectedConnector == null)
                        ConnectedConnector = o.ConnectedConnector = new ComConnectorCanvas(this, o, globalGUIManager.rootcvs);
                }
                else
                {
                    globalGUIManager.TipText("端口类型不匹配");
                }
            
        }


        System.Windows.Point anchor = new System.Windows.Point(5, 5);

        public override double X()
        {
            return TranslatePoint(anchor, globalGUIManager.rootcvs).X;
        }
        public override double Y()
        {
            return TranslatePoint(anchor, globalGUIManager.rootcvs).Y;
        }
        /// <summary>
        /// 只能被Connector调用！否则会无限递归
        /// </summary>
        public override void DisConnect()
        {
            ConnectedConnector = null;
        }

        public void Remove()
        {
            ConnectedConnector?.Remove();
        }

        //重绘连接线条，建议在OnMouseMove中调用
        public void Update()
        {
            ConnectedConnector?.Update(null,null);
        }
    }

    public class ComConnectorCanvas : ConnectorCanvas
    {
        ComConnector connector = new ComConnector();

        public ComConnectorCanvas(ComCanvas A, ComCanvas B, Canvas rootcvs):base(A, B, rootcvs)
        {
            //调用驱动
            connector.Bind(A.ComBase, B.ComBase);
        }

        public override void Remove()
        {
            connector.Close();
            base.Remove();
        }
    }
}
