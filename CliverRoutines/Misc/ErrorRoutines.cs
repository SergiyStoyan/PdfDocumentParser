using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Cliver
{
    public class ErrorRoutines
    {
        public static Win32Exception GetLastError()
        {
            return new Win32Exception(Marshal.GetLastWin32Error());
        }

        public static string GetLastErrorMessage()
        {
            Win32Exception e = GetLastError();
            return e?.Message;
        }

        public static string GetErrorMessage(int errorCode)
        {
            return new Win32Exception(errorCode).Message;
        }
    }
}