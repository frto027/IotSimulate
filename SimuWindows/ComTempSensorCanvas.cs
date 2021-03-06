﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using IoTSimulate;

namespace SimuWindows
{
    class ComTempSensorCanvas:DragCanvas,IEnviromentSetable
    {
        ComTempSensor tempSensor = new ComTempSensor();
        ComCanvas comCanvas;

        DispatcherTimer timer = new DispatcherTimer();

        public ComTempSensorCanvas(GlobalGUIManager global) : base(global.rootcvs)
        {
            EnviromentCanvas.EnvSetableList.Add(this);

            Width = 130;Height = 60;
            AddClickPoint(new RemoveClickPoint(0, 0, this));

            SetupBackgrountStyle();

            Label titleLabel = new Label()
            {
                Margin = new Thickness(20, 1, 0, 0),
                Content = "温度传感器",
                FontSize = 20,
                IsHitTestVisible = false
            };
            Children.Add(titleLabel);
            


            comCanvas = new ComCanvas(30, 40, global, tempSensor);
            AddClickPoint(comCanvas);

            timer.Tick += Update;
            timer.Interval = TimeSpan.FromMilliseconds(200);
            timer.Start();
        }

        private void Update(object sender, EventArgs e)
        {
            tempSensor.Update();
        }



        public override void Remove()
        {
            timer.Stop();
            comCanvas.Remove();
            EnviromentCanvas.EnvSetableList.Remove(this);
            base.Remove();
        }

        void IEnviromentSetable.GetXY(out float x, out float y)
        {
            x = (float)Margin.Left;
            y = (float)Margin.Top;
        }

        void IEnviromentSetable.SetEnv(IEnviroment enviroment)
        {
            tempSensor.enviroment = enviroment;
        }

        void IEnviromentSetable.CancelEnv(IEnviroment enviroment)
        {
            if(tempSensor.enviroment == enviroment)
            {
                tempSensor.enviroment = null;
            }
        }
    }
}
