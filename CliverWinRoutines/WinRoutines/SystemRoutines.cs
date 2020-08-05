/********************************************************************************************
        Author: Sergey Stoyan
        sergey.stoyan@gmail.com
        sergey.stoyan@hotmail.com
        stoyan@cliversoft.com
        http://www.cliversoft.com
********************************************************************************************/
using System.Diagnostics;

namespace Cliver.Win
{
    public static class SystemRoutines
    {
        public static void StartShutDown(string param)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "cmd";
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.Arguments = "/C shutdown " + param;
            Process.Start(psi);
        }
    }
}

