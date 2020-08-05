/********************************************************************************************
        Author: Sergey Stoyan
        sergey.stoyan@gmail.com
        sergey.stoyan@hotmail.com
        stoyan@cliversoft.com
        http://www.cliversoft.com
********************************************************************************************/
using System.Reflection;

namespace Cliver.Win
{
    public static class AssemblyRoutines
    {
        public static System.Windows.Media.ImageSource GetAppIconImageSource()
        {
            return GetAppIcon().ToImageSource();
        }

        public static System.Drawing.Icon GetAppIcon(Assembly assembly = null)
        {
            return System.Drawing.Icon.ExtractAssociatedIcon((assembly != null ? assembly : Assembly.GetEntryAssembly()).Location);
        }
    }
}