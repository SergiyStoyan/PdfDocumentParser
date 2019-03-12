//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006, Sergey Stoyan
//********************************************************************************************
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