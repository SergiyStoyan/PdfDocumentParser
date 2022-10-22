//********************************************************************************************
//Author: Sergiy Stoyan
//        systoyan@gmail.com
//        sergiy.stoyan@outlook.com
//        stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************
using System.Runtime.InteropServices;
using System;


namespace Cliver.Win
{
    public static class ConsoleRoutines
    {
        public static bool OpenConsole()
        {
            open = AllocConsole();
            return open;
        }
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

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
        static extern bool FreeConsole();

        /// <summary>
        /// Required when debugging in VS
        /// </summary>
        private static void OverrideRedirection()
        {
            var hOut = WinApi.Kernel32.GetStdHandle(WinApi.Kernel32.StdHandles.STD_OUTPUT_HANDLE);
            var hRealOut = WinApi.Kernel32.CreateFile("CONOUT$", WinApi.Kernel32.dwDesiredAccess.GENERIC_READ | WinApi.Kernel32.dwDesiredAccess.GENERIC_WRITE, WinApi.Kernel32.dwShareMode.FILE_SHARE_WRITE, null, WinApi.Kernel32.dwCreationDisposition.CREATE_NEW, 0, IntPtr.Zero);
            if (hRealOut != hOut)
            {
                WinApi.Kernel32.SetStdHandle(WinApi.Kernel32.StdHandles.STD_OUTPUT_HANDLE, hRealOut);
                Console.SetOut(new System.IO.StreamWriter(Console.OpenStandardOutput(), Console.OutputEncoding) { AutoFlush = true });
            }
        }
    }
}

