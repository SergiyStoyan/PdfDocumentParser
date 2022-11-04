/********************************************************************************************
        Author: Sergiy Stoyan
        s.y.stoyan@gmail.com, sergiy.stoyan@outlook.com, stoyan@cliversoft.com
        http://www.cliversoft.com
********************************************************************************************/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Management;
using System.Diagnostics;
using System.Linq;
using Microsoft.Win32;

namespace Cliver.Win
{
    public static class Wmi
    {
        static public Tuple<string, string> Win32_BaseBoard_SerialNumber = new Tuple<string, string>("Win32_BaseBoard", "SerialNumber");
        static public Tuple<string, string> Win32_Processor_ProcessorID = new Tuple<string, string>("Win32_Processor", "ProcessorID");
        static public Tuple<string, string> Win32_BIOS_Manufacturer = new Tuple<string, string>("Win32_BIOS", "Manufacturer");
        static public Tuple<string, string> Win32_BIOS_SMBIOSBIOSVersion = new Tuple<string, string>("Win32_BIOS", "SMBIOSBIOSVersion");
        static public Tuple<string, string> Win32_BIOS_IdentificationCode = new Tuple<string, string>("Win32_BIOS", "IdentificationCode");
        static public Tuple<string, string> Win32_BIOS_SerialNumber = new Tuple<string, string>("Win32_BIOS", "SerialNumber");
        static public Tuple<string, string> Win32_BIOS_ReleaseDate = new Tuple<string, string>("Win32_BIOS", "ReleaseDate");
        static public Tuple<string, string> Win32_BIOS_Version = new Tuple<string, string>("Win32_BIOS", "Version");
        static public Tuple<string, string> Win32_OperatingSystem_SerialNumber = new Tuple<string, string>("Win32_OperatingSystem", "SerialNumber");
        static public Tuple<string, string> Win32_OperatingSystem_PlusProductID = new Tuple<string, string>("Win32_OperatingSystem", "PlusProductID");

        public static IEnumerable<string> GetProperty(Tuple<string, string> wmiRequest)
        {
            return GetProperty(wmiRequest.Item1, wmiRequest.Item2);
        }

        public static IEnumerable<string> GetProperty(string wmiClass, string wmiProperty)
        {
            ManagementClass mc = new ManagementClass(wmiClass);
            foreach (ManagementObject mo in mc.GetInstances())
            {
                var v = mo[wmiProperty];
                if(v != null)
                    yield return v.ToString().Trim();
            }
        }
    }
}