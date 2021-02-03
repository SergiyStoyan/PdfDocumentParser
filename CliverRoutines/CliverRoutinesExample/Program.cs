using System;
using System.Threading;
using Cliver;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {            
            try
            {
                LogExample.Run();

                ConfigExample.Run();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
