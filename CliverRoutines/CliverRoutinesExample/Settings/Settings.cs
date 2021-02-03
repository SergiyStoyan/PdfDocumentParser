using System;
using System.Collections.Generic;
using System.Text;
using Cliver;

namespace Example
{
    class Settings
    {
        /// <summary>
        ///Settings type field can be declared anywhere in the code. It must be static to be processed by Config.
        ///Also, it can be declared readonly which is optional because sometimes the logic of the app may require replacing the value.
        /// </summary>
        internal static GeneralSettings General;

        [Settings(indented: false)]
        internal static SmtpSettings Smtp { get; set; }
    }
}
