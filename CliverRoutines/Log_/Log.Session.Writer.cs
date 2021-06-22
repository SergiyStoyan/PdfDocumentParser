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

namespace Cliver
{
    public partial class Log
    {
        public partial class Session
        {
            public abstract partial class Writer : Log.Writer
            {
                internal Writer(string name, Session session) : base(name)
                {
                    Session = session;
                    SetFile();
                }

                /// <summary>
                /// Message importance level.
                /// </summary>
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
                            {
                                setWorkDir(true);
                                Directory.CreateDirectory(Session.Dir);
                            }
                            level = value;
                        }
                    }
                }

                override internal void SetFile()
                {
                    lock (this)
                    {
                        string file2 = Session.Dir + System.IO.Path.DirectorySeparatorChar;
                        if (Log.mode.HasFlag(Mode.FOLDER_PER_SESSION))
                        {
                            file2 += DateTime.Now.ToString("yyMMddHHmmss");
                        }
                        else //if (Log.mode.HasFlag(Mode.ONE_FOLDER))//default
                        {
                            //file2 += (string.IsNullOrWhiteSpace(Session.Name) ? "" : Session.Name + "_") + Session.TimeMark;//separates session name from log name
                            file2 += Session.TimeMark + (string.IsNullOrWhiteSpace(Session.Name) ? "" : "_" + Session.Name);
                        }
                        file2 += (string.IsNullOrWhiteSpace(Name) ? "" : "_" + Name) + (fileCounter > 0 ? "[" + fileCounter + "]" : "") + "." + FileExtension;

                        if (File == file2)
                            return;
                        if (logWriter != null)
                            logWriter.Close();
                        File = file2;
                    }
                }

                /// <summary>
                /// Session to which this log belongs.
                /// </summary>
                public readonly Session Session;
            }
        }
    }
}