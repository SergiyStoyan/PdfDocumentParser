/********************************************************************************************
        Author: Sergiy Stoyan
        s.y.stoyan@gmail.com
        sergiy.stoyan@outlook.com
        stoyan@cliversoft.com
        http://www.cliversoft.com
********************************************************************************************/

using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Cliver.Win
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

    public class LastErrorException : System.Exception
    {
        public LastErrorException(string message) : base(message, ErrorRoutines.GetLastError())
        {

        }
    }
}