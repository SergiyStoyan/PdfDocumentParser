using System;
using System.Threading;
using Cliver;

namespace CliverRoutinesTests
{
    class Program
    {
        static void Main(string[] args)
        {            
            try
            {
                DateTimeRoutinesTests t = new DateTimeRoutinesTests();
                t.TestDate();
                t.TestDateTime();
                t.TestTime();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
