//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************

namespace Cliver.Win
{
    public static class ImageRoutines
    {
        public static System.Windows.Media.ImageSource ToImageSource(this System.Drawing.Icon icon)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                System.Windows.Int32Rect.Empty,
                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
        }
    }
}