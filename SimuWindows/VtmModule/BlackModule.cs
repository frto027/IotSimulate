using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using IoTSimulate;

namespace SimuWindows.VtmModule
{
    /// <summary>
    /// 空白Module，比较特殊，不要参考
    /// </summary>
    class BlackModule : VtmModuleBase
    {

        public static IDictionary<Position, BlackModule> NewBlackModules(VtmDevCanvas canvas)
        {
            return new Dictionary<Position, BlackModule> {
                { Position.A,new BlackModule(null,Position.A,canvas) },
                { Position.B,new BlackModule(null,Position.B,canvas) },
                {Position.C,new BlackModule(null,Position.C,canvas) },
                {Position.D,new BlackModule(null,Position.D,canvas) }
            };
        }

        private static Tuple<Type, string>[] ModuleList = new Tuple<Type,string>[] {
            new Tuple<Type,string>(typeof(LedModule),"数码管"),
        };

        private List<MenuItem> menuItems = new List<MenuItem>();

        VtmDevCanvas VtmDevCanvas;

        public BlackModule(VtmDev dev,Position position,VtmDevCanvas canvas) : base(dev)
        {
            VtmDevCanvas = canvas;
            Children.Add(new Label() { Content = position.ToString()});

            this.position = position;

            //为所有此位置的模块创建menuItems
            foreach(var moduleInfo in 
                from info in ModuleList
                where ModulePositionAttribute.GetPositionsByType(info.Item1).Contains(position)
                select info)
            {
                //moduleInfo中所有Type都是符合position的
                MenuItem menuItem = new MenuItem() { Header = moduleInfo.Item2 };
                menuItem.Click += AddItem;
                menuItem.Tag = moduleInfo.Item1;
                menuItems.Add(menuItem);
            }
        }

        private void AddItem(object sender, System.Windows.RoutedEventArgs e)
        {
            Type type = (sender as MenuItem)?.Tag as Type;
            if(type != null)
            {
                AddModule(type);
            }
        }

        private void AddModule(Type t)
        {
            VtmDevCanvas.AddModule(t, false, position);
        }

        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            ContextMenu.Items.Clear();
            
            foreach(var okItem in from item in menuItems where
                                  VtmDevCanvas.ModuleDictionary.Values.Count((instance) => ((Type)(item.Tag)).IsInstanceOfType(instance))
                                  == 0
                                  select item)
            {
                ContextMenu.Items.Add(okItem);
            }

            base.OnContextMenuOpening(e);
        }


    }
}
