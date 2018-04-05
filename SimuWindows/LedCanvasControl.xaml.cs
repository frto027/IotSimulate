using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
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

namespace SimuWindows
{
    /// <summary>
    /// LedCanvas.xaml 的交互逻辑
    /// </summary>
    public partial class LedCanvasControl : UserControl
    {
        public byte Value = 0;
        private static Bitmap[] bitmaps;
        private static volatile BitmapSource[]rowImgs;

        public static volatile bool FirstInitComplete = false;

        public static int[] ScaleRates = new int[] { 100, 50, 30, 10 };
        private static volatile int ScaleRate;

        private static Thread InitThread = null;

        public LedCanvasControl()
        {
            InitializeComponent();

            //之前没启动过初始化，启动
            if(rowImgs == null)
            {
                StartInitAsync();
            }
            //首次初始化未完成，等待
            while(FirstInitComplete == false)
            {
                new LoadingForm(() => FirstInitComplete).ShowDialog();
            }

            img.Source = GetBitmapInList(0);
        }

        [DllImport("gdi32")]
        static extern int DeleteObject(IntPtr o);

        public static BitmapSource GetMapSource(Bitmap pic)
        {
            IntPtr ip = pic.GetHbitmap();
            BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                ip, IntPtr.Zero, Int32Rect.Empty,
                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            DeleteObject(ip);
            return bitmapSource;
        }
        private static Bitmap[] ScaledBitmap;
        public static void StartInitAsync()
        {
            if(InitThread != null)
            {
                return;
            }
            InitThread =  new Thread(FirstInit);
            InitThread.Start();
        }
        public static void AbortInitAsync()
        {
            if(InitThread != null && InitThread.ThreadState == ThreadState.Running)
            {
                InitThread.Abort();
            }
        }
        public static void FirstInit()
        {
            if (rowImgs != null)
                return;
            rowImgs = new BitmapSource[0x100];
            bitmaps = new Bitmap[0x100];
            foreach(var scale in ScaleRates)
            {
                //每次迭代初始化
                ScaleRate = scale;
                InitScaledBitmap();
                SetListTask();
                FirstInitComplete = true;
            }
        }

        private static void InitScaledBitmap()
        {
            ScaledBitmap = new Bitmap[9];
            ScaledBitmap[0] = LedImgResource.LED_Dark;
            ScaledBitmap[0] = new Bitmap(ScaledBitmap[0], new System.Drawing.Size(ScaledBitmap[0].Size.Width / ScaleRate, ScaledBitmap[0].Size.Height / ScaleRate));

            ScaledBitmap[1] = new Bitmap(LedImgResource.LED_Light_A, ScaledBitmap[0].Size);
            ScaledBitmap[2] = new Bitmap(LedImgResource.LED_Light_B, ScaledBitmap[0].Size);
            ScaledBitmap[3] = new Bitmap(LedImgResource.LED_Light_C, ScaledBitmap[0].Size);
            ScaledBitmap[4] = new Bitmap(LedImgResource.LED_Light_D, ScaledBitmap[0].Size);
            ScaledBitmap[5] = new Bitmap(LedImgResource.LED_Light_E, ScaledBitmap[0].Size);
            ScaledBitmap[6] = new Bitmap(LedImgResource.LED_Light_F, ScaledBitmap[0].Size);
            ScaledBitmap[7] = new Bitmap(LedImgResource.LED_Light_G, ScaledBitmap[0].Size);
            ScaledBitmap[8] = new Bitmap(LedImgResource.LED_Light_H, ScaledBitmap[0].Size);
        }
        /// <summary>
        /// 将编号为i的图像加载到内存中
        /// </summary>
        private static void SetListTask()
        {
            for(int i = 0; i < 0x100; i++)
            {
                Bitmap bitmap = new Bitmap(ScaledBitmap[0]);
                for (int w = 0; w < 8; w++)
                {
                    if ((i & (1 << w)) != 0)
                    {
                        Bitmap other = ScaledBitmap[w + 1];
                        for (int y = 0; y < bitmap.Height; y++)
                            for (int x = 0; x < bitmap.Width; x++)
                            {
                                var color = other.GetPixel(x, y);
                                if (color.A > 20)
                                {
                                    bitmap.SetPixel(x, y, color);
                                }
                            }
                    }
                }
                lock (rowImgs)
                {
                    lock (bitmaps)
                    {
                        bitmaps[i] = bitmap;
                    }
                    rowImgs[i] = null;
                }
            }
        }
        
        private static BitmapSource GetBitmapInList(byte i)
        {
            lock (rowImgs)
            {
                if(rowImgs[i] == null)
                {
                    lock(bitmaps){
                        rowImgs[i] = GetMapSource(bitmaps[i]);
                    }
                }
                return rowImgs[i];
            }
        }
        public void Update()
        {
            img.Source = GetBitmapInList(Value);
        }

    }
}
