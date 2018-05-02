using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SimuWindows
{
    public class IconRes
    {
        public static readonly Uri
            RemoveButton = new Uri("pack://application:,,,/SimuWindows;component/Icons/ic_clear_black_48dp.png"),
            PowerOn = new Uri("pack://application:,,,/SimuWindows;component/Icons/ic_power_settings_new_black_on_48dp.png"),
            PowerOff = new Uri("pack://application:,,,/SimuWindows;component/Icons/ic_power_settings_new_black_off_48dp.png"),
            PowerWarr = new Uri("pack://application:,,,/SimuWindows;component/Icons/ic_power_settings_new_black_warr_48dp.png");
        public static readonly ImageBrush
            RemoveButtonBrush = drawByUri(RemoveButton),
            PowerOnImgBrush = drawByUri(PowerOn),
            PowerOffImgBrush = drawByUri(PowerOff),
            PowerWarrImgBrush = drawByUri(PowerWarr);

        private static ImageBrush drawByUri(Uri uri)
        {
            return new ImageBrush() { ImageSource = new BitmapImage(uri) };
        }

    }
}
