//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006, Sergey Stoyan
//********************************************************************************************

//CONTROLLING TEST PERIOD CLASS

//sample #1 of using:
/*
 
InternetDateTime.CHECK_TEST_PERIOD_VALIDITY(2008, 11, 8);  
 
*/

//sample #2 of using:
/*
 
#define TEST_VERSION

#if TEST_VERSION && !DEBUG
				if(new DateTime(2006, 3, 11) < InternetDateTime.GetOverHttps())
					LogMessage.Exit("Test time expired");
#endif	  
 
*/

using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Net;
using System.Reflection;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;

namespace Cliver
{
    /// <summary>
    /// Get current time from internet. Used to control test period.
    /// </summary>
    public class InternetDateTime
    {
        /// <summary>
        /// Check if the assembly was compiled as release version
        /// </summary>
        /// <param name="assembly">assembly to check</param>
        /// <returns></returns>
        static public bool IsReleaseVersion(Assembly assembly)
        {
            object[] attributes = assembly.GetCustomAttributes(typeof(DebuggableAttribute), true);
            if (attributes == null || attributes.Length == 0)
                return true;

            DebuggableAttribute d = (DebuggableAttribute)attributes[0];
            if ((d.DebuggingFlags & DebuggableAttribute.DebuggingModes.Default) == DebuggableAttribute.DebuggingModes.None)
                return true;
            //if ((d.DebuggingFlags & DebuggableAttribute.DebuggingModes.DisableOptimizations) == DebuggableAttribute.DebuggingModes.None)
            //    return true;
            //if ((d.DebuggingFlags & DebuggableAttribute.DebuggingModes.EnableEditAndContinue) == DebuggableAttribute.DebuggingModes.None)
            //    return true;
            return false;
        }

        /// <summary>
        /// Stop the code execution if the both conditions are met: 
        /// 1)the entry assembly was compiled as a release version;  
        /// 2)the test period expired.
        /// </summary>
        /// <param name="year">year when the test period expires; if it is less than 1 then the check is not performed</param>
        /// <param name="month">month when the test period expires</param>
        /// <param name="day">day when the test period expires</param>
        /// <param name="for_release_only"></param>
        static public void CHECK_TEST_PERIOD_VALIDITY(int year, int month, int day, bool silently = false, bool for_release_only = false)
        {
            try
            {
                InternetDateTime.silently = silently;
                //if (year < 1)
                //    return;
                //if (for_release_only && !IsReleaseVersion(Assembly.GetEntryAssembly()))
                //    return;
                //LogMessage.Inform("It is a demo version that is valid until " + year + "-" + month + "-" + day);
                if (!silently)
                    Log.Main.Inform("It is a demo version that is valid until " + year + "-" + month + "-" + day);
                ThreadRoutines.StartTry(() =>
                {
                    if (new DateTime(year, month, day) < InternetDateTime.GetOverHttp())
                    {
                        Action a = (Action)(() => {
                            LogMessage.Exit("The test time expired. \nPlease contact the vendor if you want to use this software.");
                        });
                        if (System.Windows.Forms.Application.OpenForms.Count > 0)
                            System.Windows.Forms.Application.OpenForms[0].Invoke(a);
                        //else if (System.Windows.Application.Current != null)
                        //    System.Windows.Application.Current.Dispatcher.Invoke(a);
                        else
                        {
                            ThreadRoutines.Start(() =>
                            {
                                Thread.Sleep(5000);
                                Environment.Exit(0);
                            });
                            a();
                        }
                    }
                },
                (Exception e) =>
                {
                    LogMessage.Exit(e);
                }
                );
            }
            catch (Exception e)
            {
                LogMessage.Exit(e);
            }
        }
        static bool silently = false;

        /// <summary>
        /// Get the current datetime from a https response header.
        /// </summary>
        /// <returns></returns>
        static public DateTime GetOverHttps()
        {
            //"https://www.google.com";
            //"https://www.2checkout.com";
            //"q" + DateTime.Now.Ticks.ToString() is used to avoid proxy caching
            return get("https://www.verisign.com?q=" + DateTime.Now.Ticks.ToString());
        }

        /// <summary>
        /// Get the current datetime from a http response header.
        /// </summary>
        /// <returns></returns>
        static public DateTime GetOverHttp()
        {
            //DateTime.Now.Ticks.ToString() is used to avoid proxy caching
            return get("http://www.google.com/search?q=" + DateTime.Now.Ticks.ToString());
            //return get("http://www.google.com/");
        }

        static DateTime get(string url, int timeout_in_secs = 10)
        {
            try
            {
                string dts = null;
                data_received = false;
                using (WebClient w = new WebClient())
                {
                    w.DownloadProgressChanged += new DownloadProgressChangedEventHandler(w_DownloadProgressChanged);
                    Uri uri = new Uri(url);
                    w.DownloadStringAsync(uri);
                    DateTime timeout = DateTime.Now.AddSeconds(timeout_in_secs);
                    while (w.IsBusy && !data_received && DateTime.Now < timeout)
                        Thread.Sleep(100);

                    if (w.ResponseHeaders == null)
                        throw (new Exception("This version of the application requires access to the internet to check its validity.\nPlease connect your computer to the internet and restart the application."));

                    dts = w.ResponseHeaders["Date"];

                    w.CancelAsync();
                }

                if (dts == null || dts.Length <= 0)
                    throw (new Exception("Could not find Date header in the response."));

                DateTime dt;
                if (!DateTime.TryParse(dts, out dt))
                    throw (new Exception("Could not convert Date header to DateTime."));

                return dt;
            }
            catch (Exception e)
            {
                if (silently)
                    Message.Error("Validation failed.\n\n" + e.Message);
                else
                    LogMessage.Exit("Test period validation failed.\n\n" + e.Message);
                //Environment.Exit(0);
            }

            return new DateTime(2100, 1, 1);
        }

        static void w_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            data_received = true;
        }

        static bool data_received = false;
    }
}