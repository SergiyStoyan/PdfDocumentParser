//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey.stoyan@hotmail.com
//        stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************
using System.Runtime.InteropServices;


namespace Cliver
{
    public static class ConsoleRoutines
    {
        public static bool OpenConsole()
        {
            open = AllocConsole();
            return open;
        }

        public static bool IsConsoleOpen
        {
            get
            {
                return open;
            }
        }
        static bool open = false;

        public static bool CloseConsole()
        {
            open = !FreeConsole();
            return !open;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool FreeConsole();
    }
}

