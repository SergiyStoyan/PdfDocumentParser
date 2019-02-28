//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006, Sergey Stoyan
//********************************************************************************************

using System;
using System.Windows.Forms;
//using System.Configuration;


namespace Cliver
{
    public partial class Log
    {
        public class Message
        {
            static object lock_variable = new object();

            static Message()
            {
                if (ProgramRoutines.IsWebContext)
                {
                    DisableStumblingDialogs = true;
                    Output2Console = false;
                }
            }

            public static Func<string, System.Drawing.Icon, string, string[], int, Form, int> ShowDialog = delegate (string title, System.Drawing.Icon icon, string message, string[] buttons, int defaultButtonId, Form owner)
             {
                 if (!DisableStumblingDialogs)
                     throw new Exception("Message dialog is not provided!\r\n\r\nMessage to be shown:\r\n" + message);
                 return 0;
             };

            /// <summary>
            /// Defines whether message boxes will be showed (run in manual mode) 
            /// </summary>
            static public bool DisableStumblingDialogs = false;

            //public enum LogMode
            //{
            //    NOT_SET,
            //    SHOW_DIALOGS,
            //    AUTOMATIC
            //}

            public static bool Output2Console = false;

            /// <summary>
            /// Receives owner window handle. It is needed to do message box owned.
            /// </summary>
            public static Form Owner;
            //{
            //    set
            //    {
            //        _Owner = value;
            //    }
            //    get
            //    {
            //        if (_Owner != null)
            //            return _Owner;
            //        return Application.OpenForms.Cast<Form>().Last();
            //    }
            //}
            //static Form _Owner = null;

            public static bool AskYesNo(string message, bool automatic_yes, bool write2log = true, Form owner = null)
            {
                lock (lock_variable)
                {
                    if (write2log)
                        Log.Main.Write(message);

                    if (!DisableStumblingDialogs)
                    {
                        if (!Output2Console)
                        {
                            //Cliver.MessageForm mf = new Cliver.MessageForm(Application.ProductName, System.Drawing.SystemIcons.Question, message, new string[2] { "Yes", "No" }, automatic_yes ? 0 : 1, Owner);
                            //mf.ShowInTaskbar = Cliver.Message.ShowInTaskbar;
                            //return mf.ShowDialog() == 0;
                            return 0 == ShowDialog(Application.ProductName, System.Drawing.SystemIcons.Question, message, new string[2] { "Yes", "No" }, automatic_yes ? 0 : 1, owner != null ? owner : Owner);
                        }
                        else
                        {
                            Console.WriteLine(message);
                            for (; ; )
                            {
                                Console.WriteLine("Enter Y[es] or N[o]:");
                                ConsoleKeyInfo cki = Console.ReadKey();
                                Console.WriteLine();
                                switch (cki.KeyChar.ToString().ToUpper())
                                {
                                    case "Y":
                                        return true;
                                    case "N":
                                        return false;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (Output2Console)
                        {
                            Console.WriteLine(message);
                            Console.WriteLine("Enter Y[es] or N[o]:");
                            Console.WriteLine("Choosen default: " + (automatic_yes ? "Y[es]" : "N[o]"));
                        }
                        return automatic_yes;
                    }
                }
            }

            public static void Error(string message, Form owner = null)
            {
                Log.Main.Error(message);
                Error_(message, owner);
            }

            public static void Error2(string message, Form owner = null)
            {
                Log.Main.Error2(message);
                Error_(message, owner);
            }

            static void Error_(string message, Form owner = null)
            {
                lock (lock_variable)
                {
                    if (!DisableStumblingDialogs)
                    {
                        if (!Output2Console)
                            ShowDialog(Application.ProductName, System.Drawing.SystemIcons.Error, message, new string[1] { "OK" }, 0, owner != null ? owner : Owner);
                        else
                            Console.WriteLine("ERROR: " + message);
                    }
                    else
                    {
                        if (Output2Console)
                            Console.WriteLine("ERROR: " + message);
                    }
                }
            }

            public static void Error(Exception e, Form owner = null)
            {
                Error(Log.GetExceptionMessage(e), owner);
            }

            public static void Error2(Exception e, Form owner = null)
            {
                Error2(Log.GetExceptionMessage(e), owner);
            }

            public static void Exit(string message, Form owner = null)
            {
                Exit_(message, owner);
                Log.Main.Exit(message);
            }

            public static void Exit2(string message, Form owner = null)
            {
                Exit_(message, owner);
                Log.Main.Exit2(message);
            }

            public static void Exit_(string message, Form owner = null)
            {
                lock (lock_variable)
                {
                    if (!DisableStumblingDialogs)
                    {
                        if (!Output2Console)
                            ShowDialog(Application.ProductName, System.Drawing.SystemIcons.Error, message, new string[1] { "OK" }, 0, owner != null ? owner : Owner);
                        else
                        {
                            Console.WriteLine("EXIT: " + message);
                            Console.WriteLine("Press any key to quit...");
                            Console.ReadKey();
                        }
                    }
                    else
                    {
                        if (Output2Console)
                            Console.WriteLine("EXIT: " + message);
                    }
                }
            }

            public static void Exit(Exception e)
            {
                Exit(Log.GetExceptionMessage(e));
            }

            public static void Inform(string message, Form owner = null)
            {
                Log.Main.Inform(message);
                lock (lock_variable)
                {
                    if (!DisableStumblingDialogs)
                    {
                        if (!Output2Console)
                            ShowDialog(Application.ProductName, System.Drawing.SystemIcons.Information, message, new string[1] { "OK" }, 0, owner != null ? owner : Owner);
                        else
                            Console.WriteLine(message);
                    }
                    else
                    {
                        if (Output2Console)
                            Console.WriteLine(message);
                    }
                }
            }

            public static void Inform(Exception e, Form owner = null)
            {
                Inform(e.Message, owner);
            }

            public static void Warning(string message, Form owner = null)
            {
                Log.Main.Warning(message);
                lock (lock_variable)
                {
                    if (!DisableStumblingDialogs)
                    {
                        if (!Output2Console)
                            ShowDialog(Application.ProductName, System.Drawing.SystemIcons.Warning, message, new string[1] { "OK" }, 0, owner != null ? owner : Owner);
                        else
                            Console.WriteLine(message);
                    }
                    else
                    {
                        if (Output2Console)
                            Console.WriteLine(message);
                    }
                }
            }

            public static void Exclaim(string message, Form owner = null)
            {
                Log.Main.Warning(message);
                lock (lock_variable)
                {
                    if (!DisableStumblingDialogs)
                    {
                        if (!Output2Console)
                            ShowDialog(Application.ProductName, System.Drawing.SystemIcons.Exclamation, message, new string[1] { "OK" }, 0, owner != null ? owner : Owner);
                        else
                            Console.WriteLine(message);
                    }
                    else
                    {
                        if (Output2Console)
                            Console.WriteLine(message);
                    }
                }
            }

            public static void Warning(Exception e, Form owner = null)
            {
                Warning(e.Message, owner);
            }

            public static void Write(string message, Form owner = null)
            {
                Log.Main.Write(message);
                lock (lock_variable)
                {
                    if (!DisableStumblingDialogs)
                    {
                        if (!Output2Console)
                            ShowDialog(Application.ProductName, System.Drawing.SystemIcons.Information, message, new string[1] { "OK" }, 0, owner != null ? owner : Owner);
                        else
                            Console.WriteLine(message);
                    }
                    else
                    {
                        if (Output2Console)
                            Console.WriteLine(message);
                    }
                }
            }
        }
    }
}