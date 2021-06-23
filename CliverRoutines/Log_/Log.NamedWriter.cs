//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey.stoyan@hotmail.com
//        stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Cliver
{
    public partial class Log
    {
        /// <summary> 
        /// A session-less named log writer that allows to write the same log file directly to WorkDir. 
        /// </summary>
        public partial class NamedWriter : Writer
        {
            /// <summary>
            /// Creates or retrieves a session-less named log writer that allows to write the same log file directly to WorkDir. 
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            static public NamedWriter Get(string name)
            {
                lock (names2NamedWriter)
                {
                    if (!names2NamedWriter.TryGetValue(name, out NamedWriter nw))
                    {
                        nw = new NamedWriter(name);
                        nw.SetFile();
                        names2NamedWriter.Add(name, nw);
                    }
                    return nw;
                }
            }
            static Dictionary<string, NamedWriter> names2NamedWriter = new Dictionary<string, NamedWriter>();

            NamedWriter(string name) : base(name) { }

            override public Level Level
            {
                get
                {
                    return level;
                }
                set
                {
                    lock (this)
                    {
                        if (level == Level.NONE && value > Level.NONE)
                            setWorkDir(true);
                        level = value;
                    }
                }
            }

            override internal void SetFile()
            {
                lock (this)
                {
                    //(!)it must differ from the session files to avoid sharing
                    string file2 = WorkDir + Path.DirectorySeparatorChar + "__" + Name + (fileCounter > 0 ? "[" + fileCounter + "]" : "") + "." + FileExtension;

                    if (File == file2)
                        return;
                    if (logWriter != null)
                        logWriter.Close();
                    File = file2;
                }
            }
        }
    }
}