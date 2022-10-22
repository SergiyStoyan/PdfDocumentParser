//********************************************************************************************
//Author: Sergiy Stoyan
//        systoyan@gmail.com
//        sergiy.stoyan@outlook.com
//        stoyan@cliversoft.com
//        http://www.cliversoft.com
//********************************************************************************************
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

