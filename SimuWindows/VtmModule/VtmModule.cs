using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using IoTSimulate;
namespace SimuWindows.VtmModule
{
    public abstract class VtmModuleBase : Canvas
    {
        protected VtmDev VtmDev;

        public enum Position { A, B, C, D, NONE };

        public Position position;//不要修改，没用

        /// <summary>
        /// 各模块槽位的坐标
        /// </summary>
        public readonly static IDictionary<Position, Thickness> MarginDictionary = new Dictionary<Position, Thickness>
        {
            {Position.A,new Thickness(20,40,0,0) },
            {Position.B,new Thickness(20,150,0,0) },
            {Position.C,new Thickness(130,40,0,0) },
            {Position.D,new Thickness(130,150,0,0) },
        };
        /// <summary>
        /// 各模块槽位的大小
        /// </summary>
        public readonly static IDictionary<Position, Tuple<double, double>> SizeDictionary = new Dictionary<Position, Tuple<double, double>> {
            {Position.A,new Tuple<double, double>(100,100) },
            {Position.B,new Tuple<double, double>(100,100) },
            {Position.C,new Tuple<double, double>(100,100) },
            {Position.D,new Tuple<double, double>(100,100) },
        };

        public VtmModuleBase(VtmDev dev)
        {
            //IsHitTestVisible = false;
            Cursor = Cursors.Cross;
            Background = Brushes.Green;
            ContextMenu = new ContextMenu();
            
            VtmDev = dev;
        }

        public void AddMenuSeparator()
        {
            ContextMenu.Items.Add(new Separator());
        }
        public void AddMenuItem(string name,Action action)
        {
            MenuItem item = new MenuItem() { Header = name };
            item.Click += (s, e) => action();
            ContextMenu.Items.Add(item);
        }
        /// <summary>
        /// 这里修改Module的位置和尺寸
        /// </summary>
        /// <param name="position"></param>
        public virtual void FitPosition(Position position)
        {
            var margin = MarginDictionary[position];
            Margin = margin;

            var newsize = SizeDictionary[position];
            Width = newsize.Item1;
            Height = newsize.Item2;
        }

        public virtual void Update()
        {

        }
        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonUp(e);
        }
        public virtual void Remove()
        {

        }
    }

    public class ModulePositionAttribute : Attribute
    {
        public static IEnumerable<VtmModuleBase.Position> GetPositionsByType(Type t)
        {
            return t.GetCustomAttributes(typeof(ModulePositionAttribute), true).SelectMany((obj) => (obj as ModulePositionAttribute).position);
        }
        public VtmModuleBase.Position [] position;
        public ModulePositionAttribute(params VtmModuleBase.Position [] position)
        {
            this.position = position;
        }
    }
}
