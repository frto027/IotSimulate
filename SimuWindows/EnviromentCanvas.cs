using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using IoTSimulate;

namespace SimuWindows
{
    /// <summary>
    /// 模拟用户环境
    /// </summary>
    class EnviromentCanvas:DragCanvas,IEnviroment
    {
        /// <summary>
        /// 所有的需要获取环境信息的物体，请把自己加入到这个列表中
        /// </summary>
        public static readonly IList<IEnviromentSetable> EnvSetableList = new List<IEnviromentSetable>();

        DispatcherTimer updateTimer = new DispatcherTimer();

        Canvas rootcvs;

        public static readonly float maxDistance = 200;

        Slider tempSlider = new Slider()
        {
            Margin = new Thickness(100,25,0,0),
            Width = 100,
            Height = 40,
            Cursor = Cursors.Arrow,
            Minimum = -30,Maximum=50,Value=25,
        };
        Label tempLabel = new Label()
        {
            Content = "??",
            Margin = new Thickness(45, 20, 0, 0),
            IsHitTestVisible = false
        };

        Slider lightSlider = new Slider()
        {
            Margin = new Thickness(100, 45, 0, 0),
            Width = 100,
            Height = 40,
            Cursor = Cursors.Arrow,
            Minimum = 0,
            Maximum = 200,
            Value = 50,
        };
        Label lightLabel = new Label()
        {
            Content = "??",
            Margin = new Thickness(45, 40, 0, 0),
            IsHitTestVisible = false
        };

        public EnviromentCanvas(GlobalGUIManager manager) : base(manager.rootcvs)
        {
            Width = 200;
            Height = 80;
            Background = Brushes.Orange;

            rootcvs = manager.rootcvs;

            AddClickPoint(new RemoveClickPoint(0, 0, this));

            Children.Add(new Label()
            {
                Content = "Enviroment",
                FontSize = 16,
                Margin = new Thickness(20, 0, 0, 0),
                Foreground = Brushes.Blue,
                IsHitTestVisible = false
            });

            Label tempTipLabel = new Label()
            {
                Content = "Temp:",
                Margin = new Thickness(0, 20, 0, 0),
                IsHitTestVisible = false
            };
            Children.Add(tempTipLabel);
            Children.Add(tempSlider);
            Children.Add(tempLabel);

            Label lightTipLabel = new Label()
            {
                Content = "Light:",
                Margin = new Thickness(0, 40, 0, 0),
                IsHitTestVisible = false
            };
            Children.Add(lightTipLabel);
            Children.Add(lightSlider);
            Children.Add(lightLabel);

            updateTimer.Tick += Update;
            updateTimer.Interval = TimeSpan.FromMilliseconds(200);
            updateTimer.Start();

            Children.Add(new Canvas()
            {
                IsHitTestVisible = false,
                Margin = new Thickness(-maxDistance, -maxDistance, 0, 0),
                Width = 2 * maxDistance, Height = 2 * maxDistance,
                Background = new DrawingBrush()
                {
                    Drawing = new GeometryDrawing()
                    {
                        Geometry = new EllipseGeometry(new Point(200, 200), 200, 200),
                        Pen = new Pen(Brushes.Black, 1) {
                            DashStyle = new DashStyle(new double[] { 8,6,6,6}, 0)
                        },
                        Brush = Brushes.Transparent
                    }
                }
            });

        }

        public override void Remove()
        {
            updateTimer.Stop();

            foreach (IEnviromentSetable sd in EnvSetableList)
            {
                sd.CancelEnv(this);
            }

            base.Remove();
        }

        float IEnviroment.GetLight()
        {
            throw new NotImplementedException();
        }

        float IEnviroment.GetTemperature()
        {
            return (float)tempSlider.Value;
        }

        private void Update(object sender, EventArgs e)
        {
            tempLabel.Content = tempSlider.Value.ToString("F2");
            lightLabel.Content = lightSlider.Value.ToString("F2");
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            UpdateEnviroments();
        }

        public void UpdateEnviroments()
        {
            foreach(IEnviromentSetable sd in EnvSetableList)
            {
                float x, y;
                sd.GetXY(out x, out y);
                x -= (float)Margin.Left;
                y -= (float)Margin.Top;
                if(x*x + y*y < maxDistance * maxDistance)
                {
                    sd.SetEnv(this);
                }
                else
                {
                    sd.CancelEnv(this);
                }
            }
        }
    }
}
