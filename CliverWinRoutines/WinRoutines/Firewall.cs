/********************************************************************************************
        Author: Sergiy Stoyan
        s.y.stoyan@gmail.com, sergiy.stoyan@outlook.com, stoyan@cliversoft.com
        http://www.cliversoft.com
********************************************************************************************/
using System;
using System.Diagnostics;


namespace Cliver.Win
{
    public static class Firewall
    {
        public enum Direction
        {
            IN,
            OUT
        }

        public static void AllowProgram(string programDisplayName, string programExeFileName, Direction direction)
        {
            string stdout;
            string stderr;
            performNetsh("advfirewall firewall add rule name=\"" + programDisplayName + "\" dir=" + direction.ToString() + " action=allow program=\"" + programExeFileName + "\"", out stdout, out stderr);
            //if (!Regex.IsMatch(stdout, "Ok."))
            //    throw new Exception(stdout + "\r\nstderr: " + stderr);
            if (!string.IsNullOrEmpty(stderr))
                throw new Exception(stderr);
        }

        public static void DeleteRule(string ruleName, string programExeFileName = null)
        {
            string stdout;
            string stderr;
            string arguments = "advfirewall firewall delete rule name=\"" + ruleName + "\"";
            if (programExeFileName != null)
                arguments += " program=\"" + programExeFileName + "\"";
            performNetsh(arguments, out stdout, out stderr);
            //if (!Regex.IsMatch(stdout, "Ok."))
            //    throw new Exception(stdout + "\r\nstderr: " + stderr);
            if (!string.IsNullOrEmpty(stderr))
                throw new Exception(stderr);
        }

        public static void BlockProgram(string programDisplayName, string programExeFileName, Direction direction)
        {
            DeleteRule(programDisplayName, programExeFileName);

            string stdout;
            string stderr;
            performNetsh("advfirewall firewall add rule name=\"" + programDisplayName + "\" dir=" + direction.ToString() + " action=block program=\"" + programExeFileName + "\"", out stdout, out stderr);
            //if (!Regex.IsMatch(stdout, "Ok."))
            //    throw new Exception(stdout + "\r\nstderr: " + stderr);
            if (!string.IsNullOrEmpty(stderr))
                throw new Exception(stderr);
        }

        static void performNetsh(string arguments, out string stdout, out string stderr)
        {
            Process p = Process.Start(
                  new ProcessStartInfo
                  {
                      FileName = "netsh",
                      Arguments = arguments,
                      WindowStyle = ProcessWindowStyle.Hidden,
                      CreateNoWindow = true,
                      RedirectStandardOutput = true,
                      RedirectStandardError = true,
                      UseShellExecute = false,
                  });
            stdout = p.StandardOutput.ReadToEnd();
            stderr = p.StandardError.ReadToEnd();
            p.WaitForExit();
        }
    }
}

