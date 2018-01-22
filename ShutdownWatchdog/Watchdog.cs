using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using ShutdownWatchdog.Util;

namespace ShutdownWatchdog
{
    public class Watchdog : RestartableThreadClass
    {
        public DateTime ShutdownTime { get; protected set; }

        List<IWatch> watches = new List<IWatch>();

        TimeSpan foodUnit;

        public Watchdog() : base(false)
        {
            foodUnit = TimeSpan.FromSeconds(300);//todo: read from config?
        }

        public override void Start()
        {
            ShutdownTime = DateTime.UtcNow + foodUnit;
            base.Start();
        }

        public void Feed(double units = 1)
        {
            DateTime nextShutdownTime = DateTime.UtcNow + TimeSpan.FromSeconds(foodUnit.TotalSeconds * units);
            if(nextShutdownTime > ShutdownTime)
            {
                ShutdownTime = nextShutdownTime;
            }
        }

        protected override void Run()
        {
            try
            {
                while(true)
                {
                    if(DateTime.UtcNow > ShutdownTime)
                    {
                        ShutdownMachine();
                        break;
                    }
                    Thread.Sleep(5000);
                }
            }
            catch(ThreadInterruptedException)
            {
                //ending
            }
        }

        void ShutdownMachine()
        {
            Process process = new Process();
            process.StartInfo.UseShellExecute = true;
            if(Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                process.StartInfo.FileName = "shutdown";
                process.StartInfo.Arguments = "-s -t 0";
            }
            else if(Environment.OSVersion.Platform == PlatformID.Unix)
            {
                process.StartInfo.FileName = "shutdown";
                process.StartInfo.Arguments = "-h now";
            }
            process.Start();
        }
    }
}
