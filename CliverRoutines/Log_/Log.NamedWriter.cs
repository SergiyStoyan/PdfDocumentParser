//********************************************************************************************
//Author: Sergiy Stoyan
//        systoyan@gmail.com
//        sergiy.stoyan@outlook.com
//        stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Cliver
{
    public partial class Log
    {
        /// <summary> 
        /// A session-less named log writer that allows to write the same log file directly to RootDir. 
        /// </summary>
        public partial class NamedWriter : Writer
        {
            /// <summary>
            /// Creates or retrieves a session-less log writer which allows continuous writing to the same log file in Log.RootDir. 
            /// </summary>
            /// <param name="name">log name</param>
            /// <returns>wirter</returns>
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

            /// <summary>
            /// Close all the session-less log files. 
            /// </summary>
            static internal void CloseAll()
            {
                lock (names2NamedWriter)
                {
                    if (names2NamedWriter.Values.FirstOrDefault(a => !a.IsClosed) == null)
                        return;

                    Log.Write("Closing the session-less logs...");

                    foreach (NamedWriter nw in names2NamedWriter.Values)
                        nw.Close();
                    //names2NamedWriter.Clear(); !!! clearing writers will bring to duplicating them if they are referenced in the calling code.
                }
            }

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
                            setRootDir(true);
                        level = value;
                    }
                }
            }

            override internal void SetFile()
            {
                lock (this)
                {
                    //(!)it must differ from the session files to avoid sharing
                    string file2 = Log.RootDir + Path.DirectorySeparatorChar + "_" + Name + (fileCounter > 0 ? "[" + fileCounter + "]" : "") + "." + FileExtension;

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