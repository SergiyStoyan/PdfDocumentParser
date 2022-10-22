//********************************************************************************************
//Author: Sergiy Stoyan
//        s.y.stoyan@gmail.com
//        sergiy.stoyan@outlook.com
//        stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Linq;
using System.Collections.Generic;

namespace Cliver
{
    public partial class Log
    {
        public partial class Session
        {
            public partial class ThreadWriter
            {
                override public void Exit(string message)
                {
                    lock (this)
                    {
                        Log.Main.Write("EXITING: due to thread #" + Name + ". See the respective Log");
                        base.Exit(message);
                    }
                }

                override public void Exit2(string message)
                {
                    lock (this)
                    {
                        Log.Main.Write("EXITING: due to thread #" + Name + ". See the respective Log");
                        base.Exit2(message);
                    }
                }

                override public void Exit(Exception e)
                {
                    lock (this)
                    {
                        Log.Main.Write("EXITING: due to thread #" + Name + ". See the respective log.");
                        base.Exit(e);
                    }
                }
            }
        }
    }
}