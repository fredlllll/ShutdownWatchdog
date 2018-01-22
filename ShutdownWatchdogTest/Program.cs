using System;
using System.Diagnostics;
using System.Threading;
using ShutdownWatchdogLeash;

namespace ShutdownWatchdogTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Test1();
        }

        static void Test1()
        {
            Process p = new Process();
            p.StartInfo.FileName = "ShutdownWatchdog.exe";
            p.Start();

            Thread.Sleep(2500);

            WatchdogLeash leash = new WatchdogLeash();
            leash.Feed(2);

            leash.AddProcessWatch(Process.GetCurrentProcess().Id);

            Console.ReadKey();
        }
    }
}
