//********************************************************************************************
//Author: Sergey Stoyan, CliverSoft.com
//        http://cliversoft.com
//        stoyan@cliversoft.com
//        sergey.stoyan@gmail.com
//        27 February 2007
//Copyright: (C) 2007, Sergey Stoyan
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Collections;
using System.Web;
using System.Net.Sockets;
using System.Net;


namespace Cliver
{
    public static class NetworkRoutines
    {
        static public IPAddress GetLocalIpForDestination(IPAddress destination_ip)
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                try
                {
                    socket.Connect(destination_ip, 0);
                }
                catch(Exception e)
                {
                    return null;
                }
                IPEndPoint iep = (IPEndPoint)socket.LocalEndPoint;
                if (iep == null)
                    return null;
                return iep.Address;
            }
        }

        public static bool IsNetworkAvailable()
        {
            return System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
        }

        //public static string GetLocalIPAddress()
        //{
        //    var host = Dns.GetHostEntry(Dns.GetHostName());
        //    foreach (var ip in host.AddressList)
        //    {
        //        if (ip.AddressFamily == AddressFamily.InterNetwork)
        //        {
        //            return ip.ToString();
        //        }
        //    }
        //    throw new Exception("No network adapters with an IPv4 address in the system!");
        //}

        static public string GetLocalIpForDestinationAsString(IPAddress destination_ip)
        {
            return GetLocalIpForDestination(destination_ip)?.ToString();
        }
    }
}

