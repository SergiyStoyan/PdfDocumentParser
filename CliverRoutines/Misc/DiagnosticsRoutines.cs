//********************************************************************************************
//Author: Sergiy Stoyan
//        systoyan@gmail.com
//        sergiy.stoyan@outlook.com
//        stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Linq;

namespace Cliver
{
    public static class DiagnosticsRoutines
    {
        /// <summary>
        /// Used to prevent calling a handler of changes when it itself does changes.
        /// </summary>
        /// <returns></returns>
        public static bool IsCallingMethodInStackTwice()
        {
            StackTrace st = new StackTrace(true);
            StackFrame sf = st.GetFrame(1);
            MethodBase method = sf.GetMethod();
            int frameI = 2;
            for (sf = st.GetFrame(frameI++); sf != null; sf = st.GetFrame(frameI++))
            {
                MethodBase m = sf.GetMethod();
                if (method == m)
                    return true;
            }
            return false;
        }
    }
}
