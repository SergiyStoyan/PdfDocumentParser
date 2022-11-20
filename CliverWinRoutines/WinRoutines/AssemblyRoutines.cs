/********************************************************************************************
        Author: Sergiy Stoyan
        s.y.stoyan@gmail.com, sergiy.stoyan@outlook.com, stoyan@cliversoft.com
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
            if (assembly == null)
                assembly = Assembly.GetEntryAssembly();
            string p = assembly.Location;
            if (p.StartsWith(@"\\"))
                try
                {
                    p = PathRoutines.GetLocalPathForUncPath(p);
                }
                catch (System.UnauthorizedAccessException)
                {
                    return null;
                }
            return System.Drawing.Icon.ExtractAssociatedIcon(p);
        }
    }
}