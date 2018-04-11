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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SimuWindows
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        GlobalGUIManager GlobalGUIManager = new GlobalGUIManager();

        //这个Timer用来维持顺序
        DispatcherTimer SortKeeper = new DispatcherTimer();
        //排列顺序，越小越靠上
        Tuple<Type, byte>[] SortOrder = new Tuple<Type, byte>[]
        {
            new Tuple<Type, byte>(typeof(Line),1),
            new Tuple<Type, byte>(typeof(DragCanvas),2),
            //默认值
            new Tuple<Type, byte>(typeof(Object),255),
        };
        
        public MainWindow()
        {
            InitializeComponent();
            GlobalGUIManager.rootcvs = cvs;

            GlobalGUIManager.BeginDragDraw += BeginDragDraw;
            GlobalGUIManager.EndDragDraw += EndDragDraw;
            GlobalGUIManager.TipText += s => TipInBase(s);

            DrawCusorLine.Stroke = Brushes.LawnGreen;
            DrawCusorLine.HorizontalAlignment = HorizontalAlignment.Left;
            DrawCusorLine.VerticalAlignment = VerticalAlignment.Top;

            SortKeeper.Tick += KeepSort;
            SortKeeper.Interval = TimeSpan.FromMilliseconds(100);
            SortKeeper.Start();
        }
        private enum CREATE_STATUS { OK, DEVELOPING, ERROR, CANCEL }
        private void MenuItem_AddDev_Click(object sender, RoutedEventArgs e)
        {
            if(sender is MenuItem m)
            {
                switch (m.Tag)
                {
                    case "vcom":
                        new ComVirtualIncCanvas(GlobalGUIManager);
                        goto case CREATE_STATUS.OK;
                    case "vled":
                        new ComLedCanvas(GlobalGUIManager);
                        goto case CREATE_STATUS.OK;
                    case "realcom":
                        ComReal.ComRealDevSetthings comRealSetthing;
                        if(new ComDevSetthingTipBox().Query(out comRealSetthing))
                        {
                            try
                            {
                                new ComRealDevCanvas(comRealSetthing, GlobalGUIManager);
                            }
                            catch (Exception err)
                            {
                                TipInBase("创建错误，" + m.Header + "未被创建\n" + err.ToString());
                                goto case CREATE_STATUS.ERROR;
                            }
                            
                            goto case CREATE_STATUS.OK;
                        }
                        else
                        {
                            goto case CREATE_STATUS.CANCEL;
                        }
                    case "roomenv":
                        new EnviromentCanvas(GlobalGUIManager);
                        goto case CREATE_STATUS.OK;
                    case "tempsensorcom":
                        new ComTempSensorCanvas(GlobalGUIManager);
                        goto case CREATE_STATUS.OK;
                    case "vtm":
                        try
                        {
                            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog
                            {
                                Title = "选择需要烧入的二进制文件"
                            };
                            dialog.ShowDialog();
                            if(dialog.FileName =="")
                            {
                                goto case CREATE_STATUS.CANCEL;
                            }
                            new VtmDevCanvas(GlobalGUIManager, dialog.FileName);
                        }catch(Exception err)
                        {
                            TipInBase(err.ToString());
                            goto case CREATE_STATUS.ERROR;
                        }
                        goto case CREATE_STATUS.OK;
                    case "wlcom":
                        new WirelessComDevCanvas(GlobalGUIManager);
                        goto case CREATE_STATUS.OK;
                    case "wlhostcom":
                        new WirelessComHostCanvas(GlobalGUIManager);
                        goto case CREATE_STATUS.OK;
                    case CREATE_STATUS.OK://已创建
                        TipInBase("已创建 " + m.Header);
                        break;
                    case CREATE_STATUS.DEVELOPING://正在开发
                        TipInBase("该组件正在开发中！");
                        break;
                    case CREATE_STATUS.ERROR:
                        //这里不覆盖之前输出的错误提示
                        //TipInBase("创建错误");
                        break;
                    case CREATE_STATUS.CANCEL:
                        TipInBase("创建取消\n创建已被用户取消");
                        break;
                    default:
                        TipInBase("错误的Tag:"+m.Tag);
                        break;
                }
            }
            else
            {
                TipInBase("错误的sender类型");
            }


            
        }

        //拖动连线相关事件
        private Line DrawCusorLine = new Line();

        private void BeginDragDraw(double x,double y)
        {
            cvs.Children.Add(DrawCusorLine);
            DrawCusorLine.X1 = x;
            DrawCusorLine.Y1 = y;
            DrawCusorLine.X2 = x;
            DrawCusorLine.Y2 = y;
            cvs.MouseMove += DragDrawing;
        }

        public void DragDrawing(object sender, MouseEventArgs e) {
            var pos = e.GetPosition(cvs);

            //pos应该向X1 X2方向移动1个单位
            Vector p1 = new Vector(DrawCusorLine.X1, DrawCusorLine.Y1);
            Vector p2 = new Vector(pos.X, pos.Y);
            Vector dec = p2 - p1;
            if (dec.Length < 0)
                p2 = p1;
            else
            {
                dec.Normalize();
                p2 = p2 - dec;
            }

            DrawCusorLine.X2 = p2.X;
            DrawCusorLine.Y2 = p2.Y;
        }


        private void EndDragDraw()
        {
            cvs.Children.Remove(DrawCusorLine);
            cvs.MouseMove -= DragDrawing;
        }

        /// <summary>
        /// 对cvs中的Children进行排序，要求优先级相同保持顺序
        /// </summary>
        private Dictionary<Type, int> SortOrderBuff = new Dictionary<Type, int>();
        private void KeepSort(object o, EventArgs e)
        {
            int GetByType(Type t)
            {
                if (SortOrderBuff.ContainsKey(t))
                {
                    return SortOrderBuff[t];
                }
                else
                {
                    var tuple = (
                        from s in SortOrder
                        where t.IsSubclassOf(s.Item1)
                        select s
                        ).First();
                    SortOrderBuff.Add(t, tuple.Item2);
                    return tuple.Item2;
                }
            }

            var ch = cvs.Children;
            bool needFix = false;
            for(int i = 1; i < ch.Count; i++)
            {
                if(GetByType(ch[i-1].GetType()) > GetByType(ch[i].GetType())){
                    needFix = true;
                    break;
                }
            }
            if (needFix)
            {
                //桶堆栈，修复顺序
                Queue<UIElement>[] elements = new Queue<UIElement>[256];
                foreach(UIElement uielem in ch)
                {
                    int p = GetByType(uielem.GetType());
                    if (elements[p] == null)
                        elements[p] = new Queue<UIElement>();
                    elements[p].Enqueue(uielem);
                }
                ch.Clear();
                for(int i = 0; i < 256; i++)
                {
                    if (elements[i] != null)
                        for (int x = elements[i].Count(); x > 0; x--)
                            ch.Add(elements[i].Dequeue());
                }
            }
            cvs.UpdateLayout();
        }

        private void TipInBase(String str,bool showNow = false)
        {
            int enterpos = str.IndexOf('\n');
            if (enterpos == -1)
            {
                BaseLabel.Content = str;
                BaseLabel.Tag = "";
            }
               
            else
            {
                BaseLabel.Content = str.Substring(0, enterpos);
                BaseLabel.Tag = str.Substring(enterpos);
            }
            if (showNow)
                BaseLabel_MouseDoubleClick(null, null);
        }

        private void BaseLabel_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show(BaseLabel.Content.ToString() + "\n" + BaseLabel.Tag?.ToString(),"阅读提示");
        }

        private void MenuItem_File_Quit_Click(object sender, RoutedEventArgs e)
        {
            TipInBase("ByeBye~", true);
            Close();
        }
    }

}
