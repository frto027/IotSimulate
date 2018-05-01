using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using IoTSimulate;
using System.Windows.Controls;
using System.Windows.Input;
using SimuWindows.VtmModule;

namespace SimuWindows
{
    /// <summary>
    /// VTM-C开发板
    /// </summary>
    class VtmDevCanvas : DragCanvas
    {
        public VtmDev dev;
        GlobalGUIManager global;


        readonly ComCanvas[] coms = new ComCanvas[VtmDev.UART_COUNT];

        public IDictionary<VtmModuleBase.Position, VtmModuleBase> ModuleDictionary =
            new Dictionary<VtmModuleBase.Position, VtmModuleBase>();
        public readonly List<VtmModuleBase> NonePositionModuleList = new List<VtmModuleBase>();

        private IDictionary<VtmModuleBase.Position, BlackModule> BlackModule;

        Label StatusLabel = new Label()
        {
            Margin = new Thickness(85, 0, 0, 0),
            IsHitTestVisible = false,
            Content = "Ready",
            Background = Brushes.YellowGreen
        };

        

        DispatcherTimer timer = new DispatcherTimer();

        ClickEventPoint RunButton = new ClickEventPoint(60, 0)
        {
            Background = Brushes.Green,
            Width = 20,Height = 20
        };

        public VtmDevCanvas(GlobalGUIManager global,String binpath) : base(global.rootcvs)
        {
            this.global = global;

            Width = 500;
            Height = 360;
            Background = Brushes.WhiteSmoke;

            AddClickPoint(new RemoveClickPoint(0, 0, this));
            AddClickPoint(RunButton);
            
            Children.Add(StatusLabel);
            //Dev
            dev = new VtmDev(binpath);
            //ComCanvas
            for(int i = 0; i < coms.Length; i++)
            {
                coms[i] = new ComCanvas(30 + 50 * i, 310, global, dev.GetComPortBase(i));
                AddClickPoint(coms[i]);
            }

            //title
            Children.Add(new Label()
            {
                Content = "VTM-C",
                FontSize = 30,
                Foreground = Brushes.SlateBlue,
                Margin = new Thickness(20, 320, 0, 0),
                IsHitTestVisible = false
            });
            

            timer.Tick += Update;
            timer.Interval = TimeSpan.FromMilliseconds(50);

            RunButton.OnClickEvent += Run;

            //BlackModule
            BlackModule = VtmModule.BlackModule.NewBlackModules(this,global);
            foreach(var m in BlackModule)
            {
                Children.Add(m.Value);
                m.Value.FitPosition(m.Key);
            }

            /*
            //LedModule
            AddModule(typeof(LedModule));
            */
        }
        public string AddModule(Type type,bool autoPos = true,VtmModuleBase.Position position = VtmModuleBase.Position.NONE)
        {
            if (type != typeof(VtmModuleBase) && type.IsSubclassOf(typeof(VtmModuleBase)) == false )
            {
                return "无法创建\n类型不匹配：" + type.ToString();
            }
            var Constructor = type.GetConstructor(new Type[] { typeof(VtmDev),typeof(GlobalGUIManager) });
            if(Constructor == null)
            {
                return "构造函数不匹配\n找不到参数为VtmDev的构造函数";
            }

            var finalPos = VtmModuleBase.Position.NONE;
            bool hasPos = false;
            
            foreach(VtmModuleBase.Position classPosition in ModulePositionAttribute.GetPositionsByType(type))
            {
                if (!autoPos)//手动指定建议位置
                {
                    //如果不满足则跳过
                    if (classPosition != position)
                        continue;
                }
                if(ModuleDictionary.ContainsKey(classPosition) == false)
                {
                    hasPos = true;
                    finalPos = classPosition;
                    break;
                }
            }
            if (hasPos == false)
            {
                return "位置不足";
            }
            VtmModuleBase module = Constructor.Invoke(new object[] { dev,global }) as VtmModuleBase;
            if(module == null)
            {
                return "创建对象失败";
            }
            return AddModuleInstance(module, finalPos);
        }

        private string AddModuleInstance(VtmModuleBase module,VtmModuleBase.Position position)
        {
            if(module == null)
            {
                return "不能添加null模块到VtmDev";
            }
            if(ModuleDictionary.ContainsKey(position))
            {
                RemoveModule(position);
            }
            if (BlackModule.ContainsKey(position))
                BlackModule[position].Visibility = Visibility.Hidden;
            if(position != VtmModuleBase.Position.NONE)
            {
                Children.Add(module);
                ModuleDictionary[position] = module;
            }
            else
            {
                Children.Add(module);
                NonePositionModuleList.Add(module);
            }
            module.AddMenuSeparator();
            module.AddMenuItem("移除", () => RemoveModule(module));
            module.position = position;
            module.FitPosition(position);
            return null;
        }

        public bool RemoveModule(VtmModuleBase.Position position)
        {
            if(position != VtmModuleBase.Position.NONE)
            {
                var module = ModuleDictionary[position];

                if (BlackModule.ContainsKey(position))
                {
                    BlackModule[position].Visibility = Visibility.Visible;
                }

                if (module != null)
                {
                    Children.Remove(module);
                    module?.Remove();
                    ModuleDictionary.Remove(position);
                    return true;
                }
                
            }
            return false;
        }
        public bool RemoveModule(VtmModuleBase module)
        {
            foreach(var v in ModuleDictionary)
            {
                if (v.Value == module)
                {
                    RemoveModule(v.Key);
                    return true;
                }
            }
            if (NonePositionModuleList.Remove(module))
            {
                Children.Remove(module);
                module.Remove();
                return true;
            }
            return false;
        }
        
        void Run()
        {
            if (dev.IsExit)
            {
                dev.Start();
                timer.Start();
                RunButton.Background = Brushes.Red;
                StatusLabel.Content = "Running";
                StatusLabel.Background = Brushes.Green;
            }
            else
            {
                dev.InformStop();
            }
        }



        private void Update(object sender, EventArgs e)
        {
            if (dev.IsExit)
            {
                StatusLabel.Content = "Stop";
                StatusLabel.Background = Brushes.Red;
                RunButton.Background = Brushes.Green;
                timer.Stop();
                return;
            }

            dev.Update();

            foreach (var v in ModuleDictionary)
                v.Value.Update();
            foreach (var v in NonePositionModuleList)
                v.Update();
            
        }

       

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            foreach (var c in coms)
                c?.Update();
        }

        public override void Remove()
        {
            timer.Stop();
            try
            {
                dev?.Close();
            }catch(Exception e)
            {
                global.TipText(e.ToString());
            }

            foreach(var com in coms)
            {
                com?.Remove();
            }

            foreach(var d in ModuleDictionary)
            {
                d.Value.Remove();
            }
            foreach(var d in NonePositionModuleList)
            {
                d.Remove();
            }
            base.Remove();
        }
    }
}
