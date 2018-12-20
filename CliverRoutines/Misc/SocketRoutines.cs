//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006, Sergey Stoyan
//********************************************************************************************
using System;
using System.Linq;
using System.Threading;
using System.Net.Sockets;


namespace Cliver
{
    public static class SocketRoutines
    {
        static public bool IsConnectionAlive(this Socket socket)
        {
            return socket.Connected && !(socket.Poll(0, SelectMode.SelectRead) && socket.Available == 0);

            //bool blockingState = socket.Blocking;
            //try
            //{
            //    socket.Blocking = false;
            //    byte[] tmp = new byte[1];
            //    socket.Send(tmp, 0, 0);
            //    return true;
            //}
            //catch (SocketException e)
            //{
            //    if (e.NativeErrorCode.Equals(10035))// 10035 == WSAEWOULDBLOCK
            //        return true;
            //    return false;
            //}
            //finally
            //{
            //    socket.Blocking = blockingState;
            //}
        }
    }
}

