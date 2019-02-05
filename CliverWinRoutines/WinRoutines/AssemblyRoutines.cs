//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006, Sergey Stoyan
//********************************************************************************************

namespace Cliver.Win
{
    public static class AssemblyRoutines
    {
        public static System.Windows.Media.ImageSource GetAppIconImageSource()
        {
            return Cliver.AssemblyRoutines.GetAppIcon().ToImageSource();
        }
    }
}