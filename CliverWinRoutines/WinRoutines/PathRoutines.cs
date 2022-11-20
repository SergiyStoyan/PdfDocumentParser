/********************************************************************************************
        Author: Sergiy Stoyan
        s.y.stoyan@gmail.com, sergiy.stoyan@outlook.com, stoyan@cliversoft.com
        http://www.cliversoft.com
********************************************************************************************/

using System.Management;
using System;
using System.Linq;

namespace Cliver.Win
{
    public static class PathRoutines
    {
        public static string GetLocalPathForUncPath(string uncOrLocalPath)
        {
            string uncPath = uncOrLocalPath.Replace(@"\\", "");
            string[] uncParts = uncPath.Split(new char[] { '\\' }, System.StringSplitOptions.RemoveEmptyEntries);
            if (uncParts.Length < 2)
                throw new Exception("Could not resolve UNC path: " + uncPath);
            // Get a connection to the server as found in the UNC path
            ManagementScope scope = new ManagementScope(@"\\" + uncParts[0] + @"\root\cimv2");
            // Query the server for the share name
            SelectQuery query = new SelectQuery("Select * From Win32_Share Where Name = '" + uncParts[1] + "'");
            ManagementObjectSearcher mos = new ManagementObjectSearcher(scope, query);

            // Get the path
            string path = string.Empty;
            foreach (ManagementObject mo in mos.Get())
                path = mo["path"].ToString();

            // Append any additional folders to the local path name
            if (uncParts.Length > 2)
                for (int i = 2; i < uncParts.Length; i++)
                    path += @"\" + uncParts[i];

            return Cliver.PathRoutines.GetNormalizedPath(path, false);
        }
    }
}